using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frankcrum.DeductionChanges.Infrastructure
{
    public class InfrastructureSettings
    {
        public InfrastructureSettings(string myFrankCrumConnectionString, string worklioConnectionString
            , string tokenEndpoint, string clientId, string clientSecret, string scope,
           string legacyClientId, string legacyClientSecret, string mailServer,
           string fromEmailAddress, string messageTemplateHeader, string messageTemplateFooter, bool isProduction, string testEmailAddress,
          string emailServiceUrl
            )
        {
            MyFrankCrumConnectionString = myFrankCrumConnectionString;
            WorklioConnectionString = worklioConnectionString;
            TokenEndpoint = tokenEndpoint;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Scope = scope;
            LegacyClientId = legacyClientId;
            LegacyClientSecret = legacyClientSecret;
            MailServer = mailServer;
            FromEmailAddress = fromEmailAddress;
            MessageTemplateHeader = messageTemplateHeader;
            MessageTemplateFooter = messageTemplateFooter;
            IsProduction = isProduction;
            TestEmailAddress = testEmailAddress;
            EmailServiceUrl = emailServiceUrl;

        }

        public Uri AuthApiBaseUrl { get; }
        public string BenefitPlanDocumentPath { get; }
        public string EmailClientID { get; }
        public string EmailClientSecrate { get; }
        public string EmailScope { get; }
        public string EmailServiceUrl { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string FromEmailAddress { get; }
        public bool IsProduction { get; }
        public string LegacyClientId { get; }
        public string LegacyClientSecret { get; }
        public string MailServer { get; }
        public string MessageTemplateFooter { get; }
        public string MessageTemplateHeader { get; }
        public string MyFrankCrumConnectionString { get; }
        public string WorklioConnectionString { get; set; }
        public Uri PayrollApiBaseUrl { get; }
        public string Scope { get; }
        public string TestEmailAddress { get; }
        public string TokenEndpoint { get; }
        public string TokenRevocationEndpoint { get; }
        public int TokenRevokeGraceSeconds { get; }

    }
}
