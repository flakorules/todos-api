using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using todos.api.Abstractions.Helpers;
using todos.api.Abstractions.Repository;
using todos.api.Config;
using todos.api.DTO;
using todos.api.Entities;
using todos.api.Helpers;
using todos.api.Persistency;

namespace todos.api.Repository
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        IEncriptionHelper _encriptionHelper;
        JwtConfig _jwtConfig;
        public UserRepository(TodosDBContext context, IEncriptionHelper encriptionHelper, IOptions<JwtConfig> jwtConfig) : base(context)
        {
            _encriptionHelper = encriptionHelper;
            _jwtConfig = jwtConfig.Value;
        }

        public async Task<string> AuthenticateUser(AuthenticateUserRequestDTO request)
        {
            var foundUser = _context.Users.FirstOrDefault(user => user.UserName == request.UserName);
            
            if (foundUser == null)
            {
                return string.Empty;
            }


            if (_encriptionHelper.EncryptString(request.Password) == foundUser.Password)
            {

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.ASCII.GetBytes(_jwtConfig.Key);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                       
                        new Claim("UserId", foundUser.UserId.ToString()),
                        new Claim(ClaimTypes.Name, foundUser.UserName)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
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
                Password = _encriptionHelper.EncryptString(request.Password),
                Name = request.Name
            };

            _context.Users.Add(newUser);

            return (await _context.SaveChangesAsync() > 0);
        }
    }
}
