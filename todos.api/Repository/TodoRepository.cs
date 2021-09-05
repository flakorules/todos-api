using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todos.api.Abstractions.Helpers;
using todos.api.Abstractions.Repository;
using todos.api.DTO;
using todos.api.Entities;
using todos.api.Exceptions.Repository;
using todos.api.Persistency;

namespace todos.api.Repository
{
    public class TodoRepository : RepositoryBase, ITodoRepository
    {
        private readonly IEncryptionHelper _encriptionHelper;

        public TodoRepository(TodosDBContext context, IEncryptionHelper encriptionHelper) : base(context)
        {
            _encriptionHelper = encriptionHelper;
        }

        public async Task<Todo> Create(CreateTodoRequestDTO request, string bearerToken)
        {
            var userIdInBearerToken = _encriptionHelper.GetUserIdFromBearerToken(bearerToken);

            var newTodo = new Todo()
            {
                UserId = userIdInBearerToken,
                Name = request.Name,
                Solved = false,
                Description = request.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            _context.Todos.Add(newTodo);
            await _context.SaveChangesAsync();
            
            return newTodo;
        }

        public async Task<bool> Delete(int todoId, string bearerToken)
        {
            var todo = _context.Todos.FirstOrDefault(obj => obj.TodoId == todoId);
            
            if (todo == null)
            {
                return false;
            }

            var userIdInBearerToken = _encriptionHelper.GetUserIdFromBearerToken(bearerToken);

            if(userIdInBearerToken != todo.UserId)
            {
                throw new UnauthorizedException("No está autorizado a eliminar los Todos de otro usuario.");
            }

            _context.Todos.Remove(todo);
            return await _context.SaveChangesAsync() > 0;            
        }

        public async Task<IEnumerable<Todo>> GetByUserId(int userId, string bearerToken)
        {
            var userIdInBearerToken = _encriptionHelper.GetUserIdFromBearerToken(bearerToken);

            if (userIdInBearerToken != userId)
            {
                throw new UnauthorizedException("No está autorizado a ver los Todos de otro usuario.");
            }

            return await _context.Todos.Where(todo => todo.UserId == userId).ToListAsync();
        }

        public async Task<bool> Solve(int todoId, string bearerToken)
        {
            var todo = _context.Todos.FirstOrDefault(obj => obj.TodoId == todoId);
            if (todo == null)
            {
                return false;
            }

            var userIdInBearerToken = _encriptionHelper.GetUserIdFromBearerToken(bearerToken);

            if (userIdInBearerToken != todo.UserId)
            {
                throw new UnauthorizedException("No está autorizado a resolver los Todos de otro usuario.");
            }

            todo.Solved = true;
            todo.UpdatedAt = DateTime.Now;

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
