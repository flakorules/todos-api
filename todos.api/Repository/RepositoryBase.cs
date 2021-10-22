using AutoMapper;
using todos.api.Persistency;

namespace todos.api.Repository
{
    public abstract class RepositoryBase
    {
        protected TodosDBContext _context;
        protected IMapper _mapper;
        public RepositoryBase(TodosDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
