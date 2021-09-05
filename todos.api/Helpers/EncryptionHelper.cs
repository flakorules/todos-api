using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using todos.api.Abstractions.Helpers;
using todos.api.Config;

namespace todos.api.Helpers
{
    public class EncryptionHelper: IEncriptionHelper
    {

        private readonly EncryptionConfig _encryptionConfig;

        public EncryptionHelper(IOptions<EncryptionConfig> myConfiguration)
        {
            _encryptionConfig = myConfiguration.Value;
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

        public int GetUserIdFromBearerToken(string bearerToken)
        {
            var tokenArray = bearerToken.Split(' ');
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenArray[1]);
            return int.Parse(token.Claims.First(c => c.Type == "UserId").Value);
        }
    }
}
