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

public class ProjectsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IProjectRepository> _projectsRepositoryMock = new();
    private readonly Mock<IProjectAuthorizationService> _projectAuthorizationServiceMock = new();

    public ProjectsControllerTests(WebApplicationFactory<Program> factory)
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
    public async Task GetByUserIdAsync_ReturnsStatus200OK()
    {
        // arrange
        int registeredUser = 1001;
        int amountProjects = 15;
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(amountProjects, registeredUser);

        _projectsRepositoryMock.Setup(p => p.GetProjectsByUserIdAsync(registeredUser, CancellationToken.None))
                                   .ReturnsAsync(projects);

        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/projects/users/{registeredUser}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ReturnsStatus200OK()
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
        var response = await client.GetAsync($"/api/projects/{projectId}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateAsync_ReturnsStatus201Created()
    {
        // arrange
        int projectId = 1;
        _projectsRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Project>(), CancellationToken.None))
            .ReturnsAsync(projectId);

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
    public async Task RemoveAsync_ReturnsStatus204NoContent()
    {
        // arrange
        int registeredUser = 1001;
        int amountProjects = 15;
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(amountProjects, registeredUser);
        int projectId = projects.First().Id;
        Project project = projects.First();

        _projectsRepositoryMock.Setup(p => p.GetProjectByIdAsync(projectId, CancellationToken.None))
                               .ReturnsAsync(project);

        _projectAuthorizationServiceMock.Setup(service => service.Authorize(project, ResourceOperation.Remove))
                                        .Returns(true);

        var client = _factory.CreateClient();

        // act
        var response = await client.DeleteAsync($"/api/projects/{projectId}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoveAsync_ReturnsStatus400BadRequest()
    {
        // arrange
        int registeredUser = 1001;
        int amountProjects = 15;
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(amountProjects, registeredUser);
        int projectId = projects.First().Id;
        Project project = projects.First();

        _projectsRepositoryMock.Setup(p => p.GetProjectByIdAsync(projectId, CancellationToken.None))
                               .ReturnsAsync(project);

        _projectAuthorizationServiceMock.Setup(service => service.Authorize(project, ResourceOperation.Remove))
                                        .Returns(false);

        var client = _factory.CreateClient();

        // act
        var response = await client.DeleteAsync($"/api/projects/{projectId}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveAsync_ReturnsStatus404NotFound()
    {
        // arrange
        int registeredUser = 1001;
        int amountProjects = 15;
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(amountProjects, registeredUser);
        int projectId = projects.First().Id;
        Project? project = null;

        _projectsRepositoryMock.Setup(p => p.GetProjectByIdAsync(projectId, CancellationToken.None))
                               .ReturnsAsync(project);

        _projectAuthorizationServiceMock.Setup(service => service.Authorize(project, ResourceOperation.Remove))
                                        .Returns(false);

        var client = _factory.CreateClient();

        // act
        var response = await client.DeleteAsync($"/api/projects/{projectId}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

}