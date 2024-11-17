using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;
using Bigai.TaskManager.Infrastructure.Persistence;
using Bigai.TaskManager.Infrastructure.Persistence.Settings;
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
        DbSettings sqlSettings = new DbSettings()
        {
            Host = configuration[DbSettings.DbServer] ?? DbSettings.DefaultDbServer,
            Port = configuration[DbSettings.DbPort] ?? DbSettings.DefaultDbPort,
            User = configuration[DbSettings.DbUser] ?? DbSettings.DefaultDbUser,
            Password = configuration[DbSettings.DbPassword] ?? DbSettings.DefaultDbPassword,
            Database = configuration[DbSettings.DbName] ?? DbSettings.DefaultDbName,
        };

        var connectionString = sqlSettings.ConnectionString;

        services.AddDbContext<TaskManagerDbContext>(options =>
                        options.UseSqlServer(connectionString)
                               .EnableSensitiveDataLogging());

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectAuthorizationService, ProjectAuthorizationService>();
        services.AddScoped<IBussinessNotificationsHandler, BussinessNotificationsHandler>();
        services.AddScoped<ISerializeService, SerializeService>();

        return services;
    }
}
