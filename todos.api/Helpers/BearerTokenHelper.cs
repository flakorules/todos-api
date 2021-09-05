using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
