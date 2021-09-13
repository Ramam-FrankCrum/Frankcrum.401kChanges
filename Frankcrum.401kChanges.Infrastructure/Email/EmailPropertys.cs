using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Frankcrum.DeductionChanges.Infrastructure.Email
{
    public class EmailPropertys
    {
        private const int MaxMessageLength = 1024 * 1000;
        public Guid Id { get; set; }
        public EmailUser Sender { get; set; }
        public List<EmailUser> Receivers { get; set; }

        public string Subject { get; set; }

        [MaxLength(MaxMessageLength)]
        public string BodyUrl { get; set; }
        public List<string> AttachmentsUrl { get; set; }
        public string Body { get; set; }
        public List<EmailUser> CC { get; set; }

        public List<EmailUser> BCC { get; set; }

        public bool IsBodyHtml { get; set; }
    }
}
