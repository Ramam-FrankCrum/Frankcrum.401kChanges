using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frankcrum.DeductionChanges.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        string SendNotification(
           DateTime jobTimestamp,
           string recipientEmailAddress,
           string ccAddress,
           string bccAddress,
           string subject,
           string messageBody,
           string attachmentFileName,
           MemoryStream fileStream);
    }
}
