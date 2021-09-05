using System.Collections.Generic;
using System.Threading.Tasks;
using todos.api.DTO;
using todos.api.Entities;

namespace todos.api.Abstractions.Repository
{
    public interface ITodoRepository
    {
        Task<Todo> Create(CreateTodoRequestDTO request, string bearerToken);
        Task<bool> Solve(int todoId, string bearerToken);
        Task<bool> Delete(int todoId, string bearerToken);
        Task<IEnumerable<Todo>> GetByUserId(int userId, string bearerToken);
    }
}
