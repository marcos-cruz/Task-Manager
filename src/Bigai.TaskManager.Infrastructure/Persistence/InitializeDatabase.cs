using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bigai.TaskManager.Infrastructure.Persistence;

public static class InitializeDatabase
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}