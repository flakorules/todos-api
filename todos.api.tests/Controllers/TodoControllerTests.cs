using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using todos.api.Abstractions.Repository;
using todos.api.Entities;
using todos.api.Controllers;
using Xunit;
using todos.api.Abstractions.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using todos.api.DTO;
using todos.api.Exceptions.Repository;

namespace todos.api.tests.Controllers
{
    public class TodoControllerTests
    {
        private readonly string fakeToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiIyIiwidW5pcXVlX25hbWUiOiJ0ZXJlc28iLCJuYmYiOjE2MzA3ODA3MjcsImV4cCI6MTYzMDc4NDMyNywiaWF0IjoxNjMwNzgwNzI3fQ.v7r20znyg7LCZHCcZShdmZtRmAqTb9Cz1rLSSWK73Uo";

        [Fact]
        public async Task GetByUserId()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.GetByUserId(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new List<Todo>()
            {
                new Todo(){
                    TodoId = 1,
                    UserId = 1,
                    Name = "Ir al supermercado",
                    Solved = false,
                    Description = "Ir al supermercado",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                },
                new Todo(){
                    TodoId = 2,
                    UserId = 1,
                    Name = "Lavar la ropa",
                    Solved = false,
                    Description = "Lavar la ropa",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                },
                new Todo(){
                    TodoId = 3,
                    UserId = 1,
                    Name = "Reparar el auto",
                    Solved = false,
                    Description = "Lavar la ropa",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                },
            });
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var userId = 1;
            //Act
            var result = (OkObjectResult)await controller.GetByUserId(userId);
            //Assert
            Assert.True(((IEnumerable<Todo>)result.Value).ToList().Count > 0);
        }

        [Fact]
        public async Task GetByUserId_Unauthorized()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.GetByUserId(It.IsAny<int>(), It.IsAny<string>())).Throws(new UnauthorizedException("No está autorizado a ver los Todos de otro usuario."));
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var expected = new BadRequestObjectResult(new { OK = false, message = "No está autorizado a ver los Todos de otro usuario." });
            var userId = 1;
            //Act
            var result = (BadRequestObjectResult)await controller.GetByUserId(userId);
            //Assert
            Assert.True(expected.Value.ToString().Equals(result.Value.ToString()));
        }

        [Fact]
        public async Task Create()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Create(It.IsAny<CreateTodoRequestDTO>(), It.IsAny<string>())).ReturnsAsync(new Todo()
            {
                TodoId = 4,
                UserId = 1,
                Name = "Pasear al perro",
                Solved = false,
                Description = "Pasear al perro",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            });
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var request = new CreateTodoRequestDTO()
            {
                Name = "Pasear al perro",
                Description = "Pasear al perro"
            };
            var expected = 4;
            //Act
            var result = (OkObjectResult)await controller.Create(request);
            //Assert
            Assert.Equal(expected, ((Todo)result.Value).TodoId);

        }

        [Fact]
        public async Task Delete_Successfull()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Delete(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var todoId = 4;
            var expected = new OkObjectResult(new { OK = true, message = "El todo se ha eliminado correctamente" });
            //Act
            var result = (OkObjectResult)await controller.Delete(todoId);
            //Assert
            Assert.True(expected.Value.ToString().Equals(result.Value.ToString()));
        }

        [Fact]
        public async Task Delete_Failed()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Delete(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(false);
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var todoId = 4;
            var expected = new BadRequestObjectResult(new { OK = true, message = "Error al eliminar el todo" });
            //Act
            var result = (BadRequestObjectResult)await controller.Delete(todoId);
            //Assert
            Assert.True(expected.Value.ToString().Equals(result.Value.ToString()));
        }

        [Fact]
        public async Task Delete_Unauthorized()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Delete(It.IsAny<int>(), It.IsAny<string>())).Throws(new UnauthorizedException("No está autorizado a eliminar los Todos de otro usuario."));
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var todoId = 4;
            var expected = new BadRequestObjectResult(new { OK = false, message = "No está autorizado a eliminar los Todos de otro usuario." });
            //Act
            var result = (BadRequestObjectResult)await controller.Delete(todoId);
            //Assert
            Assert.True(expected.Value.ToString().Equals(result.Value.ToString()));
        }

        [Fact]
        public async Task Solve_Successful()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Solve(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var todoId = 4;
            var expected = new OkObjectResult(new { OK = true, message = "El todo se ha resuelto correctamente" });
            //Act
            var result = (OkObjectResult)await controller.Solve(todoId);
            //Assert
            Assert.True(expected.Value.ToString().Equals(result.Value.ToString()));
        }
        [Fact]
        public async Task Solve_Failed()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Solve(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(false);
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var todoId = 4;
            var expected = new BadRequestObjectResult(new { OK = false, message = "Error al resolver el todo" });
            //Act
            var result = (BadRequestObjectResult)await controller.Solve(todoId);
            //Assert
            Assert.True(expected.Value.ToString().Equals(result.Value.ToString()));
        }

        [Fact]
        public async Task Solve_Unauthorized()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Solve(It.IsAny<int>(), It.IsAny<string>())).Throws(new UnauthorizedException("No está autorizado a resolver los Todos de otro usuario."));
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var todoId = 4;
            var expected = new BadRequestObjectResult(new { OK = false, message = "No está autorizado a resolver los Todos de otro usuario." });
            //Act
            var result = (BadRequestObjectResult)await controller.Solve(todoId);
            //Assert
            Assert.True(expected.Value.ToString().Equals(result.Value.ToString()));
        }
    }
}