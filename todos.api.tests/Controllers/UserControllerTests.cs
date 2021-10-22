using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using todos.api.Abstractions.Repository;
using todos.api.Controllers;
using todos.api.DTO;
using todos.api.Exceptions.Repository;
using Xunit;

namespace todos.api.tests.Controllers
{
    public class UserControllerTests
    {
        private readonly string fakeToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiIyIiwidW5pcXVlX25hbWUiOiJ0ZXJlc28iLCJuYmYiOjE2MzA3ODA3MjcsImV4cCI6MTYzMDc4NDMyNywiaWF0IjoxNjMwNzgwNzI3fQ.v7r20znyg7LCZHCcZShdmZtRmAqTb9Cz1rLSSWK73Uo";

        [Fact]
        public void Authentication_successful()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.AuthenticateUser(It.IsAny<AuthenticateUserRequestDTO>())).Returns(new GenericResponseDTO<string>()
            {
                ErrorCode = "000",
                Message = "User cristian was authenticated correctly.",
                Data = fakeToken
            }
            );            
            var controller = new UserController(mockRepo.Object);
            var request = new AuthenticateUserRequestDTO() { UserName = "cristian", Password = "123456" };
            var expected = new GenericResponseDTO<string>()
            {
                ErrorCode = "000",
                Message = "User cristian was authenticated correctly.",
                Data = fakeToken
            };

            // Act
            var result = (OkObjectResult)controller.Authenticate(request);
            // Assert

            Assert.True(expected.Message.Equals(((GenericResponseDTO<string>)result.Value).Message));

        }

        [Fact]
        public void Authentication_Failed()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.AuthenticateUser(It.IsAny<AuthenticateUserRequestDTO>())).Throws(new GenericRepositoryException("002", "Error authenticating user cristian."));
            var controller = new UserController(mockRepo.Object);
            var request = new AuthenticateUserRequestDTO() { UserName = "cristian", Password = "123456" };
            var expected = new GenericResponseDTO<string>()
            {
                ErrorCode = "002",
                Message = "Error authenticating user cristian.",
                Data = null
            };

            // Act
            var result = (BadRequestObjectResult)controller.Authenticate(request);
            // Assert
            Assert.True(expected.Message.Equals(((GenericResponseDTO<string>)result.Value).Message));

        }

        [Fact]
        public async Task Register_successful()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.RegisterUser(It.IsAny<RegisterUserRequestDTO>())).ReturnsAsync(
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
                );
            var controller = new UserController(mockRepo.Object);
            var request = new RegisterUserRequestDTO() { UserName = "cristian", Password = "123456", Name = "cristian" };
            var expected = new GenericResponseDTO<RegisterUserResponseDTO>()
            {
                ErrorCode = "000",
                Message = "User cristian was created.",
                Data = new RegisterUserResponseDTO()
                {
                    UserId = 1,
                    UserName = "cristian",
                    Name = "cristian"
                }
            };

            // Act
            var result = (CreatedAtActionResult)await controller.Register(request);

            // Assert
            Assert.True(expected.Message.Equals(((GenericResponseDTO<RegisterUserResponseDTO>)result.Value).Message));

        }

        [Fact]
        public async Task Register_failed()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.RegisterUser(It.IsAny<RegisterUserRequestDTO>())).Throws(new GenericRepositoryException("001", "User cristian already exists in database."));
            var controller = new UserController(mockRepo.Object);
            var request = new RegisterUserRequestDTO() { UserName = "cristian", Password = "123456", Name = "cristian" };
            var expected = new GenericResponseDTO<RegisterUserResponseDTO>()
            {
                ErrorCode = "001",
                Message = "User cristian already exists in database.",
                Data = null
            };
            // Act
            var result = (BadRequestObjectResult)await controller.Register(request);
            // Assert
            Assert.True(expected.Message.Equals(((GenericResponseDTO<RegisterUserResponseDTO>)result.Value).Message));
        }

        [Fact]
        public void GetByUserName_Successful()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.GetByUserName(It.IsAny<string>())).Returns(new GenericResponseDTO<GetUserResponseDTO>()
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
        
            var controller = new UserController(mockRepo.Object);
            string userName = "cristian";

            var expected = new GenericResponseDTO<GetUserResponseDTO>() {
                ErrorCode = "000",
                Message = "User name cristian",
                Data = new GetUserResponseDTO()
                {
                    UserId = 1,
                    UserName = "cristian",
                    Name = "cristian"
                }
            };
            // Act
            var result = (OkObjectResult)controller.GetByUserName(userName);
            // Assert
            Assert.True(expected.Message.Equals(((GenericResponseDTO<GetUserResponseDTO>)result.Value).Message));

        }

        [Fact]
        public void GetByUserName_Failed()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.GetByUserName(It.IsAny<string>())).Throws(new GenericRepositoryException("003", "User cristian was not found."));
            var controller = new UserController(mockRepo.Object);
            string userName = "cristian";
            var expected = new GenericResponseDTO<RegisterUserResponseDTO>()
            {
                ErrorCode = "003",
                Message = "User cristian was not found.",
                Data = null
            };
            // Act
            var result = (NotFoundObjectResult)controller.GetByUserName(userName);
            // Assert
            Assert.True(expected.Message.Equals(((GenericResponseDTO<GetUserResponseDTO>)result.Value).Message));

        }
    }
}
