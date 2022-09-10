using FluentAssertions;
using FluentValidation.Results;
using Task3.Application.Coins.Commands;
using Task3.Application.Coins.Validators;

namespace Task3.Application.Tests.Unit.Coins;

public class MoveCoinsCommandTests
{
    private readonly Random _random;

    public MoveCoinsCommandTests()
    {
        _random = new Random(12345);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\t")]
    public void Validator_ShouldReturnValidationError_WhenSrcUserIsEmpty(string srcUserName)
    {
        // Assign
        var validator = new MoveCoinsCommandValidator();
        var command = new MoveCoinsCommand(srcUserName, "dstUserName", _random.NextInt64(1, 100000000));
        
        var result = validator.Validate(command);

        // Assert
        result.Should().BeOfType<ValidationResult>()
            .Which.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\t")]
    public void Validator_ShouldReturnValidationError_WhenDstUserIsEmpty(string dstUserName)
    {
        // Assign
        var validator = new MoveCoinsCommandValidator();
        var command = new MoveCoinsCommand("srcUserName", dstUserName, _random.NextInt64(1, 100000000));
        
        var result = validator.Validate(command);

        // Assert
        result.Should().BeOfType<ValidationResult>()
            .Which.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(-100)]
    [InlineData(long.MinValue)]
    public void Validator_ShouldReturnValidationError_WhenAmountIsLessOrEqualToZero(long amount)
    {
        // Assign
        var validator = new MoveCoinsCommandValidator();
        var command = new MoveCoinsCommand("srcUserName", "dstUserName", amount);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.Should().BeOfType<ValidationResult>()
            .Which.IsValid.Should().BeFalse();
    }
}
