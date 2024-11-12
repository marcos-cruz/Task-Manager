using Bigai.TaskManager.Application.Projects.Commands.RemoveProject;
using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;
using Bigai.TaskManager.Domain.Tests.Helpers;

using FluentAssertions;

using Moq;

namespace Bigai.TaskManager.Application.Tests.Projects.Commands.RemoveProject;

public class RemoveProjectByIdCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ReturnsNUll()
    {
        // arrange
        Project? project = null;
        int projectId = 1001;
        var projectRepositoryMock = new Mock<IProjectRepository>();
        var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();

        projectRepositoryMock
            .Setup(repo => repo.GetProjectByIdAsync(projectId, CancellationToken.None))
            .ReturnsAsync(project);

        var command = new RemoveProjectByIdCommand(projectId);

        var commandHandler = new RemoveProjectByIdCommandHandler(projectRepositoryMock.Object, projectAuthorizationServiceMock.Object);

        // act
        var removed = await commandHandler.Handle(command, CancellationToken.None);

        // assert
        removed.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenProjectExistsAndHasPendingStatus_ReturnsFalse()
    {
        // arrange
        int projectId = 1001;
        var projectRepositoryMock = new Mock<IProjectRepository>();
        var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(1, 101);
        Project project = projects.First();


        projectRepositoryMock
            .Setup(repo => repo.GetProjectByIdAsync(projectId, CancellationToken.None))
            .ReturnsAsync(project);

        projectAuthorizationServiceMock
            .Setup(service => service.Authorize(project, ResourceOperation.Remove))
            .Returns(false);

        var command = new RemoveProjectByIdCommand(projectId);

        var commandHandler = new RemoveProjectByIdCommandHandler(projectRepositoryMock.Object, projectAuthorizationServiceMock.Object);

        // act
        var removed = await commandHandler.Handle(command, CancellationToken.None);

        // assert
        removed.Should().Be(false);
    }

    [Fact]
    public async Task Handle_WhenProjectExistsAndDoesNotHavePendingStatus_ReturnsTrue()
    {
        // arrange
        int projectId = 1001;
        int amountProjects = 1;
        int userId = 101;
        var projectRepositoryMock = new Mock<IProjectRepository>();
        var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(amountProjects, userId);
        Project project = projects.First();


        projectRepositoryMock
            .Setup(repo => repo.GetProjectByIdAsync(projectId, CancellationToken.None))
            .ReturnsAsync(project);

        projectAuthorizationServiceMock
            .Setup(service => service.Authorize(project, ResourceOperation.Remove))
            .Returns(true);

        var command = new RemoveProjectByIdCommand(projectId);

        var commandHandler = new RemoveProjectByIdCommandHandler(projectRepositoryMock.Object, projectAuthorizationServiceMock.Object);

        // act
        var removed = await commandHandler.Handle(command, CancellationToken.None);

        // assert
        removed.Should().Be(true);
    }
}