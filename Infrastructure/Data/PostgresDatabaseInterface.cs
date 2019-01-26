using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Data
{
    public class PostgresDatabaseInterface
    {
        private readonly IConfiguration _configuration;

        public PostgresDatabaseInterface(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public IDbConnection OpenConnection(bool matchNamesWithUnderscores = true)  
        {  
            var connection = new NpgsqlConnection(_configuration.GetConnectionString("PostgresConnection"));  
            connection.Open();  
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = matchNamesWithUnderscores;
            return connection;  
        }  
    }
}