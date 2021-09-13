using System.Data;
using System.Data.SqlClient;
using Azure.Identity;
using Frankcrum.DeductionChanges.Infrastructure.Interfaces;

namespace Frankcrum.DeductionChanges.Infrastructure
{
    public class ConnectionFactory:ISQLConnectionFactory
    {
        public IDbConnection GetConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public IDbConnection GetWorklioConnection(string worklioConnectionstring)
        {

            var worklioConnection = new SqlConnection(worklioConnectionstring);
            return worklioConnection;
        }
    }
}
