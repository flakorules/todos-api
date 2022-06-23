using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using todos.api.Abstractions.Repository;
using todos.api.Controllers;
using todos.api.DTO;
using todos.api.Exceptions.Repository;
using todos.api.tests.Helpers;
using Xunit;

namespace todos.api.tests.Controllers
{
    public class UserControllerTests
    {
        private readonly string fakeToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiIyIiwidW5pcXVlX25hbWUiOiJ0ZXJlc28iLCJuYmYiOjE2MzA3ODA3MjcsImV4cCI6MTYzMDc4NDMyNywiaWF0IjoxNjMwNzgwNzI3fQ.v7r20znyg7LCZHCcZShdmZtRmAqTb9Cz1rLSSWK73Uo";

        private readonly Mock<IUserRepository> mockRepo;
        private readonly Mock<IUserRepository> mockRepoWithExceptions;

        public UserControllerTests()
        {
            this.mockRepo = MockHelper.MockUserRepository;
            this.mockRepoWithExceptions = MockHelper.MockUserRepositoryWithExceptions;
        }

        [Fact(DisplayName = "Authentication successful")]
        
        public void Authentication_successful()
        {
            // Arrange
            var controller = new UserController(mockRepo.Object);
            var request = new AuthenticateUserRequestDTO() { UserName = "cristian", Password = "123456" };
            var expected = new GenericResponseDTO<AuthenticationResponseDTO>()
            {
                ErrorCode = "000",
                Message = "User cristian was authenticated correctly.",
                Data = new AuthenticationResponseDTO()
                {
                    UserId = 1,
                    UserName = "cristian",
                    Token = fakeToken
                }
            };

            // Act
            var result = (OkObjectResult)controller.Authenticate(request);
            // Assert

            Assert.True(expected.Message.Equals(((GenericResponseDTO<AuthenticationResponseDTO>)result.Value).Message));

        }

        [Fact(DisplayName = "Authentication Failed")]
        public void Authentication_Failed()
        {
            // Arrange
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

        [Fact(DisplayName = "Register successful")]
        public async Task Register_successful()
        {
            // Arrange
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

        [Fact(DisplayName = "Register failed")]
        public async Task Register_failed()
        {
            // Arrange
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
            var controller = new UserController(mockRepoWithExceptions.Object);
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
