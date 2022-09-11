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
using Task3.Application.Tests.Unit.Common.Data;
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
        var existentUserName = srcUserDoesntExist ? "dstUserName" : "srcUserName";
        var existentUser = new User
        {
            Id = 1,
            Name = existentUserName
        };

        var usersRepository = CreateMockedUsersRepositoryWithDefinedUsers(existentUser);
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

    [Theory]
    [InlineData(10, 23)]
    [InlineData(5, 100)]
    [InlineData(0, 1)]
    public async Task TransferService_ShouldReturnErrorFailure_WhenSrcUserHasInsufficientCoins(long srcUserAmount, long amountToMove)
    {
        // Assign
        var srcUser = UsersDataGenerator.CreateUserWithCoins(1, "srcUserName", srcUserAmount);
        var dstUser = UsersDataGenerator.CreateUserWithCoins(2, "dstUserName", 0, srcUserAmount + 1);

        var usersRepository = CreateMockedUsersRepositoryWithDefinedUsers(srcUser, dstUser);
        var coinsRepository = Substitute.For<ICoinsRepository>();
        var mapper = Substitute.For<IMapper>();

        ICoinsTransferService transferService = new CoinsTransferService(
            usersRepository,
            coinsRepository,
            mapper
        );
        
        // Act
        var result = await transferService.MoveCoinsAsync("srcUserName", "dstUserName", amountToMove);
        
        // Assert
        srcUserAmount.Should().BeLessThan(amountToMove);

        result.Should().BeOfType<ErrorOr<bool>>()
            .Which.IsError.Should().BeTrue();
        
        result.Errors.Should().HaveCount(1);

        result.Errors[0].Should().BeOfType<Error>()
            .Which.Type.Should().Be(ErrorType.Failure);
    }

    [Theory]
    [InlineData(5, 0, 3)]
    [InlineData(100, 1000, 100)]
    [InlineData(500, 13, 1)]
    public async Task TransferService_ShouldMoveCoinsCorrectly_WhenCoinsCanBeMoved(
        long srcUserAmount,
        long dstUserAmount,
        long amountToMove)
    {
        // Assign
        var srcUser = UsersDataGenerator.CreateUserWithCoins(1, "srcUserName", srcUserAmount);
        var dstUser = UsersDataGenerator.CreateUserWithCoins(2, "dstUserName", dstUserAmount, srcUserAmount + 1);

        var coins = new List<Coin>();
        coins.AddRange(srcUser.Coins);
        coins.AddRange(dstUser.Coins);

        var moves = new List<Move>();

        var usersRepository = CreateMockedUsersRepositoryWithDefinedUsers(srcUser, dstUser);
        var coinsRepository = CreateMockedCoinsRepository(coins, moves, srcUser, dstUser);
        var mapper = Substitute.For<IMapper>();

        ICoinsTransferService transferService = new CoinsTransferService(
            usersRepository,
            coinsRepository,
            mapper
        );
        
        // Act
        var result = await transferService.MoveCoinsAsync("srcUserName", "dstUserName", amountToMove);
        
        // Assert
        srcUserAmount.Should().BeGreaterThanOrEqualTo(amountToMove);

        result.Should().BeOfType<ErrorOr<bool>>()
            .Which.IsError.Should().BeFalse();

        moves.Should().HaveCount((int)amountToMove);
        
        coins.Where(c => c.UserId == srcUser.Id)
            .Should().HaveCount((int)(srcUserAmount - amountToMove));

        coins.Where(c => c.UserId == dstUser.Id)
            .Should().HaveCount((int)(dstUserAmount + amountToMove));

        srcUser.Amount.Should().Be(srcUserAmount - amountToMove);

        dstUser.Amount.Should().Be(dstUserAmount + amountToMove);
    }

    private static IUsersRepository CreateMockedUsersRepositoryWithDefinedUsers(params User[] users)
    {
        var repository = Substitute.For<IUsersRepository>();
        repository.GetOneWithCoinsAsync(string.Empty)
            .ReturnsForAnyArgs(null as User);

        for (var i = 0; i < users.LongLength; i++)
        {
            repository.GetOneWithCoinsAsync(users[i].Name)
                .Returns(users[i]);
        }
        
        return repository;
    }

    private static ICoinsRepository CreateMockedCoinsRepository(
        List<Coin> coins,
        List<Move> moves,
        User srcUser,
        User dstUser)
    {
        var repository = Substitute.For<ICoinsRepository>();
        repository.AddMovesAsync(default, default, default)
            .ReturnsForAnyArgs(true)
            .AndDoes(c =>
            {
                var srcUserId = c.ArgAt<long>(0);
                var dstUserId = c.ArgAt<long>(1);
                var amount = c.ArgAt<long>(2);

                long moved = 0;
                for (var i = 0; i < coins.LongCount(); i++)
                {
                    if (moved == amount)
                    {
                        break;
                    }

                    if (coins[i].UserId != srcUserId)
                    {
                        continue;
                    }

                    srcUser.Coins.Remove(coins[i]);
                    coins[i].UserId = dstUserId;
                    dstUser.Coins.Add(coins[i]);
                    
                    var move = new Move
                    {
                        SrcUserId = srcUserId,
                        DstUserId = dstUserId,
                        UnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                        CoinId = coins[i].Id
                    };
                    moves.Add(move);
                    moved++;
                }
            });

        return repository;
    }
}
