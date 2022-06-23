using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using todos.api.Abstractions.Repository;
using todos.api.DTO;
using todos.api.Exceptions.Repository;

namespace todos.api.tests.Helpers
{
    public class MockHelper
    {
        private static readonly string fakeToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiIyIiwidW5pcXVlX25hbWUiOiJ0ZXJlc28iLCJuYmYiOjE2MzA3ODA3MjcsImV4cCI6MTYzMDc4NDMyNywiaWF0IjoxNjMwNzgwNzI3fQ.v7r20znyg7LCZHCcZShdmZtRmAqTb9Cz1rLSSWK73Uo";
        private static Mock<IUserRepository> mockUserRepository = null;

        public static Mock<IUserRepository> MockUserRepository
        {
            get
            {
                if (mockUserRepository == null){

                    mockUserRepository = new Mock<IUserRepository>();
                    mockUserRepository.SetupSequence(repo => repo.AuthenticateUser(It.IsAny<AuthenticateUserRequestDTO>()))
                        .Throws(new GenericRepositoryException("002", "Error authenticating user cristian."))
                        .Returns(new GenericResponseDTO<AuthenticationResponseDTO>()
                        {
                            ErrorCode = "000",
                            Message = "User cristian was authenticated correctly.",
                            Data = new AuthenticationResponseDTO()
                            {
                                UserId = 1,
                                UserName = "cristian",
                                Token = fakeToken
                            }
                        });

                    mockUserRepository.SetupSequence(repo => repo.RegisterUser(It.IsAny<RegisterUserRequestDTO>()))
                        .ReturnsAsync(
                            new GenericResponseDTO<RegisterUserResponseDTO>()
                            {
                                ErrorCode = "000",
                                Message = "User cristian was created.",
                                Data = new RegisterUserResponseDTO()
                                {
                                    UserId = 1,
                                    UserName = "cristian",
                                    Name = "cristian"
                                }
                            }
                        )
                        .Throws(new GenericRepositoryException("001", "User cristian already exists in database."));


                    mockUserRepository.Setup(repo => repo.GetByUserName(It.IsAny<string>())).Returns(new GenericResponseDTO<GetUserResponseDTO>()
                    {
                        ErrorCode = "000",
                        Message = "User name cristian",
                        Data = new GetUserResponseDTO()
                        {
                            UserId = 1,
                            UserName = "cristian",
                            Name = "cristian"
                        }
                    }); 
                }

                return mockUserRepository;

            }
        }

        public static Mock<IUserRepository> MockUserRepositoryWithExceptions
        {
            get
            {
                var mockRepo = new Mock<IUserRepository>();
                
                mockRepo.Setup(repo => repo.AuthenticateUser(It.IsAny<AuthenticateUserRequestDTO>())).Throws(new GenericRepositoryException("002", "Error authenticating user cristian."));
                mockRepo.Setup(repo => repo.RegisterUser(It.IsAny<RegisterUserRequestDTO>())).Throws(new GenericRepositoryException("001", "User cristian already exists in database."));
                mockRepo.Setup(repo => repo.GetByUserName(It.IsAny<string>())).Throws(new GenericRepositoryException("003", "User cristian was not found."));

                return mockRepo;

            }
        }
    }
}
