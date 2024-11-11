using Bigai.TaskManager.Application.Projects.Mappers;
using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;

using FluentAssertions;

namespace Bigai.TaskManager.Application.Tests.Projects.Mappers;

public class ProjectMapperTests
{
    [Fact()]
    public void AsDto_FromProjectToProjectDto_MapsCorrectly()
    {
        // arrange
        var project = Project.Create(1001, "test project", Priority.High);

        // act
        var projectDto = project.AsDto();

        // assert
        projectDto.Should().NotBeNull();
        projectDto.ProjectId.Should().Be(project.Id);
        projectDto.UserId.Should().Be(project.UserId);
        projectDto.Name.Should().Be(project.Name);
        projectDto.Priority.Should().Be(project.Priority);
    }
}