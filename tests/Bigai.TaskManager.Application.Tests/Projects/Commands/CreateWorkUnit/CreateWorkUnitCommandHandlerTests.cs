using Bigai.TaskManager.Application.Projects.Commands.CreateWorkUnit;
using Bigai.TaskManager.Application.Users;
using Bigai.TaskManager.Domain.Projects.Constants;
using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Infrastructure.Persistence;
using Bigai.TaskManager.Infrastructure.Projects.Repositories;
using Bigai.TaskManager.Infrastructure.Projects.Services;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Moq;

namespace Bigai.TaskManager.Application.Tests.Projects.Commands.CreateWorkUnit;

public class CreateWorkUnitCommandHandlerTests
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
    public async Task Handle_ForValidCommand_ReturnsCreatedWorkUnitId()
    {
        // arrange
        int numberOfProjects = 1;
        int userId = 101;
        int numberOfTasks = 7;

        using var dbContext = await GetInMemoryDbContextAsync(numberOfProjects, userId, numberOfTasks);
        var repository = new ProjectRepository(dbContext);

        var projects = await dbContext.Projects.ToArrayAsync();
        var project = projects[0];

        var projectAuthorizationService = new ProjectAuthorizationService();

        var notificationHandler = new BussinessNotificationsHandler();

        var userContextMock = new Mock<IUserContext>();
        var currentUser = new CurrentUser(userId, []);
        userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        var command = new CreateWorkUnitCommand()
        {
            ProjectId = project.Id,
            Title = "Title of task",
            Description = "Description of task",
            DueDate = DateTime.Now.AddDays(18),
            Priority = Priority.Low
        };

        var commandHandler = new CreateWorkUnitCommandHandler(repository,
                                                              projectAuthorizationService,
                                                              notificationHandler,
                                                              userContextMock.Object);

        // act
        var workUnitId = await commandHandler.Handle(command, CancellationToken.None);

        // assert
        workUnitId.Should().BeGreaterThan(0);
    }

    [Fact()]
    public async Task Handle_ForValidCommandWithNoExistenteProject_ReturnsNotFound()
    {
        // arrange
        int numberOfProjects = 1;
        int userId = 101;
        int numberOfTasks = 7;

        using var dbContext = await GetInMemoryDbContextAsync(numberOfProjects, userId, numberOfTasks);
        var repository = new ProjectRepository(dbContext);
        var projectAuthorizationService = new ProjectAuthorizationService();

        var notificationHandler = new BussinessNotificationsHandler();

        var userContextMock = new Mock<IUserContext>();
        var currentUser = new CurrentUser(userId, []);
        userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        var command = new CreateWorkUnitCommand()
        {
            ProjectId = 1010101,
            Title = "Title of task",
            Description = "Description of task",
            DueDate = DateTime.Now.AddDays(18),
            Priority = Priority.Low
        };

        var commandHandler = new CreateWorkUnitCommandHandler(repository,
                                                              projectAuthorizationService,
                                                              notificationHandler,
                                                              userContextMock.Object);

        // act
        var workUnitId = await commandHandler.Handle(command, CancellationToken.None);

        // assert
        workUnitId.Should().Be(TaskManagerRoles.NotFound);
    }

    [Fact()]
    public async Task Handle_ForValidCommandAndProjectWithOverrunTasks_ReturnsForbidden()
    {
        // arrange
        int numberOfProjects = 1;
        int userId = 101;
        int numberOfTasks = 20;

        using var dbContext = await GetInMemoryDbContextAsync(numberOfProjects, userId, numberOfTasks);
        var repository = new ProjectRepository(dbContext);

        var projects = await dbContext.Projects.ToArrayAsync();
        var project = projects[0];

        var projectAuthorizationService = new ProjectAuthorizationService();

        var notificationHandler = new BussinessNotificationsHandler();

        var userContextMock = new Mock<IUserContext>();
        var currentUser = new CurrentUser(101, []);
        userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        var command = new CreateWorkUnitCommand()
        {
            ProjectId = project.Id,
            Title = "Title of task",
            Description = "Description of task",
            DueDate = DateTime.Now.AddDays(18),
            Priority = Priority.Low
        };

        var commandHandler = new CreateWorkUnitCommandHandler(repository,
                                                              projectAuthorizationService,
                                                              notificationHandler,
                                                              userContextMock.Object);

        // act
        var workUnitId = await commandHandler.Handle(command, CancellationToken.None);

        // assert
        workUnitId.Should().Be(TaskManagerRoles.Forbidden);
    }

}