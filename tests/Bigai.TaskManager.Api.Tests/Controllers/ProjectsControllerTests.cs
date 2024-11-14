using System.Net;

using Bigai.TaskManager.Api.Tests.Helpers;
using Bigai.TaskManager.Application.Projects.Commands.CreateProject;
using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;
using Bigai.TaskManager.Infrastructure.Persistence;
using Bigai.TaskManager.Infrastructure.Projects.Repositories;
using Bigai.TaskManager.Infrastructure.Projects.Services;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bigai.TaskManager.Api.Tests.Controllers;

public class ProjectsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IProjectRepository _projectsRepositoryMock;
    private readonly IProjectAuthorizationService _projectAuthorizationServiceMock;
    private readonly ISerializeService _serializeService;

    private readonly int _numberOfProjects = 7;
    private readonly int _userId = 101;
    private readonly int _numberOfTasks = 7;

    public ProjectsControllerTests(WebApplicationFactory<Program> factory)
    {
        var dbContext = GetInMemoryDbContext(_numberOfProjects, _userId, _numberOfTasks);
        _projectsRepositoryMock = new ProjectRepository(dbContext);
        _projectAuthorizationServiceMock = new ProjectAuthorizationService();
        _serializeService = new SerializeService();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Replace(ServiceDescriptor.Scoped(typeof(IProjectRepository), _ => _projectsRepositoryMock));
                services.Replace(ServiceDescriptor.Scoped(typeof(IProjectAuthorizationService), _ => _projectAuthorizationServiceMock));
                services.Replace(ServiceDescriptor.Scoped(typeof(ISerializeService), _ => _serializeService));
            });
        });
    }

    private static TaskManagerDbContext GetInMemoryDbContext(int numberOfProjects, int userId, int numberOfTasks)
    {
        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new TaskManagerDbContext(options);

        List<Project> projects = GetMockedValues(numberOfProjects, userId, numberOfTasks);

        if (projects is not null && projects.Count > 0)
        {
            context.Projects.AddRange(projects);

            context.SaveChanges();
        }

        return context;
    }

    private static List<Project> GetMockedValues(int numberOfProjects, int userId, int numberOfTasks)
    {
        var projects = new List<Project>();

        if (numberOfProjects > 0)
        {
            Random rnd = new();

            for (int i = 0; i < numberOfProjects; i++)
            {
                var project = Project.Create($"Project name {Guid.NewGuid()}");

                for (int j = 0; j < numberOfTasks; j++)
                {
                    var dueDate = DateTime.Now.AddDays(rnd.Next(15, 45));
                    var priority = (Priority)rnd.Next(0, 2);
                    var workUnit = WorkUnit.Create("Title of task", "Description of task", dueDate, priority);

                    if (i == 0)
                    {
                        workUnit.ChangeStatus(Status.Completed);
                    }

                    workUnit.AssignToUser(userId);
                    project.AddWorkUnit(workUnit);
                }

                projects.Add(project);
            }
        }

        return projects;
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsStatus200OK()
    {
        // arrange
        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/projects/users/{_userId}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ReturnsStatus200OK()
    {
        // arrange
        var projects = await _projectsRepositoryMock.GetProjectsByUserIdAsync(_userId);
        var project = projects.ToArray()[0];

        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/projects/{project.Id}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ReturnsStatus404NotFound()
    {
        // arrange
        int projectId = 101001001;
        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/projects/{projectId}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateAsync_ReturnsStatus201Created()
    {
        // arrange
        var command = new CreateProjectCommand
        {
            Name = "Create new project test"
        };

        var client = _factory.CreateClient();

        // act
        var response = await client.PostAsync("/api/projects", TestHelper.GetJsonStringContent(command));

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateAsync_ReturnsStatus400BadRequest()
    {
        // arrange
        var command = new CreateProjectCommand();

        var client = _factory.CreateClient();

        // act
        var response = await client.PostAsync("/api/projects", TestHelper.GetJsonStringContent(command));

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveAsync_ReturnsStatus204NoContent()
    {
        // arrange
        var projects = await _projectsRepositoryMock.GetProjectsByUserIdAsync(_userId);
        var project = projects.ToArray()[0];

        var client = _factory.CreateClient();

        // act
        var response = await client.DeleteAsync($"/api/projects/{project.Id}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoveAsync_ReturnsStatus400BadRequest()
    {
        // arrange
        var projects = await _projectsRepositoryMock.GetProjectsByUserIdAsync(_userId);
        var project = projects.ToArray()[1];

        var client = _factory.CreateClient();

        // act
        var response = await client.DeleteAsync($"/api/projects/{project.Id}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveAsync_ReturnsStatus404NotFound()
    {
        // arrange
        int projectId = _numberOfProjects * 2;
        var client = _factory.CreateClient();

        // act
        var response = await client.DeleteAsync($"/api/projects/{projectId}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}