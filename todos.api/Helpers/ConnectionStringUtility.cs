using Microsoft.Extensions.Configuration;
using System;

namespace todos.api.Helpers
{
    public class ConnectionStringUtility
    {
        private readonly IConfiguration _configuration;
        public ConnectionStringUtility(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public string GetConnectionString()
        {
            var IsDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            var connectionString = IsDevelopment ? _configuration["ConnectionString:TodosDB"] : GetHerokuConnectionString();

            return connectionString;
        }

        private string GetHerokuConnectionString()
        {
            string connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            var databaseUri = new Uri(connectionUrl);

            string db = databaseUri.LocalPath.TrimStart('/');
            string[] userInfo = databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);

            return $"User ID={userInfo[0]};Password={userInfo[1]};Host={databaseUri.Host};Port={databaseUri.Port};Database={db};Pooling=true;SSL Mode=Require;Trust Server Certificate=True;";
        }

    }
}
