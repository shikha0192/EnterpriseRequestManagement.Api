using Microsoft.Data.SqlClient;
using System.Data;

namespace EnterpriseRequestManagement.Api.Data
{
    public class DbConnectionFactory(IConfiguration config)
    {
        private readonly IConfiguration _config = config;

        public IDbConnection Create()
            => new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    }
}