using Dot.Net.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Dto.Bid;
using P7CreateRestApi.Repositories.Interfaces;
using System.Threading.Tasks;
using Xunit;

namespace P7CreateRestApi.Test.Controllers
{
    public class BidControllerTests
    {
        private readonly Mock<IBidRepository> _bidRepositoryMock;
        private readonly BidController _controller;

        public BidControllerTests()
        {
            _bidRepositoryMock = new Mock<IBidRepository>();

            _controller = new BidController(
                _bidRepositoryMock.Object,
                Mock.Of<ILogger<BidController>>());
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenBidExists()
        {
            // Arrange
            var bid = new Bid
            {
                Id = 1,
                Account = "ACC1",
                BidType = "Type",
                BidQuantity = 10
            };

            _bidRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(bid);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnedBid = Assert.IsType<BidDto>(okResult.Value);

            Assert.Equal(1, returnedBid.Id);
            Assert.Equal("ACC1", returnedBid.Account);
            Assert.Equal("Type", returnedBid.BidType);
            Assert.Equal(10, returnedBid.BidQuantity);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenBidDoesNotExist()
        {
            // Arrange
            _bidRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Bid?)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Account", "Required");

            var dto = new CreateBidDto
            {
                Account = "",       // invalide
                BidType = "Type",
                BidQuantity = 10
            };

            // Act
            var result = await _controller.Create(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
