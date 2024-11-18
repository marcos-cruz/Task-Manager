using System.Net;

using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Queries.GetAllProjectsByUserId;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;
using Bigai.TaskManager.Domain.Tests.Helpers;
using Bigai.TaskManager.Infrastructure.Projects.Services;

using FluentAssertions;

using Moq;

namespace Bigai.TaskManager.Application.Tests.Projects.Queries.GetAllProjectsByUserId;

public class GetAllProjectsByUserIdQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _projectsRepositoryMock;
    private readonly GetAllProjectsByUserIdQueryHandler _queryHandler;
    private readonly IBussinessNotificationsHandler _notificationsHandler;

    public GetAllProjectsByUserIdQueryHandlerTests()
    {
        _projectsRepositoryMock = new Mock<IProjectRepository>();
        _notificationsHandler = new BussinessNotificationsHandler();
        _queryHandler = new GetAllProjectsByUserIdQueryHandler(_projectsRepositoryMock.Object, _notificationsHandler);
    }

    [Fact()]
    public async Task Handle_WithUserWithProjects_ReturnsUsersProjects()
    {
        // arrange
        int registeredUser = 1001;
        int amountProjects = 15;
        var query = new GetAllProjectsByUserIdQuery(registeredUser);
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(amountProjects, registeredUser);

        _projectsRepositoryMock
            .Setup(repo => repo.GetProjectsByUserIdAsync(registeredUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        // act
        IEnumerable<ProjectDto> result = await _queryHandler.Handle(query, CancellationToken.None);

        // assert
        result.Should().NotBeNullOrEmpty();
        _notificationsHandler.StatusCode.Should().Be(HttpStatusCode.OK);
        Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(result);
    }
}