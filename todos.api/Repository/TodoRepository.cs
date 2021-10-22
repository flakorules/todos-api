using AutoMapper;
using Microsoft.Data.SqlClient;
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

        public TodoRepository(TodosDBContext context, IEncryptionHelper encriptionHelper, IMapper mapper) : base(context,mapper)
        {
            _encriptionHelper = encriptionHelper;
        }

        public async Task<GenericResponseDTO<Todo>> Create(CreateTodoRequestDTO request, string bearerToken)
        {
            try
            {
                #region
                var newTodo = _mapper.Map<Todo>(request);
                newTodo.UserId = _encriptionHelper.GetUserIdFromBearerToken(bearerToken);
                newTodo.Solved = false;
                newTodo.CreatedAt = DateTime.Now;
                newTodo.UpdatedAt = DateTime.Now;

                _context.Todos.Add(newTodo);
                await _context.SaveChangesAsync();

                return new GenericResponseDTO<Todo>()
                {
                    ErrorCode = "000",
                    Message = "Todo was created succesfully",
                    Data = newTodo
                }; 
                #endregion
            }
            catch (SqlException)
            {
                throw new GenericRepositoryException("020", $"Error connecting to the Database.");   
            }            
        }

        public async Task<GenericResponseDTO<bool>> Delete(int todoId, string bearerToken)
        {
            try
            {
                #region
                var todo = _context.Todos.FirstOrDefault(obj => obj.TodoId == todoId);
                if (todo == null)
                {
                    throw new GenericRepositoryException("004", $"Todo {todoId} was not found.");
                }

                var userIdInBearerToken = _encriptionHelper.GetUserIdFromBearerToken(bearerToken);
                if (userIdInBearerToken != todo.UserId)
                {
                    throw new GenericRepositoryException("005", $"Not authorized to delete other's todos.");
                }

                _context.Todos.Remove(todo);

                if (await _context.SaveChangesAsync() > 0)
                {
                    return new GenericResponseDTO<bool>()
                    {
                        ErrorCode = "000",
                        Message = $"Todo {todoId} was deleted successfully",
                        Data = true
                    };
                }
                else
                {
                    return new GenericResponseDTO<bool>()
                    {
                        ErrorCode = "006",
                        Message = $"Todo {todoId} was not deleted",
                        Data = false
                    };
                }
                #endregion
            }
            catch (SqlException)
            {
                throw new GenericRepositoryException("020", $"Error connecting to the Database.");
            }
        }

        public async Task<GenericResponseDTO<IEnumerable<Todo>>> GetByUserId(string bearerToken)
        {
            try
            {
                var userIdInBearerToken = _encriptionHelper.GetUserIdFromBearerToken(bearerToken);

                return new GenericResponseDTO<IEnumerable<Todo>>()
                {
                    ErrorCode = "000",
                    Message = $"Todos from userId {userIdInBearerToken}",
                    Data = await _context.Todos.Where(todo => todo.UserId == userIdInBearerToken).ToListAsync()
                };
            }
            catch (Exception)
            {
                throw new GenericRepositoryException("020", $"Error connecting to the Database.");
            }

        }

        public async Task<GenericResponseDTO<bool>> Solve(int todoId, string bearerToken)
        {
            try
            {
                #region
                var todo = _context.Todos.FirstOrDefault(obj => obj.TodoId == todoId);
                if (todo == null)
                {
                    throw new GenericRepositoryException("007", $"todoId {todoId} doesn't exist");
                }

                var userIdInBearerToken = _encriptionHelper.GetUserIdFromBearerToken(bearerToken);
                if (userIdInBearerToken != todo.UserId)
                {
                    throw new GenericRepositoryException("005", $"Not authorized to Solve other's todos.");
                }

                todo.Solved = true;
                todo.UpdatedAt = DateTime.Now;
                if (await _context.SaveChangesAsync() > 0)
                {
                    return new GenericResponseDTO<bool>()
                    {
                        ErrorCode = "000",
                        Message = $"Todo {todoId} was solved succesfully.",
                        Data = true
                    };
                }
                else
                {
                    return new GenericResponseDTO<bool>()
                    {
                        ErrorCode = "006",
                        Message = $"Todo {todoId} was not solved.",
                        Data = true
                    };
                }
                #endregion
            }
            catch (SqlException)
            {
                throw new GenericRepositoryException("020", $"Error connecting to the Database.");
            }

        }

        public GenericResponseDTO<Todo> GetById(int todoId)
        {
            try
            {
                #region

                var todo = _context.Todos.Where(todo => todo.TodoId == todoId).FirstOrDefault();               

                return new GenericResponseDTO<Todo>()
                {
                    ErrorCode = "000",
                    Message = $"Data from TodoId {todo.TodoId}",
                    Data = todo
                };
                #endregion
            }
            catch (SqlException)
            {
                throw new GenericRepositoryException("020", $"Error connecting to the Database.");
            }
        }
    }
}
