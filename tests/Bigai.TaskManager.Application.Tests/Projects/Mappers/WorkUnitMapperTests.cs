using Bigai.TaskManager.Application.Projects.Commands.CreateProject;
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
        var workUnit = WorkUnit.Create("Work unit title", "Work unit description", new DateTimeOffset().AddDays(15), Priority.Average);

        // act
        var workUnitDto = workUnit.AsDto();

        // assert
        workUnitDto.Should().NotBeNull();
        workUnitDto.Title.Should().Be(workUnit.Title);
        workUnitDto.Description.Should().Be(workUnit.Description);
        workUnitDto.DueDate.Should().Be(workUnit.DueDate);
        workUnitDto.Priority.Should().Be(workUnit.Priority);
    }
}