using Microsoft.EntityFrameworkCore;
using todos.api.Entities;
using todos.api.Helpers;

namespace todos.api.Persistency
{
    public class TodosDBContext: DbContext
    {
        private readonly ConnectionStringUtility _connectionStringUtility;
        public TodosDBContext(ConnectionStringUtility connectionStringUtility)
        {
            _connectionStringUtility = connectionStringUtility;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(_connectionStringUtility.GetConnectionString());

        public DbSet<User> Users { get; set; }
        public DbSet<Todo> Todos { get; set; }
    }
}
