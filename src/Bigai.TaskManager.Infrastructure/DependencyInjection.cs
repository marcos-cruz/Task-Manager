using Bigai.TaskManager.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bigai.TaskManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //
        // TODO: lançar uma exception se a connectionstring não estiver definida.
        //
        var connectionString = configuration.GetConnectionString("TaskManagerContext");

        services.AddDbContext<TaskManagerDbContext>(options =>
                    options.UseSqlServer(connectionString)
                           .EnableSensitiveDataLogging());

        return services;
    }

}
