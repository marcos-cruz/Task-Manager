using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Tests.Helpers;
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

        var projects = ProjectHelper.GetProjects(15, _registeredUser);

        // Add test data to the in-memory database
        context.Projects.AddRange(projects);

        await context.SaveChangesAsync();

        return context;
    }

    [Fact]
    public async Task GetAllAsync_WhenUserHasRegisteredProject_ReturnsProjects()
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
    public async Task GetAllAsync_WhenUserHasNoRegisteredProject_ReturnsNoOneProjects()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);

        // Act
        var projects = await repository.GetProjectsByUserIdAsync(_unRegisteredUser, CancellationToken.None);

        // Assert
        projects.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenCreateProject_ReturnsProjectId()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);
        Project project = ProjectHelper.GetProjects(15, _registeredUser).First();

        // Act
        var projectId = await repository.CreateAsync(project, CancellationToken.None);

        // Assert
        projectId.Should().BeGreaterThan(0);
    }


    [Fact]
    public async Task GetProjectByIdAsync_WhenProjectExists_ReturnsProject()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);

        // Act
        var project = await repository.GetProjectByIdAsync(1, CancellationToken.None);

        // Assert
        project.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProjectByIdAsync_WhenProjectDoesNotExist_ReturnsNull()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);

        // Act
        var project = await repository.GetProjectByIdAsync(_unRegisteredUser, CancellationToken.None);

        // Assert
        project.Should().BeNull();
    }
}