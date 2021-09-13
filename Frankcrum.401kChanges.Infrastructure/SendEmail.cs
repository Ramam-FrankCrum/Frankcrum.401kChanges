using Frankcrum.DeductionChanges.Infrastructure.Interfaces;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using emailservice = Frankcrum.DeductionChanges.Infrastructure.Email;

namespace Frankcrum.DeductionChanges.Infrastructure
{
    public class SendEmail
    {
        private readonly InfrastructureSettings _infrastructureSettings;
        private readonly IConfiguration _config;
        private IConfiguration config;

        public SendEmail(InfrastructureSettings infrastructureSettings)
        {
            _infrastructureSettings = infrastructureSettings;
        }

        public SendEmail(IConfiguration config)
        {
            this.config = config;
        }

        public async Task<bool> SendEmailAsync(emailservice.EmailPropertys email)
        {
            bool result = false;
            try
            {
                string clientID = _infrastructureSettings?.ClientId;
                string clientSecrate = _infrastructureSettings?.ClientSecret;
                string scope = _infrastructureSettings?.Scope;
                var token = await GetDevAuthToken(clientID, clientSecrate, scope).ConfigureAwait(false);
                string emailUrl = _infrastructureSettings?.EmailServiceUrl;
                var client = new RestClient(emailUrl);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("Content-Type", "application/json");

                var jSON = JsonConvert.SerializeObject(email).ToString();
                request.AddParameter("application/json", jSON, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                result = response.IsSuccessful;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
            }

            return result;
        }

        public async Task<string> GetDevAuthToken(string clientID, string clientSecret, string scope)
        {
            try
            {
                string urlForToken = _infrastructureSettings?.TokenEndpoint;
                string tokens = string.Empty;
                using (var httpClientObj = new HttpClient())
                {
                    var tokenResponse2 = await httpClientObj.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    {
                        Address = urlForToken,
                        ClientId = clientID,
                        ClientSecret = clientSecret,
                        Scope = scope,
                    }).ConfigureAwait(false);

                    if (!tokenResponse2.IsError)
                    {
                        var data = JObject.Parse(Convert.ToString(tokenResponse2.Json));
                        tokens = data["access_token"].Value<string>();
                    }
                }

                return tokens;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
            }

            return null;
        }
    }
}

