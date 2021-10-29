using AutoMapper;
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
    public class UserRepository : RepositoryBase, IUserRepository
    {
        private readonly IEncryptionHelper _encryptionHelper;
        
        public UserRepository(TodosDBContext context, IEncryptionHelper encryptionHelper, IMapper mapper) : base(context, mapper)
        {
            _encryptionHelper = encryptionHelper;
        }

        public GenericResponseDTO<AuthenticationResponseDTO> AuthenticateUser(AuthenticateUserRequestDTO request)
        {
            var foundUser = _context.Users
                .FirstOrDefault(user => user.UserName == request.UserName
                && user.Password == _encryptionHelper.EncryptString(request.Password));

            if (foundUser == null)
            {
                throw new GenericRepositoryException("002", $"Error authenticating user {request.UserName}.");
            }

            var resp = _mapper.Map<AuthenticationResponseDTO>(foundUser);
            resp.Token = _encryptionHelper.CreateToken(foundUser);

            return new GenericResponseDTO<AuthenticationResponseDTO>()
            {
                ErrorCode = "000",
                Message = $"User {request.UserName} was authenticated correctly.",
                Data = resp

            };
        }

        public GenericResponseDTO<GetUserResponseDTO> GetByUserName(string userName)
        {
            var foundUser = _context.Users.FirstOrDefault(user => user.UserName == userName);

            if (foundUser == null)
            {
                throw new GenericRepositoryException("003", $"User {userName} was not found.");
            }

            return new GenericResponseDTO<GetUserResponseDTO>()
            {
                ErrorCode = "000",
                Message = $"User name {userName}",
                Data = _mapper.Map<GetUserResponseDTO>(foundUser)
            };
        }
        
        public async Task<GenericResponseDTO<RegisterUserResponseDTO>> RegisterUser(RegisterUserRequestDTO request)
        {

            var foundUser = _context.Users.FirstOrDefault(user => user.UserName == request.UserName);

            if (foundUser != null)
            {
                throw new GenericRepositoryException("001", $"User {request.UserName} already exists in database.");
            }

            var newUser = _mapper.Map<User>(request);
            newUser.Password = _encryptionHelper.EncryptString(request.Password);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            var responseData = _mapper.Map<RegisterUserResponseDTO>(newUser);

            var returnValue = new GenericResponseDTO<RegisterUserResponseDTO>()
            {
                ErrorCode = "000",
                Message = $"User {request.UserName} was created.",
                Data = responseData
            };

            return returnValue;
        }
    }
}
