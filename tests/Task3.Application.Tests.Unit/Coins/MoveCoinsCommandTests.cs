using ErrorOr;
using FluentAssertions;
using FluentValidation.Results;
using MapsterMapper;
using NSubstitute;
using Task3.Application.Coins.Commands;
using Task3.Application.Coins.Services;
using Task3.Application.Coins.Validators;
using Task3.Application.Common.Interfaces.Repositories;
using Task3.Application.Common.Interfaces.Services;
using Task3.Domain.Entities;

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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TransferService_ShouldReturnErrorFailure_WhenOneOfUsersDoesntExist(bool srcUserDoesntExist)
    {
        // Assign
        var nonExistentUserName = srcUserDoesntExist ? "srcUserName" : "dstUserName";
        var existentUserName = !srcUserDoesntExist ? "srcUserName" : "dstUserName";

        var usersRepository = CreateMockedUserRepositoryWithoutDefinedUser(nonExistentUserName, existentUserName);
        var coinsRepository = Substitute.For<ICoinsRepository>();
        var mapper = Substitute.For<IMapper>();

        ICoinsTransferService transferService = new CoinsTransferService(
            usersRepository,
            coinsRepository,
            mapper
        );
        
        // Act
        var result = await transferService.MoveCoinsAsync("srcUserName", "dstUserName", _random.NextInt64(1, 1000000));
        
        // Assert
        result.Should().BeOfType<ErrorOr<bool>>()
            .Which.IsError.Should().BeTrue();
        
        result.Errors.Should().HaveCount(1);

        result.Errors[0].Should().BeOfType<Error>()
            .Which.Type.Should().Be(ErrorType.NotFound);
    }

    private static IUsersRepository CreateMockedUserRepositoryWithoutDefinedUser(string nonExistentUserName,
        string existentUserName = "existentUserName")
    {
        User existentUser = new()
        {
            Id = 1,
            Name = existentUserName
        };

        var repository = Substitute.For<IUsersRepository>();

        repository.GetOneWithCoinsAsync("")
            .ReturnsForAnyArgs(existentUser);
        repository.GetOneWithCoinsAsync(nonExistentUserName)
            .Returns(null as User);
        
        return repository;
    }
}
