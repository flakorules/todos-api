using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using todos.api.Abstractions.Repository;
using todos.api.Controllers;
using todos.api.DTO;
using Xunit;

namespace todos.api.tests.Controllers
{
    public class UserControllerTests
    {
        private readonly string fakeToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiIyIiwidW5pcXVlX25hbWUiOiJ0ZXJlc28iLCJuYmYiOjE2MzA3ODA3MjcsImV4cCI6MTYzMDc4NDMyNywiaWF0IjoxNjMwNzgwNzI3fQ.v7r20znyg7LCZHCcZShdmZtRmAqTb9Cz1rLSSWK73Uo";

        [Fact]
        public async Task Authentication_successful()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.AuthenticateUser(It.IsAny<AuthenticateUserRequestDTO>())).ReturnsAsync(fakeToken);            
            var controller = new UserController(mockRepo.Object);
            var request = new AuthenticateUserRequestDTO() { UserName = "cristian", Password = "123456" };
            var expected = new OkObjectResult(new { OK = true, token = fakeToken });

            // Act
            OkObjectResult result = (OkObjectResult)await controller.Authenticate(request);
            // Assert
            
            Assert.True(expected.Value.ToString().Equals(result.Value.ToString()));
            
        }

        [Fact]
        public async Task Authentication_Failed()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.AuthenticateUser(It.IsAny<AuthenticateUserRequestDTO>())).ReturnsAsync(string.Empty);
            var controller = new UserController(mockRepo.Object);
            var request = new AuthenticateUserRequestDTO() { UserName = "cristian", Password = "123456" };
            var expected = new BadRequestObjectResult(new { OK = false, message = "Error al autenticar el Usuario cristian." });

            // Act
            var result = (BadRequestObjectResult)await controller.Authenticate(request);
            // Assert
            Assert.True(expected.Value.ToString().Equals(result.Value.ToString()));

        }

        [Fact]
        public async Task Register_successful()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.RegisterUser(It.IsAny<RegisterUserRequestDTO>())).ReturnsAsync(true);
            var controller = new UserController(mockRepo.Object);
            var request = new RegisterUserRequestDTO() { UserName = "cristian", Password = "123456", Name="cristian" };
            var expected = new OkObjectResult(new { OK = true, message = "Usuario cristian creado correctamente." });

            // Act
            OkObjectResult result = (OkObjectResult)await controller.Register(request);
            // Assert
            Assert.True(expected.Value.ToString().Equals(result.Value.ToString()));

        }

        [Fact]
        public async Task Register_failed()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.RegisterUser(It.IsAny<RegisterUserRequestDTO>())).ReturnsAsync(false);
            var controller = new UserController(mockRepo.Object);
            var request = new RegisterUserRequestDTO() { UserName = "cristian", Password = "123456", Name = "cristian" };
            var expected = new BadRequestObjectResult(new { OK = false, message = "Error al crear el Usuario cristian." });
            // Act
            var result = (BadRequestObjectResult)await controller.Register(request);
            // Assert
            Assert.True(expected.Value.ToString().Equals(result.Value.ToString()));

        }
    }
}
