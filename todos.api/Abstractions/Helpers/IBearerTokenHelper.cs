using Microsoft.AspNetCore.Http;

namespace todos.api.Abstractions.Helpers
{
    public interface IBearerTokenHelper
    {
        string GetBearerToken(HttpRequest request);
    }
}
