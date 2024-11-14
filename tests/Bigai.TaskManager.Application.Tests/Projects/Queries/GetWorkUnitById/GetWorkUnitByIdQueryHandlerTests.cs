using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Queries.GetWorkUnitById;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Tests.Helpers;

using FluentAssertions;

using Moq;

namespace Bigai.TaskManager.Application.Tests.Projects.Queries.GetWorkUnitById;

public class GetWorkUnitByIdQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _projectsRepositoryMock;
    private readonly GetWorkUnitByIdQueryHandler _queryHandler;

    public GetWorkUnitByIdQueryHandlerTests()
    {
        _projectsRepositoryMock = new Mock<IProjectRepository>();
        _queryHandler = new GetWorkUnitByIdQueryHandler(_projectsRepositoryMock.Object);
    }

    [Fact()]
    public async Task Handle_WithExistingWorkUnitId_ReturnsWorkUnit()
    {
        // arrange
        int userId = 1010101;
        int amountProjects = 1;
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(amountProjects, userId);
        WorkUnit workUnit = projects.First().WorkUnits.ToArray()[0];
        int projectId = workUnit.ProjectId.HasValue ? workUnit.ProjectId.Value : 0;
        var query = new GetWorkUnitByIdQuery(projectId, workUnit.Id);

        _projectsRepositoryMock
            .Setup(repo => repo.GetWorkUnitByIdAsync(projectId, workUnit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workUnit);

        // act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // assert
        result.Should().NotBeNull();
        Assert.IsAssignableFrom<WorkUnitDto>(result);
    }

    [Fact()]
    public async Task Handle_WithNonExistingWorkUnitId_ReturnsNUll()
    {
        // arrange
        int projectId = 1001;
        int workUnitId = 1001;
        var query = new GetWorkUnitByIdQuery(projectId, workUnitId);
        WorkUnit? workUnit = null;

        _projectsRepositoryMock
            .Setup(repo => repo.GetWorkUnitByIdAsync(projectId, workUnitId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workUnit);

        // act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // assert
        result.Should().BeNull();
    }
}