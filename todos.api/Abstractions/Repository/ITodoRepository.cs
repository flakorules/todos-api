using System.Collections.Generic;
using System.Threading.Tasks;
using todos.api.DTO;
using todos.api.Entities;

namespace todos.api.Abstractions.Repository
{
    public interface ITodoRepository
    {
        Task<GenericResponseDTO<Todo>> Create(CreateTodoRequestDTO request, string bearerToken);
        Task<GenericResponseDTO<bool>> Solve(int todoId, string bearerToken);
        Task<GenericResponseDTO<bool>> Delete(int todoId, string bearerToken);
        Task<GenericResponseDTO<IEnumerable<Todo>>> GetByUserId(string bearerToken);
        GenericResponseDTO<Todo> GetById(int todoId);
        Task<GenericResponseDTO<Todo>> Update(int todoId, UpdateTodoRequestDTO request, string bearerToken);
    }
}
