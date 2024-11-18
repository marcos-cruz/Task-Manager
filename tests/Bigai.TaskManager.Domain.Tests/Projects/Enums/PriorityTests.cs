using Bigai.TaskManager.Domain.Projects.Enums;

using FluentAssertions;

namespace Bigai.TaskManager.Domain.Tests.Projects.Enums;

public class PriorityTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Priority_MustBe_Valid_Enum_True(int value)
    {
        // arrange
        bool isValid;

        // act
        isValid = Enum.IsDefined(typeof(Priority), value);

        // assert
        isValid.Should().Be(true);
    }

    [Fact]
    public void Priority_MustBe_Valid_Enum_False()
    {
        // arrange
        bool isValid;

        // act
        isValid = Enum.IsDefined(typeof(Priority), 3);

        // assert
        isValid.Should().Be(false);
    }
}