using Bigai.TaskManager.Domain.Projects.Enums;
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

    private readonly int _userIdReport = 1002;

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

    private async Task<TaskManagerDbContext> GetInMemoryReportDbContextAsync()
    {
        var dbContext = await GetInMemoryDbContextAsync();
        var projects = new List<Project>();
        var rnd = new Random();

        for (int i = 0; i < 2; i++)
        {
            var project = Project.Create($"Test Project {Guid.NewGuid()}");

            for (int j = 0; j < 20; j++)
            {
                var priority = (Priority)rnd.Next(0, 2);
                var dueDate = DateTime.Now.AddDays(rnd.Next(15, 45));

                var workUnit = WorkUnit.Create("Work unit title", "Work unit description", dueDate, priority);
                workUnit.AssignToUser(_userIdReport);
                workUnit.ChangeStatus(Status.InProgress);
                workUnit.ChangeStatus(Status.Completed);
                project.AddWorkUnit(workUnit);
            }
            projects.Add(project);
        }

        dbContext.Projects.AddRange(projects);

        await dbContext.SaveChangesAsync();

        return dbContext;
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

    [Fact]
    public async Task RemoveWorkUnitAsync_RemoveWorkUnitSuccessfully()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);
        var existingWorkUnit = await repository.GetWorkUnitByIdAsync(1, 1, CancellationToken.None);
        int projectId = existingWorkUnit!.ProjectId!.Value;

        // Act
        await repository.RemoveWorkUnitAsync(existingWorkUnit!);
        var removedWorkUnit = await repository.GetWorkUnitByIdAsync(projectId, existingWorkUnit!.Id, CancellationToken.None);

        // Assert
        removedWorkUnit.Should().BeNull();
    }

    [Fact]
    public async Task GetWorkUnitByIdAsync_WhenWorkUnitExists_ReturnsWorkUnit()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);
        var existingProjectId = 1;
        var existingWorkUnitId = 1;

        // Act
        var workUnit = await repository.GetWorkUnitByIdAsync(existingProjectId, existingWorkUnitId, CancellationToken.None);

        // Assert
        workUnit.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWorkUnitByIdAsync_WhenWorkUnitDoesNotExist_ReturnsNull()
    {
        // Arrange
        using var dbContext = await GetInMemoryDbContextAsync();
        var repository = new ProjectRepository(dbContext);
        var unregisteredWorkProjectId = _amountProjects + 99;
        var unregisteredWorkUnitId = _amountProjects + 99;

        // Act
        var workUnit = await repository.GetWorkUnitByIdAsync(unregisteredWorkProjectId, unregisteredWorkUnitId, CancellationToken.None);

        // Assert
        workUnit.Should().BeNull();
    }

    [Fact]
    public async Task GetReportByRangeAsync_ReturnsValues()
    {
        // arrange
        DateTime initialRange = DateTime.Now;
        DateTime finalRange = initialRange.AddDays(30);
        using var dbContext = await GetInMemoryReportDbContextAsync();
        var repository = new ProjectRepository(dbContext);

        // act
        var report = await repository.GetReportByRangeAsync(initialRange, finalRange, CancellationToken.None);

        // assert
        report.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetReportByProjectIdAsync_ReturnsValues()
    {
        // arrange
        DateTime initialRange = DateTime.Now;
        DateTime finalRange = initialRange.AddDays(30);
        using var dbContext = await GetInMemoryReportDbContextAsync();
        var repository = new ProjectRepository(dbContext);
        Project existingProject = (await repository.GetProjectsByUserIdAsync(_userIdReport, CancellationToken.None)).First();

        // act
        var report = await repository.GetReportByProjectIdAsync(existingProject.Id, initialRange, finalRange, CancellationToken.None);

        // assert
        report.Should().NotBeNullOrEmpty();
    }
}