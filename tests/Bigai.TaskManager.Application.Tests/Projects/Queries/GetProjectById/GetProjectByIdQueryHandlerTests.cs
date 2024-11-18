using System.Net;

using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Queries.GetProjectById;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;
using Bigai.TaskManager.Domain.Tests.Helpers;
using Bigai.TaskManager.Infrastructure.Projects.Services;

using FluentAssertions;

using Moq;

namespace Bigai.TaskManager.Application.Tests.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _projectsRepositoryMock;
    private readonly GetProjectByIdQueryHandler _queryHandler;
    private readonly IBussinessNotificationsHandler _notificationsHandler;

    public GetProjectByIdQueryHandlerTests()
    {
        _projectsRepositoryMock = new Mock<IProjectRepository>();
        _notificationsHandler = new BussinessNotificationsHandler();
        _queryHandler = new GetProjectByIdQueryHandler(_projectsRepositoryMock.Object, _notificationsHandler);
    }

    [Fact()]
    public async Task Handle_WithExistingProjectId_ReturnsProject()
    {
        // arrange
        int projectId = 1001;
        int userId = 1010101;
        int amountProjects = 15;
        var query = new GetProjectByIdQuery(projectId);
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(amountProjects, userId);

        _projectsRepositoryMock
            .Setup(repo => repo.GetProjectByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects.First());

        // act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // assert
        result.Should().NotBeNull();
        _notificationsHandler.StatusCode.Should().Be(HttpStatusCode.OK);
        Assert.IsAssignableFrom<ProjectDto>(result);
    }

    [Fact()]
    public async Task Handle_WithNonExistingProjectId_ReturnsNull()
    {
        // arrange
        int projectId = 1001;
        var query = new GetProjectByIdQuery(projectId);
        Project? project = null;

        _projectsRepositoryMock
            .Setup(repo => repo.GetProjectByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // assert
        result.Should().BeNull();
        _notificationsHandler.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}