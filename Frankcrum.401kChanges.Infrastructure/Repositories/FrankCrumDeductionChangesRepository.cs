using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frankcrum.DeductionChanges.Applications.Entities;
using Frankcrum.DeductionChanges.Applications.Interfaces.Repositories;
using Frankcrum.DeductionChanges.Infrastructure.Interfaces;
using Dapper;
using System.Data;
using Serilog;

namespace Frankcrum.DeductionChanges.Infrastructure.Repositories
{
    public class FrankCrumDeductionChangesRepository:IFrankCrumDeductionChangesRepository
    {
        private readonly InfrastructureSettings _settings;
        ISQLConnectionFactory _connectionFactory;
        public FrankCrumDeductionChangesRepository(ISQLConnectionFactory connectionFactory, InfrastructureSettings settings)
        {
            _connectionFactory = connectionFactory;
            _settings = settings;

        }
        public async Task<IEnumerable<CorporatePayGroups>> GetCorporatePaygroups()
        {
            try
            {
                Console.WriteLine("Geting CorporatePaygroups ");
                Log.Information("Get GetCorporatePaygroups Method");
                using (var connection = _connectionFactory.GetConnection(_settings.MyFrankCrumConnectionString))
                {
                    var result = await connection.QueryAsync<CorporatePayGroups>(StoredProcedures.Worklio_401kChanges, commandType: CommandType.StoredProcedure, commandTimeout: 1800).ConfigureAwait(true);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error(ex, "Error");
                return null;
            }

        }
    }
}
