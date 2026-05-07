using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;

namespace TaskManager.Tests.Helpers;

public static class DbContextFactory
{
    public static AppDbContext Create(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new AppDbContext(options);
    }
}
