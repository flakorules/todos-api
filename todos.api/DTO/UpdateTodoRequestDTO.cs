﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace todos.api.DTO
{
    public class UpdateTodoRequestDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
