using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frankcrum.DeductionChanges.Applications.Entities;


namespace Frankcrum.DeductionChanges.Applications.Interfaces.Repositories
{
    public interface IDeductionChangesRepository
    {
        Task<IEnumerable<DeductionChangesRequestResponse>> Get401kChanges();

    }
}
