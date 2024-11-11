using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Queries.GetAllProjectsByUserId;
using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;

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
    public async System.Threading.Tasks.Task Handle_WithUserWithProjects_ReturnsProjects()
    {
        // arrange
        int registeredUser = 1001;
        var query = new GetAllProjectsByUserIdQuery(registeredUser);
        IReadOnlyCollection<Project> projects = new List<Project>
        {
            Project.Create(registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.Average),
            Project.Create(registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.High),
            Project.Create(registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.Low),
            Project.Create(registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.Average),
            Project.Create(registeredUser, $"Test Project {Guid.NewGuid().ToString()}", Priority.High),
        };

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