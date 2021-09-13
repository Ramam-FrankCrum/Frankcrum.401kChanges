using Frankcrum.DeductionChanges.Applications.Interfaces.Email;
using Frankcrum.DeductionChanges.Applications.Interfaces.Repositories;
using Frankcrum.DeductionChanges.Infrastructure;
using Frankcrum.DeductionChanges.Infrastructure.Common;
using Frankcrum.DeductionChanges.Infrastructure.Email;
using Frankcrum.DeductionChanges.Infrastructure.Interfaces;
using Frankcrum.DeductionChanges.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frankcrum.DeductionChanges.Api
{
    public class DeductionChangesProcessor : IHostedService, IDisposable
    {
        private bool _disposedValue = false;
        private static readonly Container container = new Container();
        public static IConfiguration Config { get; set; }
        private readonly IEmailSender _emailSender;

        public DeductionChangesProcessor(IConfiguration configuration)
        {
            Config = configuration;
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            ConfigureSimpleInjector();
        }


        private static void ConfigureSimpleInjector()
        {

            container.Register<ISQLConnectionFactory, ConnectionFactory>();
            container.Register<IDeductionChangesRepository, DeductionChangesRepository>();
            container.Register<IFrankCrumDeductionChangesRepository, FrankCrumDeductionChangesRepository>();
            container.Register<IEmailSender, EmailSender>();
            container.Register<IEmailService, EmailService>();

            var infrastructureSettings = new InfrastructureSettings(
               Config.GetConnectionString(name: "MyFrankCrumConnectionString"),
               Config.GetConnectionString(name: "WorklioConnectionString"),
               Config.GetValue<string>(key: "TokenEndpoint"),
               Config.GetValue<string>(key: "ClientId"),
               Config.GetValue<string>(key: "ClientSecret"),
               Config.GetValue<string>(key: "Scope"),
               Config.GetValue<string>(key: "LegacyClientId"),
               Config.GetValue<string>(key: "LegacyClientSecret"),
               Config.GetValue<string>(key: "MailServer"),
               Config.GetValue<string>(key: "FromEmailAddress"),
               Config.GetValue<string>(key: "MessageTemplateHeader"),
               Config.GetValue<string>(key: "MessageTemplateFooter"),
               Config.GetValue<bool>(key: "IsProduction"),
               Config.GetValue<string>(key: "TestEmailAddress"),
                Config.GetValue<string>(key: "emailServiceUrl")
               );


            var emailSettings = new EmailSettings(
              Config.GetSection("Email").GetValue<string>(key: "MailServer"),
              Config.GetSection("Email").GetValue<string>(key: "FromEmailAddress"),
              Config.GetSection("Email").GetValue<bool>(key: "IsProduction"),
              Config.GetSection("Email").GetValue<string>(key: "TestEmailAddress"),
              Config.GetSection("Email").GetValue<string>(key: "EmailTemplate"),
              Config.GetSection("Email").GetValue<string>(key: "RecipientEmailAddress"),
                Config.GetSection("Email").GetValue<string>(key: "ccAddress"),
                Config.GetSection("Email").GetValue<string>(key: "bccAddress"),
                Config.GetSection("Email").GetValue<string>(key: "attachmentFileName")
                );

            emailSettings.EmailTemplate = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, Config.GetSection("Email").GetValue<string>(key: "EmailTemplate")));


            //Register Logging
            var logFilePath = Config.GetSection("LogFilePath")?.Value;
            Log.Logger = new LoggerConfiguration()
                                        .WriteTo.File(logFilePath, Serilog.Events.LogEventLevel.Information, fileSizeLimitBytes: null)
                                        .ReadFrom.Configuration(Config)
                                        .Enrich.WithMachineName()
                                        .Enrich.WithProcessId()
                                        .Enrich.FromLogContext()
                                        .CreateLogger();


            container.RegisterInstance(Log.Logger);
            PerformElasticsearchCleanup();
            container.RegisterInstance(emailSettings);
            container.RegisterInstance(infrastructureSettings);
            container.RegisterInstance(Config);
            container.Verify();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            try
            {
                System.Console.WriteLine($"Running 401kChanges Processor.");
                Log.Information("Running The 401kChanges Processor.");
                var _frankcrumRepository = container.GetInstance<IFrankCrumDeductionChangesRepository>();
                var _DeductionChangesRrepository = container.GetInstance<IDeductionChangesRepository>();
                var _emailSender = container.GetInstance<IEmailSender>();

                var result = await _DeductionChangesRrepository.Get401kChanges().ConfigureAwait(false);

                _emailSender.SendEmailResponse(result);
                Log.Information("Email Sent");
                Log.Information("Application Completed.");
                System.Console.WriteLine($"Email Sent");
                System.Console.WriteLine($"Application Completed.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
            }
        }

        private static void PerformElasticsearchCleanup()
        {
            // Update elasticsearch to increase the field limit      
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(Config.GetSection("ElasticsearchUri")?.Value)
            };

            Task.Run(() =>
            {
                try
                {
                    httpClient.PutAsync(new Uri($"myfrankcrum-api-{DateTime.Now:yyyy.MM.dd}/_settings", UriKind.Relative), new StringContent(content: "{\"index.mapping.total_fields.limit\": 3000 }", encoding: Encoding.UTF8, mediaType: "application/json")).GetAwaiter().GetResult();
                    CloseOldElasticsearchIndicesAsync(httpClient).GetAwaiter().GetResult();
                    DeleteExpiredElasticsearchIndicesAsync(httpClient).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    //Console.out.WriteLine($"Exception occurred calling elasticsearch: {ex}");
                    Log.Warning(ex, $"Exception occurred calling elasticsearch: {ex.Message}");
                }
            });
        }

        private static async Task CloseOldElasticsearchIndicesAsync(HttpClient httpClient)
        {
            var indicesToClose = CalculateIndicesToClose();

            foreach (var index in indicesToClose)
            {
                // Flush the index to make sure there are no transacton left in the transaction log
                await httpClient.PostAsync(new Uri($"{index}/_flush", UriKind.Relative), content: null).ConfigureAwait(continueOnCapturedContext: false);

                // Close the index
                await httpClient.PostAsync(new Uri($"{index}/_close", UriKind.Relative), content: null).ConfigureAwait(continueOnCapturedContext: false);
            }
        }

        private static List<string> CalculateIndicesToClose()
        {
            var startDate = DateTime.Now.Subtract(TimeSpan.FromDays(value: 21));
            var endDate = DateTime.Now.Subtract(TimeSpan.FromDays(value: 7));

            var days = Enumerable.Range(start: 0, count: 1 + endDate.Subtract(startDate).Days)
                .Select(offset => startDate.AddDays(offset))
                .ToArray();

            return days.Select(day => $"myfrankcrum-api-{day:yyyy.MM.dd}").ToList();
        }

        private static async Task DeleteExpiredElasticsearchIndicesAsync(HttpClient httpClient)
        {
            var indicesToDelete = CalculateIndicesToDelete();

            foreach (var index in indicesToDelete)
            {
                await httpClient.DeleteAsync(new Uri(index, UriKind.Relative)).ConfigureAwait(continueOnCapturedContext: false);
            }
        }

        private static List<string> CalculateIndicesToDelete()
        {
            var startDate = DateTime.Now.Subtract(TimeSpan.FromDays(value: 28));
            var endDate = DateTime.Now.Subtract(TimeSpan.FromDays(value: 21));

            var days = Enumerable.Range(start: 0, count: 1 + endDate.Subtract(startDate).Days)
                .Select(offset => startDate.AddDays(offset))
                .ToArray();

            return days.Select(day => $"myfrankcrum-api-{day:yyyy.MM.dd}").ToList();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        ~DeductionChangesProcessor()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing)
                {
                    if (container != null)
                    {
                        container.Dispose();
                    }
                }

                this._disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
