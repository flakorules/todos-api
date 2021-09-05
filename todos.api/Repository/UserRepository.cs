using System.Linq;
using System.Threading.Tasks;
using todos.api.Abstractions.Helpers;
using todos.api.Abstractions.Repository;
using todos.api.DTO;
using todos.api.Entities;
using todos.api.Persistency;

namespace todos.api.Repository
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        private readonly IEncryptionHelper _encryptionHelper;
        
        public UserRepository(TodosDBContext context, IEncryptionHelper encryptionHelper) : base(context)
        {
            _encryptionHelper = encryptionHelper;            
        }

        public async Task<string> AuthenticateUser(AuthenticateUserRequestDTO request)
        {
            var foundUser = _context.Users.FirstOrDefault(user => user.UserName == request.UserName);
            
            if (foundUser == null)
            {
                return string.Empty;
            }


            if (_encryptionHelper.EncryptString(request.Password) == foundUser.Password)
            {
                return _encryptionHelper.CreateToken(foundUser);
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task<bool> RegisterUser(RegisterUserRequestDTO request)
        {
            var foundUser = _context.Users.FirstOrDefault(user => user.UserName == request.UserName);

            if(foundUser != null)
            {
                return false;
            }

            var newUser = new User()
            {
                UserName = request.UserName,
                Password = _encryptionHelper.EncryptString(request.Password),
                Name = request.Name
            };

            _context.Users.Add(newUser);

            return (await _context.SaveChangesAsync() > 0);
        }
    }
}
