using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Queries.GetAllProjectsByUserId;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Tests.Helpers;

using FluentAssertions;

using Moq;

namespace Bigai.TaskManager.Application.Tests.Projects.Queries;

public class GetAllProjectsByUserIdQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _projectsRepositoryMock;
    private readonly GetAllProjectsByUserIdQueryHandler _queryHandler;

    public GetAllProjectsByUserIdQueryHandlerTests()
    {
        _projectsRepositoryMock = new Mock<IProjectRepository>();
        _queryHandler = new GetAllProjectsByUserIdQueryHandler(_projectsRepositoryMock.Object);
    }

    [Fact()]
    public async Task Handle_WithUserWithProjects_ReturnsProjects()
    {
        // arrange
        int registeredUser = 1001;
        var query = new GetAllProjectsByUserIdQuery(registeredUser);
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(15, registeredUser);

        _projectsRepositoryMock
            .Setup(repo => repo.GetProjectsByUserIdAsync(registeredUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        // act
        IEnumerable<ProjectDto> result = await _queryHandler.Handle(query, CancellationToken.None);

        // assert
        result.Should().NotBeNullOrEmpty();
        Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(result);
    }
}