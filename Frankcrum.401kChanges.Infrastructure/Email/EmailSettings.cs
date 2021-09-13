using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frankcrum.DeductionChanges.Infrastructure.Email
{
    public class EmailSettings
    {
        public EmailSettings(string mailServer, string fromEmailAddress, bool isProduction, string testEmailAddress, string emailTemplate, string RecipientEmailAddress, string ccAddress, string bccAddress, string attachmentFileName)
        {
            this.FromEmailAddress = fromEmailAddress;
            this.IsProduction = isProduction;
            this.MailServer = mailServer;
            this.TestEmailAddress = testEmailAddress;
            this.EmailTemplate = emailTemplate;

            this.RecipientEmailAddress = RecipientEmailAddress;
            this.ccAddress = ccAddress;
            this.bccAddress = bccAddress;
            this.attachmentFileName = attachmentFileName;
        }

        public string MessageBodyTemplate { get; set; }

        public string FromEmailAddress { get; }

        public bool IsProduction { get; }

        public string MailServer { get; }

        public string EmailTemplate { get; set; }

        public string TestEmailAddress { get; }

        public string ccAddress { get; }

        public string RecipientEmailAddress { get; }

        public string bccAddress { get; }
        public string attachmentFileName { get; }
    }
}
