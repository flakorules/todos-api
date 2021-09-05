using todos.api.Entities;

namespace todos.api.Abstractions.Helpers
{
    public interface IEncryptionHelper
    {
        public string EncryptString(string plainText);
        public int GetUserIdFromBearerToken(string bearerToken);
        public string CreateToken(User user);
    }
}
