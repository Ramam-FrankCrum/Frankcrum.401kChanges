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
    public class DeductionChangesRepository: IDeductionChangesRepository
    {

        ISQLConnectionFactory _connectionFactory;
        private readonly InfrastructureSettings _connectionSettings;
        public DeductionChangesRepository(ISQLConnectionFactory connectionFactory, InfrastructureSettings connectionSettings)
        {
            if (connectionSettings == null)
            {
                throw new ArgumentNullException(nameof(connectionSettings));
            }
            _connectionSettings = connectionSettings;
            _connectionFactory = connectionFactory;

        }
        public async Task<IEnumerable<DeductionChangesRequestResponse>> Get401kChanges()
        {
            try
            {
                Log.Information("Get Get401kChanges Method");
                Console.WriteLine("Geting Get401kChanges Data");
                var connection = _connectionFactory.GetWorklioConnection(_connectionSettings.WorklioConnectionString);

                var result = await connection.QueryAsync<DeductionChangesRequestResponse>(StoredProcedures.Worklio_401kChanges, commandType: CommandType.StoredProcedure, commandTimeout: 1800).ConfigureAwait(true);
                return result;
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
