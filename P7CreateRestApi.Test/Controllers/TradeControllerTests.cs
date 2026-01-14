using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Dot.Net.WebApi.Controllers;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories.Interfaces;
using System.Threading.Tasks;

namespace P7CreateRestApi.Test.Controllers
{
    public class TradeControllerTests
    {
        private readonly Mock<ITradeRepository> _tradeRepositoryMock;
        private readonly TradeController _controller;

        public TradeControllerTests()
        {
            _tradeRepositoryMock = new Mock<ITradeRepository>();

            _controller = new TradeController(
                _tradeRepositoryMock.Object,
                Mock.Of<ILogger<TradeController>>());
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenTradeExists()
        {
            // Arrange
            var trade = new Trade { Id = 1, Account = "ACC1" };

            _tradeRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(trade);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTrade = Assert.IsType<Trade>(okResult.Value);
            Assert.Equal(1, returnedTrade.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenTradeDoesNotExist()
        {
            // Arrange
            _tradeRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Trade?)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenBuyAndSellQuantitiesAreNull()
        {
            // Arrange
            var trade = new Trade
            {
                BuyQuantity = null,
                SellQuantity = null
            };

            // Act
            var result = await _controller.Create(trade);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

            // (Optionnel mais propre) : s’assurer qu’on n’a pas tenté d’écrire en base
            _tradeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Trade>()), Times.Never);
        }
    }
}
