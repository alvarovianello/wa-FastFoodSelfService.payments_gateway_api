using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace Infrastructure.Configurations.Database
{
    public class PostgreSqlConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public PostgreSqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            return new NpgsqlConnection(connectionString);
        }
    }
}
