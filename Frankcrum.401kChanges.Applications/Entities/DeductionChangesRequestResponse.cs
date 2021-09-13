using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frankcrum.DeductionChanges.Applications.Entities
{
    public class DeductionChangesRequestResponse
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNumber { get; set; }
        public string DedCode { get; set; }
        public string ChangeDate { get; set; }
        public string ChangeReason { get; set; }



    }
}
