using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bigai.TaskManager.Application.Projects.Commands.CreateWorkUnit;
using Bigai.TaskManager.Domain.Projects.Constants;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;
using Bigai.TaskManager.Domain.Tests.Helpers;

using FluentAssertions;

using Moq;

namespace Bigai.TaskManager.Application.Tests.Projects.Commands.CreateWorkUnit;

public class CreateWorkUnitCommandHandlerTests
{
    [Fact()]
    public async Task Handle_ForValidCommand_ReturnsCreatedWorkUnitId()
    {
        // arrange
        int amountProjects = 1;
        int userId = 101;
        int projectId = 1001;
        var projectRepositoryMock = new Mock<IProjectRepository>();
        IReadOnlyCollection<Project> projects = ProjectHelper.GetProjects(amountProjects, userId);
        Project project = projects.First();

        projectRepositoryMock
            .Setup(repo => repo.GetProjectByIdAsync(projectId, CancellationToken.None))
            .ReturnsAsync(project);

        projectRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<WorkUnit>(), CancellationToken.None))
            .ReturnsAsync(0);

        var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();
        projectAuthorizationServiceMock
            .Setup(service => service.AuthorizeLimit(project, ProjectRoles.MaximumTaskLimit))
            .Returns(true);

        var command = new CreateWorkUnitCommand();

        var commandHandler = new CreateWorkUnitCommandHandler(projectRepositoryMock.Object, projectAuthorizationServiceMock.Object);

        // act
        var workUnitId = await commandHandler.Handle(command, CancellationToken.None);

        // assert
        workUnitId.Should().Be(0);
    }

}