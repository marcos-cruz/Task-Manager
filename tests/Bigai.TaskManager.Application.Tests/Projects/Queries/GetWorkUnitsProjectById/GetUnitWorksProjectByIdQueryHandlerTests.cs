using System.Net;

using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Queries.GetWorkUnitsProjectById;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;
using Bigai.TaskManager.Domain.Tests.Helpers;
using Bigai.TaskManager.Infrastructure.Projects.Services;

using FluentAssertions;

using Moq;

namespace Bigai.TaskManager.Application.Tests.Projects.Queries.GetWorkUnitsProjectById;

public class GetUnitWorksProjectByIdQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _projectsRepositoryMock;
    private readonly GetUnitWorksProjectByIdQueryHandler _queryHandler;
    private readonly IBussinessNotificationsHandler _notificationsHandler;


    public GetUnitWorksProjectByIdQueryHandlerTests()
    {
        _projectsRepositoryMock = new Mock<IProjectRepository>();
        _notificationsHandler = new BussinessNotificationsHandler();
        _queryHandler = new GetUnitWorksProjectByIdQueryHandler(_projectsRepositoryMock.Object, _notificationsHandler);
    }

    [Fact()]
    public async Task Handle_WhenProjectDoesNotExist_ReturnsNull()
    {
        // arrange
        int projectId = 1;
        var query = new GetUnitWorksProjectByIdQuery(projectId);
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

    [Fact()]
    public async Task Handle_WhenProjectExists_ReturnsWorkUnits()
    {
        // arrange
        int projectId = 1;
        int amountProjects = 1;
        int registeredUser = 1001;
        var query = new GetUnitWorksProjectByIdQuery(projectId);
        Project project = ProjectHelper.GetProjects(amountProjects, registeredUser).First();

        _projectsRepositoryMock
            .Setup(repo => repo.GetProjectByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // assert
        result.Should().NotBeNullOrEmpty();
        _notificationsHandler.StatusCode.Should().Be(HttpStatusCode.OK);
        Assert.IsAssignableFrom<IEnumerable<WorkUnitDto>>(result);
    }
}