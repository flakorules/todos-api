using System.Threading.Tasks;
using todos.api.DTO;

namespace todos.api.Abstractions.Repository
{
    public interface IUserRepository
    {
        Task<bool> RegisterUser(RegisterUserRequestDTO request);
        Task<string> AuthenticateUser(AuthenticateUserRequestDTO request);
    }

}
