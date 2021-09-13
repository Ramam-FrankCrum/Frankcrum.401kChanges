using System.Data;


namespace Frankcrum.DeductionChanges.Infrastructure.Interfaces
{
    public interface ISQLConnectionFactory
    {
        public IDbConnection GetConnection(string connectionString);
        public IDbConnection GetWorklioConnection(string worklioconnectionstring);
    }
}
