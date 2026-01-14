using P7CreateRestApi.Tests.Helpers;
using P7CreateRestApi.Repositories;
using P7CreateRestApi.Domain;
using Xunit;
using System.Linq;
using System.Threading.Tasks;

namespace P7CreateRestApi.Tests.Repositories
{
    public class CurvePointRepositoryTests
    {
        private CurvePoint CreateValidCurvePoint() =>
            new CurvePoint
            {
                CurveId = 1,
                Term = 10,
                CurvePointValue = 100
            };

        [Fact]
        public async Task AddAsync_ShouldAddCurvePoint()
        {
            var context = TestDbContextFactory.Create();
            var repo = new CurvePointRepository(context);

            var curve = CreateValidCurvePoint();
            await repo.AddAsync(curve);

            Assert.Equal(1, curve.Id);
            Assert.Single(context.CurvePoints);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCurvePoints()
        {
            var context = TestDbContextFactory.Create();
            var repo = new CurvePointRepository(context);

            context.CurvePoints.Add(CreateValidCurvePoint());
            context.CurvePoints.Add(CreateValidCurvePoint());
            context.SaveChanges();

            var list = await repo.GetAllAsync();

            Assert.Equal(2, list.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCurvePoint()
        {
            var context = TestDbContextFactory.Create();
            var repo = new CurvePointRepository(context);

            var curve = CreateValidCurvePoint();
            context.CurvePoints.Add(curve);
            context.SaveChanges();

            var found = await repo.GetByIdAsync(curve.Id);

            Assert.NotNull(found);
            Assert.Equal(curve.Id, found.Id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyCurvePoint()
        {
            var context = TestDbContextFactory.Create();
            var repo = new CurvePointRepository(context);

            var curve = CreateValidCurvePoint();
            context.CurvePoints.Add(curve);
            context.SaveChanges();

            curve.Term = 20;
            await repo.UpdateAsync(curve);

            var updated = context.CurvePoints.First();
            Assert.Equal(20, updated.Term);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveCurvePoint()
        {
            var context = TestDbContextFactory.Create();
            var repo = new CurvePointRepository(context);

            var curve = CreateValidCurvePoint();
            context.CurvePoints.Add(curve);
            context.SaveChanges();

            var result = await repo.DeleteAsync(curve.Id);

            Assert.True(result);
            Assert.Empty(context.CurvePoints);
        }
    }
}
