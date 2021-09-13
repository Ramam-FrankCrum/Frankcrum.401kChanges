using Frankcrum.DeductionChanges.Applications.Entities;
using Frankcrum.DeductionChanges.Applications.Interfaces.Email;
using Frankcrum.DeductionChanges.Infrastructure.Email;
using Frankcrum.DeductionChanges.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frankcrum.DeductionChanges.Infrastructure.Common
{
    public class EmailSender:IEmailSender
    {
        private readonly IEmailService _emailService;
        private static EmailSettings _emailSettings;
        private readonly IConfiguration _config;


        public EmailSender(IEmailService emailService, EmailSettings emailSettings, IConfiguration config)
        {
            _emailService = emailService;
            _emailSettings = emailSettings;
            _config = config;
        }

        public static string GetEmailMessageBody(List<DeductionChangesRequestResponse> statsRequestResponse)
        {
            string body = _emailSettings.EmailTemplate;
            body = body.Replace("{tablerowplaceholder}", GenerateTableRow(statsRequestResponse), StringComparison.OrdinalIgnoreCase);
            return body;
        }

        public string SendEmailResponse(IEnumerable<DeductionChangesRequestResponse> statsRequestResponse)
        {
            MemoryStream ms = new MemoryStream();
            string recptAddress = _emailSettings.RecipientEmailAddress;
            string ccAddress = _emailSettings.ccAddress;
            string bccAddress = _emailSettings.bccAddress;
            string attachmentfilename = _emailSettings.attachmentFileName;

            string subject = "HR - 401K Changes";
            var messageBody = EmailSender.GetEmailMessageBody(statsRequestResponse.ToList());
            string result = _emailService.SendNotification(DateTime.Now, recptAddress, ccAddress, bccAddress, subject, messageBody, attachmentfilename, ms);
            return result;
        }

        private static string GenerateTableRow(List<DeductionChangesRequestResponse> statsRequestResponse)
        {
            var result = new StringBuilder();
            statsRequestResponse.ForEach(data =>
            {
                result.Append("<tr>");

                result.Append("<td style='border:0;padding:2px 3px;'>" + data.CompanyCode + "</td> ");
                result.Append("<td style='border:0;padding:2px 3px;'>" + data.CompanyName + "</td> ");
                result.Append("<td style='border:0;padding:2px 3px;'>" + data.EmployeeName + "</td> ");
                result.Append("<td style='border:0;padding:2px 3px;'>" + data.EmployeeNumber + "</td> ");
                result.Append("<td style='border:0;padding:2px 3px;'>" + data.DedCode + "</td> ");
                result.Append("<td style='border:0;padding:2px 3px;'>" + data.ChangeDate + "</td> ");              
                result.Append("<td style='border:0;padding:2px 3px;'>" + data.ChangeReason + "</td> ");
                result.Append("</tr>");
            });

            return result.ToString();
        }
    }
}
