using Bigai.TaskManager.Application.Projects.Commands.CreateProject;
using Bigai.TaskManager.Application.Projects.Mappers;
using Bigai.TaskManager.Domain.Projects.Models;

using FluentAssertions;

namespace Bigai.TaskManager.Application.Tests.Projects.Mappers;

public class ProjectMapperTests
{
    [Fact()]
    public void AsDto_FromProjectToProjectDto_MapsCorrectly()
    {
        // arrange
        var project = Project.Create("test project");

        // act
        var projectDto = project.AsDto();

        // assert
        projectDto.Should().NotBeNull();
        projectDto.ProjectId.Should().Be(project.Id);
        projectDto.Name.Should().Be(project.Name);
        projectDto.WorkUnits.Should().NotBeNull();
    }

    [Fact()]
    public void AsEntity_FromCreateProjectCommandToProject_MapsCorrectly()
    {
        // arrange
        var command = new CreateProjectCommand
        {
            Name = "test create project"
        };

        // act
        var project = command.AsEntity();

        // assert
        project.Should().NotBeNull();
        project.Name.Should().Be(command.Name);
    }
}