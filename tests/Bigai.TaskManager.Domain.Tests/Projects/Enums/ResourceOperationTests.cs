using Bigai.TaskManager.Domain.Projects.Enums;

using FluentAssertions;

namespace Bigai.TaskManager.Domain.Tests.Projects.Enums;

public class ResourceOperationTests
{
    [Theory]
    [InlineData(0)]
    public void ResourceOperation_MustBe_Valid_Enum_True(int value)
    {
        // arrange
        bool isValid;

        // act
        isValid = Enum.IsDefined(typeof(ResourceOperation), value);

        // assert
        isValid.Should().Be(true);
    }

    [Fact]
    public void ResourceOperation_MustBe_Valid_Enum_False()
    {
        // arrange
        bool isValid;

        // act
        isValid = Enum.IsDefined(typeof(ResourceOperation), 3);

        // assert
        isValid.Should().Be(false);
    }
}