using Bigai.TaskManager.Application.Projects.Commands.CreateWorkUnit;
using Bigai.TaskManager.Application.Projects.Mappers;
using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;

using FluentAssertions;

namespace Bigai.TaskManager.Application.Tests.Projects.Mappers;

public class WorkUnittMapperTests
{
    [Fact()]
    public void AsDto_FromWorkUnitToWorkUnitDto_MapsCorrectly()
    {
        // arrange
        var workUnit = WorkUnit.Create("Work unit title", "Work unit description", DateTime.Now.AddDays(15), Priority.Average);

        // act
        var workUnitDto = workUnit.AsDto();

        // assert
        workUnitDto.Should().NotBeNull();
        workUnitDto.Title.Should().Be(workUnit.Title);
        workUnitDto.Description.Should().Be(workUnit.Description);
        workUnitDto.DueDate.Should().Be(workUnit.DueDate);
        workUnitDto.Priority.Should().Be(workUnit.Priority);
    }

    [Fact()]
    public void AsEntity_FromCreateWorkUnitCommandToWorkUnit_MapsCorrectly()
    {
        // arrange
        var dueDate = DateTime.Now.AddDays(25);
        var command = new CreateWorkUnitCommand
        {
            ProjectId = 15,
            Title = "test create work unit",
            Description = "Details about this work unit",
            DueDate = dueDate,
            Priority = Priority.Average
        };

        // act
        var workUnit = command.AsEntity();

        // assert
        workUnit.Should().NotBeNull();
        workUnit.ProjectId.Should().Be(command.ProjectId);
        workUnit.Title.Should().Be(command.Title);
        workUnit.Description.Should().Be(command.Description);
        workUnit.DueDate.Should().Be(dueDate);
        workUnit.Priority.Should().Be(command.Priority);
    }

}