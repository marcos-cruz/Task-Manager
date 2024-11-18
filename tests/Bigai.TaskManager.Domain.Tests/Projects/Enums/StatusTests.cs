using Bigai.TaskManager.Domain.Projects.Enums;

using FluentAssertions;

namespace Bigai.TaskManager.Domain.Tests.Projects.Enums;

public class StatusTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Status_MustBe_Valid_Enum_True(int value)
    {
        // arrange
        bool isValid;

        // act
        isValid = Enum.IsDefined(typeof(Status), value);

        // assert
        isValid.Should().Be(true);
    }

    [Fact]
    public void Status_MustBe_Valid_Enum_False()
    {
        // arrange
        bool isValid;

        // act
        isValid = Enum.IsDefined(typeof(Status), 3);

        // assert
        isValid.Should().Be(false);
    }
}