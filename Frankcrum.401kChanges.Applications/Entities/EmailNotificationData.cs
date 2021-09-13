using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frankcrum.DeductionChanges.Applications.Entities
{
    public class EmailNotificationData
    {
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool NotifyOnly { get; set; }
    }
}
