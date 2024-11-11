using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Infrastructure.Persistence;
using Bigai.TaskManager.Infrastructure.Projects.Repositories;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

namespace Bigai.TaskManager.Infrastructure.Tests.Projects.Repositories;

public class ProjectRepositoryTests
{
    private readonly int _registeredUser = 1001;
    private readonly int _unRegisteredUser = 1001;

    private async Task<TaskManagerDbContext> GetInMemoryDbContextAsync()
    {
        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new TaskManagerDbContext(options);

        // Add test data to the in-memory database
        context.Projects.AddRange(new List<Project>
        {
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.Average),
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.High),
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.Low),
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.Average),
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.High),
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.Low),
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.Low),
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.Average),
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.Average),
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.High),
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.Low),
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.Average),
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.High),
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.Low),
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.Low),
            Project.Create(_registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.Average),
        });

        await context.SaveChangesAsync();

        return context;
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllAsync_WhenUserHasRegisteredProject_ReturnsProjects()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);

        // Act
        var projects = await repository.GetProjectsByUserIdAsync(_registeredUser, CancellationToken.None);

        // Assert
        projects.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllAsync_WhenUserHasNoRegisteredProject_ReturnsNoOneProjects()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);

        // Act
        var projects = await repository.GetProjectsByUserIdAsync(_unRegisteredUser, CancellationToken.None);

        // Assert
        projects.Should().NotBeNullOrEmpty();
    }

}