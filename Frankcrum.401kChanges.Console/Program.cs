using System;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Serilog;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Frankcrum.DeductionChanges.Api;

namespace Frankcrum.DeductionChanges.Console
{
    public static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                System.Console.WriteLine($"Running The 401kChanges Console application .");
                Log.Information("Running The 401kChanges Console application .");
                MainAsync(args).GetAwaiter().GetResult();
                System.Console.ReadLine();
                Environment.Exit(0);
            }

            catch (Exception ex)
            {
                Log.Error(ex, "Error");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static async Task MainAsync(string[] args)
        {
            var currentLocation = new System.Collections.Generic.Dictionary<string, string>
            {
                { "ContentRoot", Directory.GetCurrentDirectory() }
            };
            var builder = new HostBuilder().ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<DeductionChangesProcessor>();

            }).ConfigureAppConfiguration((hostContext, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
                config.AddInMemoryCollection(currentLocation);
                config.Build();
            }); ;
            await builder.RunConsoleAsync().ConfigureAwait(false);

        }
    }
}

