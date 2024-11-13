using System.Net;

using Bigai.TaskManager.Api.Tests.Helpers;
using Bigai.TaskManager.Application.Projects.Commands.CreateProject;
using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;
using Bigai.TaskManager.Domain.Tests.Helpers;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Moq;

namespace Bigai.TaskManager.Api.Tests.Controllers;

public class WorkUnitsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IProjectRepository> _projectsRepositoryMock = new();
    private readonly Mock<IProjectAuthorizationService> _projectAuthorizationServiceMock = new();

    public WorkUnitsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Replace(ServiceDescriptor.Scoped(typeof(IProjectRepository), _ => _projectsRepositoryMock.Object));
                services.Replace(ServiceDescriptor.Scoped(typeof(IProjectAuthorizationService), _ => _projectAuthorizationServiceMock.Object));
            });
        });
    }

    [Fact]
    public async Task GetWorkUnitsByProjectIdAsync_ReturnsStatus200OK()
    {
        // arrange
        int registeredUser = 1001;
        int projectId = 1;
        int amountProjects = 15;
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(amountProjects, registeredUser);

        _projectsRepositoryMock.Setup(p => p.GetProjectByIdAsync(projectId, CancellationToken.None))
                               .ReturnsAsync(projects.First());

        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/projects/{projectId}/tasks/");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetWorkUnitsByProjectIdAsync_ReturnsStatus404NotFound()
    {
        // arrange
        int projectId = 1;
        Project? project = null;

        _projectsRepositoryMock.Setup(p => p.GetProjectByIdAsync(projectId, CancellationToken.None))
                               .ReturnsAsync(project);

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
        int registeredUser = 1001;
        int amountProjects = 15;
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(amountProjects, registeredUser);
        Project project = projects.First();
        WorkUnit workUnit = project.WorkUnits.ToArray()[0];

        _projectsRepositoryMock.Setup(p => p.GetWorkUnitByIdAsync(workUnit.Id, CancellationToken.None))
                               .ReturnsAsync(workUnit);

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
        int registeredUser = 1001;
        int amountProjects = 15;
        int workUnitId = 77;
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(amountProjects, registeredUser);
        Project project = projects.First();
        WorkUnit? workUnit = null;

        _projectsRepositoryMock.Setup(p => p.GetWorkUnitByIdAsync(workUnitId, CancellationToken.None))
                               .ReturnsAsync(workUnit);

        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/projects/{project.Id}/tasks/{workUnitId}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

}