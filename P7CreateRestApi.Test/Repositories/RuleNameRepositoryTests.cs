using Xunit;
using System.Linq;
using System.Threading.Tasks;
using P7CreateRestApi.Tests.Helpers;
using P7CreateRestApi.Repositories;
using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Tests.Repositories
{
    public class RuleNameRepositoryTests
    {
        private RuleName CreateValidRule() =>
            new RuleName
            {
                Name = "Rule1",
                Description = "Desc",
                Json = "{}",
                Template = "T",
                SqlStr = "SELECT 1",
                SqlPart = "Part"
            };

        [Fact]
        public async Task AddAsync_ShouldAddRule()
        {
            var ctx = TestDbContextFactory.Create();
            var repo = new RuleNameRepository(ctx);

            var rule = CreateValidRule();
            var added = await repo.AddAsync(rule);

            Assert.Equal(1, added.Id);
            Assert.Single(ctx.RuleNames);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllRules()
        {
            var ctx = TestDbContextFactory.Create();
            var repo = new RuleNameRepository(ctx);

            ctx.RuleNames.Add(CreateValidRule());
            ctx.RuleNames.Add(CreateValidRule());
            ctx.SaveChanges();

            var list = await repo.GetAllAsync();
            Assert.Equal(2, list.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectRule()
        {
            var ctx = TestDbContextFactory.Create();
            var repo = new RuleNameRepository(ctx);

            var rule = CreateValidRule();
            ctx.RuleNames.Add(rule);
            ctx.SaveChanges();

            var found = await repo.GetByIdAsync(rule.Id);
            Assert.NotNull(found);
            Assert.Equal(rule.Id, found.Id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyRule()
        {
            var ctx = TestDbContextFactory.Create();
            var repo = new RuleNameRepository(ctx);

            var rule = CreateValidRule();
            ctx.RuleNames.Add(rule);
            ctx.SaveChanges();

            rule.Description = "Updated";
            await repo.UpdateAsync(rule);

            Assert.Equal("Updated", ctx.RuleNames.First().Description);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveRule()
        {
            var ctx = TestDbContextFactory.Create();
            var repo = new RuleNameRepository(ctx);

            var rule = CreateValidRule();
            ctx.RuleNames.Add(rule);
            ctx.SaveChanges();

            var result = await repo.DeleteAsync(rule.Id);

            Assert.True(result);
            Assert.Empty(ctx.RuleNames);
        }
    }
}
