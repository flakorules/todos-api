using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using todos.api.Abstractions.Repository;
using todos.api.DTO;
using todos.api.Exceptions.Repository;

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
            try
            {
                var resp = await _userRepository.RegisterUser(request);
                return CreatedAtAction(nameof(GetByUserName), new { userName = request.UserName }, resp);
            }
            catch (GenericRepositoryException exception)
            {
                return BadRequest(new GenericResponseDTO<RegisterUserResponseDTO>()
                {
                    ErrorCode = exception.ErrorCode,
                    Message = exception.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Devuelve los datos del usuario a partir del userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("{userName}")]
        public IActionResult GetByUserName(string userName)
        {
            try
            {
                var user = _userRepository.GetByUserName(userName);
                return Ok(user);
            }
            catch (GenericRepositoryException exception)
            {
                var resp = new GenericResponseDTO<GetUserResponseDTO>()
                {
                    ErrorCode = exception.ErrorCode,
                    Message = exception.Message,
                    Data = null
                };
                return NotFound(resp);
            }
        }

        /// <summary>
        /// Autentica un usuario y devuelve token para ser utilizado para las peticiones en endpoint de Todos
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateUserRequestDTO request)
        {
            try
            {
                var resp = _userRepository.AuthenticateUser(request);
                return Ok(resp);
            }
            catch (GenericRepositoryException exception)
            {
                var resp = new GenericResponseDTO<string>()
                {
                    ErrorCode = exception.ErrorCode,
                    Message = exception.Message,
                    Data = null
                };

                return BadRequest(resp);

            }

        }
    }
}
