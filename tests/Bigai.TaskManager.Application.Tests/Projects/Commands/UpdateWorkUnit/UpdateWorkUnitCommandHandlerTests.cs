using System.Net;

using Bigai.TaskManager.Application.Projects.Commands.CreateWorkUnit;
using Bigai.TaskManager.Application.Projects.Commands.UpdateWorkUnit;
using Bigai.TaskManager.Domain.Projects.Constants;
using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Infrastructure.Persistence;
using Bigai.TaskManager.Infrastructure.Projects.Repositories;
using Bigai.TaskManager.Infrastructure.Projects.Services;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

namespace Bigai.TaskManager.Application.Tests.Projects.Commands.UpdateWorkUnit;

public class UpdateWorkUnitCommandHandlerTests
{
    private static async Task<TaskManagerDbContext> GetInMemoryDbContextAsync(int numberOfProjects, int userId, int numberOfTasks)
    {
        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new TaskManagerDbContext(options);

        if (numberOfProjects > 0)
        {
            var projects = new List<Project>();
            Random rnd = new();

            for (int i = 0; i < numberOfProjects; i++)
            {
                var project = Project.Create($"Project name {Guid.NewGuid()}");

                for (int j = 0; j < numberOfTasks; j++)
                {
                    var dueDate = DateTime.Now.AddDays(rnd.Next(15, 45));
                    var priority = (Priority)rnd.Next(0, 2);
                    var workUnit = WorkUnit.Create("Title of task", "Description of task", dueDate, priority);

                    workUnit.AssignToUser(userId);
                    project.AddWorkUnit(workUnit);
                }

                projects.Add(project);
            }

            context.Projects.AddRange(projects);

            await context.SaveChangesAsync();
        }

        return context;
    }

    [Fact()]
    public async Task Handle_ForValidCommand_ReturnsWorkUnitId()
    {
        // arrange
        int numberOfProjects = 1;
        int userId = 101;
        int numberOfTasks = 7;

        using var dbContext = await GetInMemoryDbContextAsync(numberOfProjects, userId, numberOfTasks);
        var repository = new ProjectRepository(dbContext);

        var projects = await dbContext.Projects.ToArrayAsync();
        var project = projects[0];
        var workUnit = project.WorkUnits.ToArray()[0];

        var projectAuthorizationService = new ProjectAuthorizationService();

        var notificationHandler = new BussinessNotificationsHandler();

        var serializeService = new SerializeService();

        var command = new UpdateWorkUnitCommand()
        {
            ProjectId = project.Id,
            WorkUnitId = workUnit.Id,
            Title = "New title of task",
            Description = "New description of task",
            DueDate = DateTime.Now.AddDays(12),
            Status = Status.InProgress
        };

        var commandHandler = new UpdateWorkUnitCommandHandler(repository,
                                                              notificationHandler,
                                                              serializeService);

        // act
        var statusCode = await commandHandler.Handle(command, CancellationToken.None);

        // assert
        statusCode.Should().Be(HttpStatusCode.NoContent);
    }

}