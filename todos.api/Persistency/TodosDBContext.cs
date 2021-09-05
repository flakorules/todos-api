﻿using Microsoft.EntityFrameworkCore;
using todos.api.Entities;

namespace todos.api.Persistency
{
    public class TodosDBContext: DbContext
    {
        public TodosDBContext(DbContextOptions options): base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Todo> Todos { get; set; }
    }
}
