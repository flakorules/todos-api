using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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

            mockRepo.Setup(repo => repo.GetByUserId(It.IsAny<string>())).ReturnsAsync(
                new GenericResponseDTO<IEnumerable<Todo>>()
                {
                    ErrorCode = "000",
                    Message = "Todos from userId 1",
                    Data = new List<Todo>()
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
                    }
                }
                );

            
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            //Act
            var result = (OkObjectResult)await controller.GetByUserId();
            //Assert
            Assert.True(((GenericResponseDTO<IEnumerable<Todo>>)result.Value).Data.ToList().Count > 0);
        }

       

        [Fact]
        public async Task Create()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Create(It.IsAny<CreateTodoRequestDTO>(), It.IsAny<string>())).ReturnsAsync(                
                new GenericResponseDTO<Todo>()
                {
                    ErrorCode = "000",
                    Message = "Todo was created succesfully",
                    Data = new Todo()
                    {
                        TodoId = 4,
                        UserId = 1,
                        Name = "Pasear al perro",
                        Solved = false,
                        Description = "Pasear al perro",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    }
                }
                
                );
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var request = new CreateTodoRequestDTO()
            {
                Name = "Pasear al perro",
                Description = "Pasear al perro"
            };
            var expected = new GenericResponseDTO<Todo>()
            {
                ErrorCode = "000",
                Message = "Todo was created succesfully",
                Data = new Todo()
                {
                    TodoId = 4,
                    UserId = 1,
                    Name = "Pasear al perro",
                    Solved = false,
                    Description = "Pasear al perro",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                }
            };
            //Act
            var result = (CreatedAtActionResult)await controller.Create(request);
            //Assert
            Assert.True(expected.Data.TodoId.Equals(((GenericResponseDTO<Todo>)result.Value).Data.TodoId));

        }

        [Fact]
        public async Task Delete_Successfull()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Delete(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(
                new GenericResponseDTO<bool>()
                {
                    ErrorCode = "000",
                    Message = "Todo 4 was deleted successfully",
                    Data = true
                });
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var todoId = 4;
            var expected = new GenericResponseDTO<bool>()
            {
                ErrorCode = "000",
                Message = "Todo 4 was deleted successfully",
                Data = true
            };
            //Act
            var result = (OkObjectResult)await controller.Delete(todoId);
            //Assert
            Assert.True(expected.Message.Equals(((GenericResponseDTO<bool>)result.Value).Message));
        }

        [Fact]
        public async Task Delete_Failed()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Delete(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(
                
                new GenericResponseDTO<bool>()
                {
                    ErrorCode = "006",
                    Message = "Todo 4 was not deleted",
                    Data = false
                }
                
                );
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var todoId = 4;
            var expected = new GenericResponseDTO<bool>()
            {
                ErrorCode = "006",
                Message = "Todo 4 was not deleted",
                Data = false
            };
            //Act
            var result = (BadRequestObjectResult)await controller.Delete(todoId);
            //Assert
            Assert.True(expected.Message.Equals(((GenericResponseDTO<bool>)result.Value).Message));
        }

        [Fact]
        public async Task Delete_Unauthorized()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Delete(It.IsAny<int>(), It.IsAny<string>())).Throws(new GenericRepositoryException("005", "Not authorized to delete other's todos."));
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var todoId = 4;
            var expected = new GenericResponseDTO<bool>()
            {
                ErrorCode = "005",
                Message = "Not authorized to delete other's todos.",
                Data = false
            };
            //Act
            var result = (BadRequestObjectResult)await controller.Delete(todoId);
            //Assert
            Assert.True(expected.Message.Equals(((GenericResponseDTO<bool>)result.Value).Message));
        }

        [Fact]
        public async Task Solve_Successful()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Solve(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(
                new GenericResponseDTO<bool>()
                {
                    ErrorCode = "000",
                    Message = "Todo 4 was solved succesfully.",
                    Data = true
                }
                );
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var todoId = 4;
            var expected = new GenericResponseDTO<bool>()
            {
                ErrorCode = "000",
                Message = "Todo 4 was solved succesfully.",
                Data = true
            };
            //Act
            var result = (OkObjectResult)await controller.Solve(todoId);
            //Assert
            Assert.True(expected.Message.Equals(((GenericResponseDTO<bool>)result.Value).Message));
        }
        [Fact]
        public async Task Solve_Failed()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Solve(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(
                new GenericResponseDTO<bool>()
                {
                    ErrorCode = "006",
                    Message = "Todo 4 was not solved.",
                    Data = false
                }
                );
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var todoId = 4;
            var expected = new GenericResponseDTO<bool>()
            {
                ErrorCode = "006",
                Message = "Todo 4 was not solved.",
                Data = false
            };
            //Act
            var result = (BadRequestObjectResult)await controller.Solve(todoId);
            //Assert
            Assert.True(expected.Message.Equals(((GenericResponseDTO<bool>)result.Value).Message));
        }

        [Fact]
        public async Task Solve_Unauthorized()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Solve(It.IsAny<int>(), It.IsAny<string>())).Throws(new GenericRepositoryException("005", "Not authorized to Solve other's todos."));
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var todoId = 4;
            var expected = new GenericResponseDTO<IEnumerable<bool>>() {
                ErrorCode= "005",
                Message = "Not authorized to Solve other's todos.",
                Data = null
            };
            //Act
            var result = (BadRequestObjectResult)await controller.Solve(todoId);
            //Assert
            Assert.True(expected.Message.Equals(((GenericResponseDTO<IEnumerable<bool>>)result.Value).Message));
        }

        [Fact]
        public async Task Update_Successful()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Update(It.IsAny<int>(), It.IsAny<UpdateTodoRequestDTO>(), It.IsAny<string>())).ReturnsAsync(
                new GenericResponseDTO<Todo>()
                {
                    ErrorCode = "000",
                    Message = "Todo 4 was updated succesfully.",
                    Data = new Todo()
                    {
                        TodoId = 4,
                        UserId = 1,
                        Name = "Pasear al perro 2",
                        Solved = false,
                        Description = "Pasear al perro 2",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    }
                }
                );

            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var todoId = 4;
            var expected = new GenericResponseDTO<Todo>()
            {
                ErrorCode = "000",
                Message = "Todo 4 was updated succesfully.",
                Data = new Todo()
                {
                    TodoId = 4,
                    UserId = 1,
                    Name = "Pasear al perro 2",
                    Solved = false,
                    Description = "Pasear al perro 2",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                }
            };
            //Act
            var result = (OkObjectResult)await controller.Update(todoId, new UpdateTodoRequestDTO() { Name = "Pasear al perro 2", Description = "Pasear al perro 2" });
            //Assert
            Assert.True(expected.Message.Equals(((GenericResponseDTO<Todo>)result.Value).Message));
        }

        [Fact]
        public async Task Update_Failed()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Update(It.IsAny<int>(), It.IsAny<UpdateTodoRequestDTO>() ,It.IsAny<string>())).ReturnsAsync(
                new GenericResponseDTO<Todo>()
                {
                    ErrorCode = "006",
                    Message = "Todo 4 was not updated.",
                    Data = new Todo()
                    {
                        TodoId = 4,
                        UserId = 1,
                        Name = "Pasear al perro",
                        Solved = false,
                        Description = "Pasear al perro",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    }
                }
                );
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var todoId = 4;
            var expected = new GenericResponseDTO<Todo>()
            {
                ErrorCode = "006",
                Message = "Todo 4 was not updated.",
                Data = new Todo()
                {
                    TodoId = 4,
                    UserId = 1,
                    Name = "Pasear al perro",
                    Solved = false,
                    Description = "Pasear al perro",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                }
            };
            //Act
            var result = (OkObjectResult)await controller.Update(todoId, new UpdateTodoRequestDTO() { Name = "Pasear al perro 2", Description = "Pasear al perro 2" });
            //Assert
            Assert.True(expected.Message.Equals(((GenericResponseDTO<Todo>)result.Value).Message));
        }

        [Fact]
        public async Task Update_Unauthorized()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockHelper = new Mock<IBearerTokenHelper>();
            mockRepo.Setup(repo => repo.Solve(It.IsAny<int>(), It.IsAny<string>())).Throws(new GenericRepositoryException("005", "Not authorized to Update other's todos."));
            mockHelper.Setup(helper => helper.GetBearerToken(It.IsAny<HttpRequest>())).Returns(fakeToken);
            var controller = new TodoController(mockRepo.Object, mockHelper.Object);
            var todoId = 4;
            var expected = new GenericResponseDTO<IEnumerable<bool>>()
            {
                ErrorCode = "005",
                Message = "Not authorized to Update other's todos.",
                Data = null
            };
            //Act
            var result = (BadRequestObjectResult)await controller.Solve(todoId);
            //Assert
            Assert.True(expected.Message.Equals(((GenericResponseDTO<IEnumerable<bool>>)result.Value).Message));
        }
    }
}