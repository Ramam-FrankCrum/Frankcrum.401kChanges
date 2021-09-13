using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frankcrum.DeductionChanges.Applications.Entities
{
   public class DeductionChangesEmailRequestBody
    {
        public string NetAmt { get; set; }
        public string DocNo { get; set; }
        public string HourlyRate { get; set; }
        public string TotalHours { get; set; }

        public string EmpNo { get; set; }
        public string Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public string EmployeeName { get; set; }
        public string SalaryHourly { get; set; }
        public string PayGroup { get; set; }
        public string PerControl { get; set; }
        public string PrintDate { get; set; }

        public string PrintUser { get; set; }
    }
}
