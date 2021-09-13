using Frankcrum.DeductionChanges.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frankcrum.DeductionChanges.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly InfrastructureSettings infrastructureSettings;
        private readonly IConfiguration _config;
        private static EmailSettings _emailSettings;


        public EmailService(InfrastructureSettings infrastructureSettings, IConfiguration configuration, EmailSettings emailSettings)
        {
            this.infrastructureSettings = infrastructureSettings;
            this._config = configuration;
            _emailSettings = emailSettings;
        }

        public string SendNotification(
           DateTime jobTimestamp,
           string recipientEmailAddress,
           string ccAddress,
           string bccAddress,
           string subject,
           string messageBody,
           string attachmentFileName,
           MemoryStream fileStream)
        {
            string result = string.Empty;
            SendEmail sendEmail = new SendEmail(this.infrastructureSettings);
            if (!this.infrastructureSettings.IsProduction)
            {
                subject = "TEST " + subject;

                recipientEmailAddress = _emailSettings.RecipientEmailAddress;

                if (!string.IsNullOrWhiteSpace(ccAddress))
                {
                    ccAddress = _emailSettings.ccAddress;
                }

                if (!string.IsNullOrWhiteSpace(bccAddress))
                {
                    bccAddress = _emailSettings.bccAddress;
                }
            }

            string notificationBody =
                    "<html><head><meta content=\"text/html; charset=utf-8\" http-equiv=\"Content-Type\"/>" +
                    "<title>" +
                    subject +
                    "</title></head><body style=\"padding:0; margin:0;\">" +

                    messageBody +

                    "</body></html>";

            if (recipientEmailAddress.Contains(".fax", StringComparison.OrdinalIgnoreCase))
            {
                if (recipientEmailAddress.StartsWith("727", StringComparison.OrdinalIgnoreCase))
                {
                    recipientEmailAddress = recipientEmailAddress.Substring(3, recipientEmailAddress.Length - 3);
                }
                else if (recipientEmailAddress.StartsWith("813", StringComparison.OrdinalIgnoreCase))
                {
                    // 813 is local
                }
                else
                {
                    recipientEmailAddress = "1" + recipientEmailAddress;
                }
            }

            EmailUser cCemailUser = new EmailUser();
            EmailUser bCCemailUser = new EmailUser();
            if (!string.IsNullOrWhiteSpace(ccAddress))
            {
                cCemailUser.EmailAddress = ccAddress;
            }

            if (!string.IsNullOrWhiteSpace(bccAddress))
            {
                bCCemailUser.EmailAddress = bccAddress;
            }

            EmailPropertys emailSend = new EmailPropertys()
            {
                Body = notificationBody.ToString(),
                Id = Guid.NewGuid(),
                Subject = subject,
                IsBodyHtml = true,
                Sender = new EmailUser()
                {
                    EmailAddress = _emailSettings.FromEmailAddress.ToString(),
                },
                Receivers = new List<EmailUser>()
                    {
                        new EmailUser()
                        {
                            EmailAddress = recipientEmailAddress,
                        },
                    },
            };
            var isSuccess = sendEmail.SendEmailAsync(emailSend);
            result = isSuccess?.Result.ToString();
            return result;
        }

        public async Task<bool> SendEmailAsync(EmailPropertys email)
        {
            return await new SendEmail(_config).SendEmailAsync(email).ConfigureAwait(true);
        }

    }
}
