using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;
using Bigai.TaskManager.Infrastructure.Persistence;
using Bigai.TaskManager.Infrastructure.Projects.Repositories;
using Bigai.TaskManager.Infrastructure.Projects.Services;

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

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectAuthorizationService, ProjectAuthorizationService>();
        services.AddScoped<IBussinessNotificationsHandler, BussinessNotificationsHandler>();

        return services;
    }
}
