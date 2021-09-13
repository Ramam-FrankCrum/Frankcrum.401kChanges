using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Frankcrum.DeductionChanges.Infrastructure.Email
{
    public class EmailUser
    {
        public EmailUser()
        {
        }

        public string EmailAddress { get; set; }

        [MaxLength(500)]
        public string Name { get; set; }
    }
}
