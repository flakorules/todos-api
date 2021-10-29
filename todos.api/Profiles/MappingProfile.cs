using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todos.api.DTO;
using todos.api.Entities;

namespace todos.api.Profiles
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            CreateMap<RegisterUserRequestDTO, User>();
            CreateMap<User, RegisterUserResponseDTO>();
            CreateMap<User, GetUserResponseDTO>();
            CreateMap<User, AuthenticationResponseDTO>();
            CreateMap<CreateTodoRequestDTO, Todo>();
        }

    }
}
