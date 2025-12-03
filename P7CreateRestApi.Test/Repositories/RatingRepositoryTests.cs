using Xunit;
using System.Linq;
using System.Threading.Tasks;
using P7CreateRestApi.Tests.Helpers;
using P7CreateRestApi.Repositories;
using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Tests.Repositories
{
    public class RatingRepositoryTests
    {
        private Rating CreateValidRating() =>
            new Rating
            {
                MoodysRating = "A",
                SandPRating = "AA",
                FitchRating = "AAA"
            };

        [Fact]
        public async Task AddAsync_ShouldAddRating()
        {
            var ctx = TestDbContextFactory.Create();
            var repo = new RatingRepository(ctx);

            var rating = CreateValidRating();
            var added = await repo.AddAsync(rating);

            Assert.Equal(1, added.Id);
            Assert.Single(ctx.Ratings);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllRatings()
        {
            var ctx = TestDbContextFactory.Create();
            var repo = new RatingRepository(ctx);

            ctx.Ratings.Add(CreateValidRating());
            ctx.Ratings.Add(CreateValidRating());
            ctx.SaveChanges();

            var list = await repo.GetAllAsync();
            Assert.Equal(2, list.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectRating()
        {
            var ctx = TestDbContextFactory.Create();
            var repo = new RatingRepository(ctx);

            var rating = CreateValidRating();
            ctx.Ratings.Add(rating);
            ctx.SaveChanges();

            var found = await repo.GetByIdAsync(rating.Id);
            Assert.NotNull(found);
            Assert.Equal(rating.Id, found.Id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyRating()
        {
            var ctx = TestDbContextFactory.Create();
            var repo = new RatingRepository(ctx);

            var rating = CreateValidRating();
            ctx.Ratings.Add(rating);
            ctx.SaveChanges();

            rating.MoodysRating = "B";
            await repo.UpdateAsync(rating);

            Assert.Equal("B", ctx.Ratings.First().MoodysRating);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveRating()
        {
            var ctx = TestDbContextFactory.Create();
            var repo = new RatingRepository(ctx);

            var rating = CreateValidRating();
            ctx.Ratings.Add(rating);
            ctx.SaveChanges();

            var result = await repo.DeleteAsync(rating.Id);

            Assert.True(result);
            Assert.Empty(ctx.Ratings);
        }
    }
}
