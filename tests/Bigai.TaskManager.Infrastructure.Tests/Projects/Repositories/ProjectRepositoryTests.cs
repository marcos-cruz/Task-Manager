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