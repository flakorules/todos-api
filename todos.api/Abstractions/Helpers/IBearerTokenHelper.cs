using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace todos.api.Abstractions.Helpers
{
    public interface IBearerTokenHelper
    {
        string GetBearerToken(HttpRequest request);
    }
}
