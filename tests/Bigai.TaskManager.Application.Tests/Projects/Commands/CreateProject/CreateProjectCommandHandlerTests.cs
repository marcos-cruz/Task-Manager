using System.Net;

using Bigai.TaskManager.Application.Projects.Commands.CreateProject;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Infrastructure.Projects.Services;

using FluentAssertions;

using Moq;

namespace Bigai.TaskManager.Application.Tests.Projects.Commands.CreateProject;

public class CreateProjectCommandHandlerTests
{
    [Fact()]
    public async Task Handle_ForValidCommand_ReturnsCreatedProjectId()
    {
        // arrange
        var projectRepositoryMock = new Mock<IProjectRepository>();
        projectRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Project>(), CancellationToken.None))
            .ReturnsAsync(1);

        var command = new CreateProjectCommand();

        var notificationHandler = new BussinessNotificationsHandler();

        var commandHandler = new CreateProjectCommandHandler(projectRepositoryMock.Object, notificationHandler);

        // act
        var projectId = await commandHandler.Handle(command, CancellationToken.None);

        // assert
        projectId.Should().Be(1);
        notificationHandler.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}