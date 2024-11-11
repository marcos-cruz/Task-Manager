using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Queries.GetProjectById;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Tests.Helpers;

using FluentAssertions;

using Moq;

namespace Bigai.TaskManager.Application.Tests.Projects.Queries;

public class GetProjectByIdQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _projectsRepositoryMock;
    private readonly GetProjectByIdQueryHandler _queryHandler;

    public GetProjectByIdQueryHandlerTests()
    {
        _projectsRepositoryMock = new Mock<IProjectRepository>();
        _queryHandler = new GetProjectByIdQueryHandler(_projectsRepositoryMock.Object);
    }

    [Fact()]
    public async Task Handle_WithExistingProjectId_ReturnsProject()
    {
        // arrange
        int registeredUser = 1001;
        var query = new GetProjectByIdQuery(registeredUser);
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(15, registeredUser);

        _projectsRepositoryMock
            .Setup(repo => repo.GetProjectByIdAsync(registeredUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects.First());

        // act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // assert
        result.Should().NotBeNull();
        Assert.IsAssignableFrom<ProjectDto>(result);
    }

    [Fact()]
    public async Task Handle_WithNonExistingProjectId_ReturnsNUll()
    {
        // arrange
        int registeredUser = 1001;
        var query = new GetProjectByIdQuery(registeredUser);
        Project? project = null;

        _projectsRepositoryMock
            .Setup(repo => repo.GetProjectByIdAsync(registeredUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // assert
        result.Should().BeNull();
    }

}