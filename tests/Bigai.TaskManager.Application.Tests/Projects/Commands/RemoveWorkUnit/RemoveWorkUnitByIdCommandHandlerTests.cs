using Bigai.TaskManager.Application.Projects.Commands.RemoveWorkUnit;
using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Tests.Helpers;
using Bigai.TaskManager.Infrastructure.Persistence;
using Bigai.TaskManager.Infrastructure.Projects.Repositories;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Moq;

namespace Bigai.TaskManager.Application.Tests.Projects.Commands.RemoveWorkUnit;

public class RemoveWorkUnitByIdCommandHandlerTests
{
    private async Task<TaskManagerDbContext> GetInMemoryDbContextAsync(int numberOfProjects, int userId, int numberOfTasks)
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
                    var dueDate = DateTimeOffset.Now.AddDays(rnd.Next(15, 45));
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

    [Fact]
    public async Task Handle_WhenWorkUnitDoesNotExist_ReturnsFalse()
    {
        // arrange
        int numberOfProjects = 1;
        int userId = 101;
        int numberOfTasks = 7;
        int workUnitId = numberOfTasks * 2;

        using var dbContext = await GetInMemoryDbContextAsync(numberOfProjects, userId, numberOfTasks);
        var repository = new ProjectRepository(dbContext);

        var command = new RemoveWorkUnitByIdCommand(workUnitId);

        var commandHandler = new RemoveWorkUnitByIdCommandHandler(repository);

        // act
        var removed = await commandHandler.Handle(command, CancellationToken.None);

        // assert
        removed.Should().Be(false);
    }

    [Fact]
    public async Task Handle_WhenWorkUnitExists_ReturnsTrue()
    {
        // arrange
        int amountProjects = 1;
        int userId = 101;
        var projectRepositoryMock = new Mock<IProjectRepository>();
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(amountProjects, userId);
        WorkUnit workUnit = projects.First().WorkUnits.ToArray()[0];

        projectRepositoryMock
            .Setup(repo => repo.GetWorkUnitByIdAsync(workUnit.Id, CancellationToken.None))
            .ReturnsAsync(workUnit);

        var command = new RemoveWorkUnitByIdCommand(workUnit.Id);

        var commandHandler = new RemoveWorkUnitByIdCommandHandler(projectRepositoryMock.Object);

        // act
        var removed = await commandHandler.Handle(command, CancellationToken.None);

        // assert
        removed.Should().Be(true);
    }
}