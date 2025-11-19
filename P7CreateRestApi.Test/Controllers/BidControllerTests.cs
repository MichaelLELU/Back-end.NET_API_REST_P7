using Dot.Net.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Tests.Helpers;


namespace P7CreateRestApi.Tests.Controllers
{
    public class BidControllerTests
    {
        private BidController GetController(LocalDbContext context)
        {
            var logger = new LoggerFactory().CreateLogger<BidController>();
            var controller = new BidController(context, logger);

            return TestControllerFactory.CreateController(controller);
        }


        private Bid CreateValidBid(int? id = null)
        {
            return new Bid
            {
                Id = id ?? 0,
                Account = "ACC1",
                BidType = "TYPE",
                Benchmark = "BMK",
                Commentary = "Test comment",
                BidSecurity = "SEC",
                BidStatus = "ACTIVE",
                Book = "BOOK",
                CreationName = "Creator",
                RevisionName = "Rev1",
                DealName = "Deal1",
                SourceListId = "SRC1"
            };
        }

        [Fact]
        public async Task GetAll_ReturnsEmpty_WhenNoBids()
        {
            var context = TestDbContextFactory.Create();
            var controller = GetController(context);

            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var bids = Assert.IsAssignableFrom<IEnumerable<Bid>>(ok.Value);

            Assert.Empty(bids);
        }

        [Fact]
        public async Task Create_AddsBidSuccessfully()
        {
            var context = TestDbContextFactory.Create();
            var controller = GetController(context);
            var bid = CreateValidBid();

            var result = await controller.Create(bid);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var saved = Assert.IsType<Bid>(created.Value);

            Assert.NotEqual(0, saved.Id);
            Assert.Single(context.Bids);
        }

        [Fact]
        public async Task GetById_ReturnsBidAfterCreation()
        {
            var context = TestDbContextFactory.Create();
            var controller = GetController(context);

            var bid = CreateValidBid();
            await controller.Create(bid);

            var result = await controller.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var found = Assert.IsType<Bid>(ok.Value);

            Assert.Equal(1, found.Id);
        }

        [Fact]
        public async Task Update_ModifiesBidSuccessfully()
        {
            var context = TestDbContextFactory.Create();
            var controller = GetController(context);

            var bid = CreateValidBid();
            await controller.Create(bid);

            var updatedBid = CreateValidBid(1);
            updatedBid.Commentary = "Updated!";

            var result = await controller.Update(1, updatedBid);

            var ok = Assert.IsType<OkObjectResult>(result);
            var updated = Assert.IsType<Bid>(ok.Value);

            Assert.Equal("Updated!", updated.Commentary);
        }

        [Fact]
        public async Task Delete_RemovesBidSuccessfully()
        {
            var context = TestDbContextFactory.Create();
            var controller = GetController(context);

            var bid = CreateValidBid();
            await controller.Create(bid);

            var result = await controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Bids);
        }
    }
}
