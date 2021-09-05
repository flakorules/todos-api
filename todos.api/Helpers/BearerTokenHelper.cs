using Microsoft.AspNetCore.Http;
using todos.api.Abstractions.Helpers;

namespace todos.api.Helpers
{
    public class BearerTokenHelper: IBearerTokenHelper
    {
        public string GetBearerToken(HttpRequest request)
        {
            return request.Headers["Authorization"].ToString();
        }
    }
}
