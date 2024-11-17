using System.Net;

using Bigai.TaskManager.Api.Tests.Helpers;
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

public class ReportsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IProjectRepository _projectsRepositoryMock;
    private readonly IProjectAuthorizationService _projectAuthorizationServiceMock;
    private readonly ISerializeService _serializeService;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IBussinessNotificationsHandler _bussinessNotificationsHandler;
    private readonly int _numberOfProjects = 7;
    private readonly int _userId = 101;
    private readonly int _numberOfTasks = 17;

    public ReportsControllerTests(WebApplicationFactory<Program> factory)
    {
        var dbContext = GetInMemoryDbContext(_numberOfProjects, _userId, _numberOfTasks);
        _projectsRepositoryMock = new ProjectRepository(dbContext);
        _projectAuthorizationServiceMock = new ProjectAuthorizationService();
        _serializeService = new SerializeService();
        _bussinessNotificationsHandler = new BussinessNotificationsHandler();

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
                services.Replace(ServiceDescriptor.Scoped(typeof(IBussinessNotificationsHandler), _ => _bussinessNotificationsHandler));
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
    public async Task GetReportByRangeAsync_ReturnsStatus200OK()
    {
        // arrange
        var projects = await _projectsRepositoryMock.GetProjectsByUserIdAsync(_userId);
        var project = projects.ToArray()[0];
        var createDate = project.WorkUnits.ToArray()[0].CreateDate;

        string initialPeriod = $"{createDate.Day:D2}/{createDate.Month:D2}/{createDate.Year}";
        createDate = createDate.AddDays(30);
        string finalPeriod = $"{createDate.Day:D2}/{createDate.Month:D2}/{createDate.Year}";
        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"api/projects/performance/range?initialDate={initialPeriod}&finalDate={finalPeriod}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetReportByPeriodAsync_ReturnsStatus200OK()
    {
        // arrange
        var projects = await _projectsRepositoryMock.GetProjectsByUserIdAsync(_userId);
        var project = projects.ToArray()[0];
        var createDate = project.WorkUnits.ToArray()[0].CreateDate;

        string initialPeriod = $"{createDate.Day:D2}/{createDate.Month:D2}/{createDate.Year}";
        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"api/projects/performance/period?initialDate={initialPeriod}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetReportByProjectIdAsync_ReturnsStatus200OK()
    {
        // arrange
        var projects = await _projectsRepositoryMock.GetProjectsByUserIdAsync(_userId);
        var project = projects.ToArray()[0];
        var createDate = project.WorkUnits.ToArray()[0].CreateDate;

        string initialPeriod = $"{createDate.Day:D2}/{createDate.Month:D2}/{createDate.Year}";
        createDate = createDate.AddDays(30);
        string finalPeriod = $"{createDate.Day:D2}/{createDate.Month:D2}/{createDate.Year}";
        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"api/projects/performance/{project.Id}/project?initialDate={initialPeriod}&finalDate={finalPeriod}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}