using System.Net;

using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
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
    private readonly int _registeredUser = 1001;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IProjectRepository> _projectsRepositoryMock = new();

    public ProjectsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Replace(ServiceDescriptor.Scoped(typeof(IProjectRepository), _ => _projectsRepositoryMock.Object));
            });
        });
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsStatus200OK()
    {
        // arrange
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(15, 1001);

        _projectsRepositoryMock.Setup(p => p.GetProjectsByUserIdAsync(_registeredUser, CancellationToken.None))
                               .ReturnsAsync(projects);

        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/projects/{_registeredUser}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}