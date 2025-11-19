using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;

namespace P7CreateRestApi.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static LocalDbContext Create()
        {
            var options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new LocalDbContext(options);
        }
    }
}
