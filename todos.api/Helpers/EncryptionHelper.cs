using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using todos.api.Abstractions.Helpers;
using todos.api.Config;
using todos.api.Entities;

namespace todos.api.Helpers
{
    public class EncryptionHelper: IEncryptionHelper
    {

        private readonly EncryptionConfig _encryptionConfig;
        private readonly JwtConfig _jwtConfig;

        public EncryptionHelper(IOptions<EncryptionConfig> myConfiguration, IOptions<JwtConfig> jwtConfig)
        {
            _encryptionConfig = myConfiguration.Value;
            _jwtConfig = jwtConfig.Value;
        }

        public string EncryptString(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_encryptionConfig.Key);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using MemoryStream memoryStream = new MemoryStream();
                using CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
                using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                {
                    streamWriter.Write(plainText);
                }
                array = memoryStream.ToArray();
            }
            return Convert.ToBase64String(array);
        }

        public string CreateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_jwtConfig.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public int GetUserIdFromBearerToken(string bearerToken)
        {
            var tokenArray = bearerToken.Split(' ');
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenArray[1]);
            return int.Parse(token.Claims.First(c => c.Type == "UserId").Value);
        }
    }
}
