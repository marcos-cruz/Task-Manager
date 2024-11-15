using System.Net;

using Bigai.TaskManager.Api.Tests.Helpers;
using Bigai.TaskManager.Application.Projects.Commands.CreateWorkUnit;
using Bigai.TaskManager.Application.Projects.Commands.UpdateWorkUnit;
using Bigai.TaskManager.Application.Users;
using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;
using Bigai.TaskManager.Infrastructure.Persistence;
using Bigai.TaskManager.Infrastructure.Projects.Repositories;
using Bigai.TaskManager.Infrastructure.Projects.Services;

using FluentAssertions;

using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Moq;

namespace Bigai.TaskManager.Api.Tests.Controllers;

public class WorkUnitsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IProjectRepository _projectsRepositoryMock;
    private readonly IProjectAuthorizationService _projectAuthorizationServiceMock;
    private readonly ISerializeService _serializeService;
    private readonly Mock<IUserContext> _userContextMock;

    private readonly int _numberOfProjects = 7;
    private readonly int _userId = 101;
    private readonly int _numberOfTasks = 7;

    public WorkUnitsControllerTests(WebApplicationFactory<Program> factory)
    {
        var dbContext = GetInMemoryDbContext(_numberOfProjects, _userId, _numberOfTasks);
        _projectsRepositoryMock = new ProjectRepository(dbContext);
        _projectAuthorizationServiceMock = new ProjectAuthorizationService();
        _serializeService = new SerializeService();

        _userContextMock = new Mock<IUserContext>();
        var currentUser = new CurrentUser(101, []);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);


        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

                services.Replace(ServiceDescriptor.Scoped(typeof(IProjectRepository), _ => _projectsRepositoryMock));
                services.Replace(ServiceDescriptor.Scoped(typeof(IProjectAuthorizationService), _ => _projectAuthorizationServiceMock));
                services.Replace(ServiceDescriptor.Scoped(typeof(ISerializeService), _ => _serializeService));
                services.Replace(ServiceDescriptor.Scoped(typeof(IUserContext), _ => _userContextMock.Object));
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
    public async Task GetWorkUnitsByProjectIdAsync_ReturnsStatus200OK()
    {
        // arrange
        var projects = await _projectsRepositoryMock.GetProjectsByUserIdAsync(_userId);
        var project = projects.ToArray()[0];

        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/projects/{project.Id}/tasks/");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetWorkUnitsByProjectIdAsync_ReturnsStatus404NotFound()
    {
        // arrange
        int projectId = 101001;

        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/projects/{projectId}/tasks/");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUnitWorkByIdAsync_ReturnsStatus200OK()
    {
        // arrange
        var projects = await _projectsRepositoryMock.GetProjectsByUserIdAsync(_userId);
        var project = projects.ToArray()[0];
        var workUnit = project.WorkUnits.ToArray()[0];
        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/projects/{project.Id}/tasks/{workUnit.Id}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ReturnsStatus404NotFound()
    {
        // arrange
        var projectId = 101001;
        var workUnitId = 122;

        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/projects/{projectId}/tasks/{workUnitId}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateAsync_ReturnsStatus201Created()
    {
        // arrange
        var projects = await _projectsRepositoryMock.GetProjectsByUserIdAsync(_userId);
        var project = projects.ToArray()[2];
        var dueDate = DateTime.Now.AddDays(15);

        var command = new CreateWorkUnitCommand
        {
            ProjectId = project.Id,
            Title = "Title of new work unit test",
            Description = "Description of new work unit test",
            DueDate = dueDate,
            Priority = Priority.Average
        };

        var client = _factory.CreateClient();

        // act
        var response = await client.PostAsync($"/api/projects/{project.Id}/tasks/", TestHelper.GetJsonStringContent(command));

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsStatus204NoContent()
    {
        // arrange
        var projects = await _projectsRepositoryMock.GetProjectsByUserIdAsync(_userId);
        var project = projects.ToArray()[2];
        var workUnit = project.WorkUnits.ToArray()[0];
        var dueDate = DateTime.Now.AddDays(5);

        var command = new UpdateWorkUnitCommand
        {
            ProjectId = project.Id,
            WorkUnitId = workUnit.Id,
            Title = "Title of new work unit test",
            Description = "Description of new work unit test",
            DueDate = dueDate,
            Status = Status.InProgress
        };

        var client = _factory.CreateClient();

        // act
        var response = await client.PutAsync($"/api/projects/{project.Id}/tasks/{workUnit.Id}", TestHelper.GetJsonStringContent(command));

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CreateAsync_ReturnsStatus400BadRequest()
    {
        // arrange
        var projectId = 1010011;
        var command = new CreateWorkUnitCommand();

        var client = _factory.CreateClient();

        // act
        var response = await client.PostAsync($"/api/projects/{projectId}/tasks", TestHelper.GetJsonStringContent(command));

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAsync_ReturnsStatus404NotFound()
    {
        // arrange
        var projectId = _numberOfProjects * 2;
        var dueDate = DateTime.Now.AddDays(15);

        var command = new CreateWorkUnitCommand
        {
            ProjectId = projectId,
            Title = "Title of new work unit test",
            Description = "Description of new work unit test",
            DueDate = dueDate,
            Priority = Priority.Average
        };

        var client = _factory.CreateClient();

        // act
        var response = await client.PostAsync($"/api/projects/{projectId}/tasks/", TestHelper.GetJsonStringContent(command));

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveAsync_ReturnsStatus204NoContent()
    {
        // arrange
        // arrange
        var projects = await _projectsRepositoryMock.GetProjectsByUserIdAsync(_userId);
        var project = projects.ToArray()[0];
        var workUnit = project.WorkUnits.ToArray()[0];

        var client = _factory.CreateClient();

        // act
        var response = await client.DeleteAsync($"/api/projects/{workUnit.ProjectId}/tasks/{workUnit.Id}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoveAsync_ReturnsStatus404NotFound()
    {
        // arrange
        int projectId = 7;
        int workUnitId = 77;

        var client = _factory.CreateClient();

        // act
        var response = await client.DeleteAsync($"/api/projects/{projectId}/tasks/{workUnitId}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

// TODO: Mock token para test integrado
// https://stebet.net/mocking-jwt-tokens-in-asp-net-core-integration-tests/