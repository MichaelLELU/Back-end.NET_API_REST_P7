using P7CreateRestApi.Tests.Helpers;
using P7CreateRestApi.Repositories;
using P7CreateRestApi.Domain;
using Xunit;

namespace P7CreateRestApi.Tests.Repositories
{
    public class BidRepositoryTests
    {
        private Bid CreateValidBid() =>
            new Bid
            {
                Account = "ACC1",
                BidType = "Type",
                Benchmark = "Bench",
                Commentary = "Comment",
                BidSecurity = "SEC",
                BidStatus = "OK",
                Book = "BOOK",
                CreationName = "Creator",
                RevisionName = "Rev",
                DealName = "Deal",
                SourceListId = "SLI"
            };

        [Fact]
        public async Task AddAsync_ShouldAddBid()
        {
            var context = TestDbContextFactory.Create();
            var repo = new BidRepository(context);

            var bid = CreateValidBid();
            var added = await repo.AddAsync(bid);

            Assert.Equal(1, added.Id);
            Assert.Single(context.Bids);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllBids()
        {
            var context = TestDbContextFactory.Create();
            var repo = new BidRepository(context);

            context.Bids.Add(CreateValidBid());
            context.Bids.Add(CreateValidBid());
            context.SaveChanges();

            var list = await repo.GetAllAsync();

            Assert.Equal(2, list.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnBid()
        {
            var context = TestDbContextFactory.Create();
            var repo = new BidRepository(context);

            var bid = CreateValidBid();
            context.Bids.Add(bid);
            context.SaveChanges();

            var found = await repo.GetByIdAsync(bid.Id);

            Assert.NotNull(found);
            Assert.Equal(bid.Id, found.Id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyBid()
        {
            var context = TestDbContextFactory.Create();
            var repo = new BidRepository(context);

            var bid = CreateValidBid();
            context.Bids.Add(bid);
            context.SaveChanges();

            bid.Commentary = "Updated";
            await repo.UpdateAsync(bid);

            var updated = context.Bids.First();

            Assert.Equal("Updated", updated.Commentary);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveBid()
        {
            var context = TestDbContextFactory.Create();
            var repo = new BidRepository(context);

            var bid = CreateValidBid();
            context.Bids.Add(bid);
            context.SaveChanges();

            var result = await repo.DeleteAsync(bid.Id);

            Assert.True(result);
            Assert.Empty(context.Bids);
        }
    }
}
