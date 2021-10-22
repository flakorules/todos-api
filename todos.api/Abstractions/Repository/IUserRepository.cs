using System.Threading.Tasks;
using todos.api.DTO;

namespace todos.api.Abstractions.Repository
{
    public interface IUserRepository
    {
        Task<GenericResponseDTO<RegisterUserResponseDTO>> RegisterUser(RegisterUserRequestDTO request);
        GenericResponseDTO<string> AuthenticateUser(AuthenticateUserRequestDTO request);
        GenericResponseDTO<GetUserResponseDTO> GetByUserName(string userName);
    }

}
