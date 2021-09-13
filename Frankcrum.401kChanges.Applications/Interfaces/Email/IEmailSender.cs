using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frankcrum.DeductionChanges.Applications.Entities;

namespace Frankcrum.DeductionChanges.Applications.Interfaces.Email
{
    public interface IEmailSender
    {
        //string SendEmailResponse(List<DeductionChangesRequestResponse> result);
       string SendEmailResponse(IEnumerable<DeductionChangesRequestResponse> result);
    }
}
