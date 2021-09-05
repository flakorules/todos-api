using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using todos.api.Abstractions.Helpers;
using todos.api.Abstractions.Repository;
using todos.api.DTO;
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
            var bearerToken = _bearerTokenHelper.GetBearerToken(Request);
            return Ok(await _todoRepository.Create(request, bearerToken));
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
                if (await _todoRepository.Delete(todoId, bearerToken))
                {
                    return Ok(new { OK = true, message = "El todo se ha eliminado correctamente" });
                }
                else
                {
                    return BadRequest(new { OK = true, message = "Error al eliminar el todo" });
                }
            }
            catch (UnauthorizedException exception)
            {
                return BadRequest(new { OK = false, message = exception.Message });
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
                if (await _todoRepository.Solve(todoId, bearerToken))
                {
                    return Ok(new { OK = true, message = "El todo se ha resuelto correctamente" });
                }
                else
                {
                    return BadRequest(new { OK = false, message = "Error al resolver el todo" });
                }
            }
            catch (UnauthorizedException exception)
            {
                return BadRequest(new { OK = false, message = exception.Message });
            }
        }

        /// <summary>
        /// Obtiene los Todos asociados a un usuario
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            try
            {
                var bearerToken = _bearerTokenHelper.GetBearerToken(Request);
                return Ok(await _todoRepository.GetByUserId(userId, bearerToken));
            }
            catch (UnauthorizedException exception)
            {
                return BadRequest(new { OK = false, message = exception.Message });
            }

        }

        
    }
}
