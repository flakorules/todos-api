using Microsoft.EntityFrameworkCore;
using todos.api.Entities;

namespace todos.api.Persistency
{
    public class TodosDBContext: DbContext
    {
        public TodosDBContext(DbContextOptions options): base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("User ID=zdcsyikfsrnkld;Password=2c75cd82518afd0a8feff037c2af1d1fd7315f1ed8b2d3d5c96613bcae4052c1;Host=ec2-23-23-182-238.compute-1.amazonaws.com;Port=5432;Database=dbc1e63ipj5soq;Pooling=true;SSL Mode=Require;Trust Server Certificate=True;");

        public DbSet<User> Users { get; set; }
        public DbSet<Todo> Todos { get; set; }
    }
}
