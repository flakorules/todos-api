using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todos.api.Persistency;

namespace todos.api.Repository
{
    public abstract class RepositoryBase
    {
        protected TodosDBContext _context;

        public RepositoryBase(TodosDBContext context)
        {
            _context = context;
        }
    }
}
