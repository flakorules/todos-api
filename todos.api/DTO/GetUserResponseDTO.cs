using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace todos.api.DTO
{
    public class GetUserResponseDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
    }
}
