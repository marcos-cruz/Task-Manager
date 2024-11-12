using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Tests.Helpers;
using Bigai.TaskManager.Infrastructure.Persistence;
using Bigai.TaskManager.Infrastructure.Projects.Repositories;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

namespace Bigai.TaskManager.Infrastructure.Tests.Projects.Repositories;

public class ProjectRepositoryTests
{
    private readonly int _userIdRegistered = 1001;

    private readonly int _userIdUnregistered = 1001;

    private readonly int _amountProjects = 15;

    private async Task<TaskManagerDbContext> GetInMemoryDbContextAsync()
    {
        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new TaskManagerDbContext(options);

        var projects = ProjectHelper.GetProjects(_amountProjects, _userIdRegistered);

        context.Projects.AddRange(projects);

        await context.SaveChangesAsync();

        return context;
    }

    [Fact]
    public async Task GetProjectsByUserIdAsync_WhenUserHasRegisteredProject_ReturnsProjects()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);

        // Act
        var projects = await repository.GetProjectsByUserIdAsync(_userIdRegistered, CancellationToken.None);

        // Assert
        projects.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetProjectsByUserIdAsync_WhenUserHasNoRegisteredProject_ReturnsNoOneProjects()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);

        // Act
        var projects = await repository.GetProjectsByUserIdAsync(_userIdUnregistered, CancellationToken.None);

        // Assert
        projects.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateAsync_WhenCreateProject_ReturnsProjectId()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);
        Project project = ProjectHelper.GetProjects(1, _userIdRegistered).First();

        // Act
        var projectId = await repository.CreateAsync(project, CancellationToken.None);

        // Assert
        projectId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateAsync_WhenCreateWorkUnit_ReturnsWorkUnitId()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);
        Project project = ProjectHelper.GetProjects(1, _userIdRegistered).First();
        WorkUnit workUnit = project.WorkUnits.First();
        workUnit.AssignToProject(project.Id);

        // Act
        var workUnitId = await repository.CreateAsync(workUnit, CancellationToken.None);

        // Assert
        workUnitId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetProjectByIdAsync_WhenProjectExists_ReturnsProject()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);
        var existingProjectId = 1;

        // Act
        var project = await repository.GetProjectByIdAsync(existingProjectId, CancellationToken.None);

        // Assert
        project.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProjectByIdAsync_WhenProjectDoesNotExist_ReturnsNull()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);
        var unregisteredProjectId = _amountProjects + 1;

        // Act
        var project = await repository.GetProjectByIdAsync(unregisteredProjectId, CancellationToken.None);

        // Assert
        project.Should().BeNull();
    }

    [Fact]
    public async Task RemoveProjectAsync_RemoveProjectSuccessfully()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);
        Project existingProject = (await repository.GetProjectsByUserIdAsync(_userIdRegistered, CancellationToken.None)).First();

        // Act
        await repository.RemoveProjectAsync(existingProject);
        var removedProject = await repository.GetProjectByIdAsync(existingProject.Id, CancellationToken.None);

        // Assert
        removedProject.Should().BeNull();
    }
}