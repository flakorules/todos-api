using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace todos.api.Abstractions.Helpers
{
    public interface IEncriptionHelper
    {
        public string EncryptString(string plainText);
        public int GetUserIdFromBearerToken(string bearerToken);
    }
}
