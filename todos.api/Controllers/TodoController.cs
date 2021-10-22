using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using todos.api.Abstractions.Helpers;
using todos.api.Abstractions.Repository;
using todos.api.DTO;
using todos.api.Entities;
using todos.api.Exceptions.Repository;

namespace todos.api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;
        private readonly IBearerTokenHelper _bearerTokenHelper; 
        
        public TodoController(ITodoRepository todoRepository, IBearerTokenHelper bearerTokenHelper)
        {
            _todoRepository = todoRepository;
            _bearerTokenHelper = bearerTokenHelper;
        }

        /// <summary>
        /// Crea un nuevo Todo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateTodoRequestDTO request)
        {
            
            try
            {
                var bearerToken = _bearerTokenHelper.GetBearerToken(Request);
                var resp = await _todoRepository.Create(request, bearerToken);

                return CreatedAtAction(nameof(GetById), new { todoId = resp.Data.TodoId }, resp);

            }
            catch (GenericRepositoryException exception)
            {
                return BadRequest(new GenericResponseDTO<Todo>()
                {
                    ErrorCode = exception.ErrorCode,
                    Message = exception.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Elimina un Todo existente
        /// </summary>
        /// <param name="todoId"></param>
        /// <returns></returns>
        [HttpDelete("{todoId}")]
        public async Task<IActionResult> Delete(int todoId)
        {
            try
            {
                var bearerToken = _bearerTokenHelper.GetBearerToken(Request);
                var response = await _todoRepository.Delete(todoId, bearerToken);
                
                if (response.Data)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (GenericRepositoryException exception)
            {
                return BadRequest(new GenericResponseDTO<bool>()
                {
                    ErrorCode = exception.ErrorCode,
                    Message = exception.Message,
                    Data = false
                });
            }
        }

        /// <summary>
        /// Resuelve un Todo
        /// </summary>
        /// <param name="todoId"></param>
        /// <returns></returns>
        [HttpPatch("{todoId}")]
        public async Task<IActionResult> Solve(int todoId)
        {
            try
            {
                var bearerToken = _bearerTokenHelper.GetBearerToken(Request);
                var response = await _todoRepository.Solve(todoId, bearerToken);


                if (response.Data)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (GenericRepositoryException exception)
            {
                return BadRequest(new GenericResponseDTO<IEnumerable<bool>>()
                {
                    ErrorCode = exception.ErrorCode,
                    Message = exception.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Obtiene los Todos asociados a un usuario
        /// </summary>
        /// <returns></returns>
        [HttpGet("user")]
        public async Task<IActionResult> GetByUserId()
        {
            try
            {
                var bearerToken = _bearerTokenHelper.GetBearerToken(Request);
                return Ok(await _todoRepository.GetByUserId(bearerToken));
            }
            catch (GenericRepositoryException exception)
            {
                return BadRequest(new GenericResponseDTO<IEnumerable<Todo>>()
                {
                    ErrorCode = exception.ErrorCode,
                    Message = exception.Message,
                    Data = null
                });
            }
        }

        [HttpGet("{todoId}")]
        public IActionResult GetById(int todoId)
        {
            try
            {
                return Ok(_todoRepository.GetById(todoId));
            }
            catch (GenericRepositoryException exception)
            {
                return BadRequest(new GenericResponseDTO<IEnumerable<Todo>>()
                {
                    ErrorCode = exception.ErrorCode,
                    Message = exception.Message,
                    Data = null
                });
            }

        }
    }
}
