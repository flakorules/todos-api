using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using todos.api.Abstractions.Repository;
using todos.api.DTO;

namespace todos.api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequestDTO request)
        {
            if(await _userRepository.RegisterUser(request))
            {
                return Ok(new { OK = true, message = $"Usuario {request.UserName} creado correctamente." });
            }
            else
            {
                return BadRequest(new { OK = false, message = $"Error al crear el Usuario {request.UserName}." });
            }
        }

        /// <summary>
        /// Autentica un usuario y devuelve token para ser utilizado para las peticiones en endpoint de Todos
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateUserRequestDTO request)
        {
            var authToken = await _userRepository.AuthenticateUser(request);

            if (!string.IsNullOrEmpty(authToken))
            {
                return Ok(new { OK = true, token = authToken });
            }
            else
            {
                return BadRequest(new { OK = false, message = $"Error al autenticar el Usuario {request.UserName}." });
            }
        }
    }
}
