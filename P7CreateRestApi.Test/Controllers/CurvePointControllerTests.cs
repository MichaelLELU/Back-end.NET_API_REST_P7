using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Dot.Net.WebApi.Controllers;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories.Interfaces;
using P7CreateRestApi.Dto.CurvePoint;
using System.Threading.Tasks;

namespace P7CreateRestApi.Tests.Controllers
{
    public class CurveControllerTests
    {
        private readonly Mock<ICurvePointRepository> _curveRepositoryMock;
        private readonly CurveController _controller;

        public CurveControllerTests()
        {
            _curveRepositoryMock = new Mock<ICurvePointRepository>();

            _controller = new CurveController(
                _curveRepositoryMock.Object,
                Mock.Of<ILogger<CurveController>>());
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenCurveExists()
        {
            // Arrange
            var curve = new CurvePoint { Id = 1, CurveId = 1, Term = 10, CurvePointValue = 100 };

            _curveRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(curve);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCurve = Assert.IsType<CurvePoint>(okResult.Value);
            Assert.Equal(1, returnedCurve.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenCurveDoesNotExist()
        {
            // Arrange
            _curveRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((CurvePoint?)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Term", "Required");

            var dto = new CreateCurvePointDto
            {
                CurveId = 1,
                Term = 0,
                CurvePointValue = 100
            };

            // Act
            var result = await _controller.Create(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenCurveIsValid()
        {
            // Arrange
            var dto = new CreateCurvePointDto
            {
                CurveId = 1,
                Term = 10,
                CurvePointValue = 100
            };

            _curveRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<CurvePoint>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            var returnedCurve = Assert.IsType<CurvePoint>(createdResult.Value);
            Assert.Equal(10, returnedCurve.Term);
            Assert.Equal(100, returnedCurve.CurvePointValue);

            _curveRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<CurvePoint>()),
                Times.Once);
        }

    }
}
