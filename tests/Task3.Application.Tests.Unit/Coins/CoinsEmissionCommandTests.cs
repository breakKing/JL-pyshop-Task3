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
using Task3.Application.Users.Dtos;
using Task3.Domain.Entities;

namespace Task3.Application.Tests.Unit.Coins;

public class CoinsEmissionCommandTests
{
    private readonly Random _random;

    public CoinsEmissionCommandTests()
    {
        _random = new Random(12345);
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
        var validator = new CoinsEmissionCommandValidator();
        var command = new CoinsEmissionCommand(amount);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.Should().BeOfType<ValidationResult>()
            .Which.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(4, 1)]
    [InlineData(4, 2)]
    [InlineData(4, 3)]
    public async Task EmissionService_ShouldReturnErrorFailure_WhenUsersCountIsGreaterThanCoinsAmount(
        long usersCount,
        long coinsAmount)
    {
        // Assign
        var users = new List<UserRatingDto>();
        for (var i = 0; i < usersCount; i++)
        {
            users.Add(new UserRatingDto
            {
                Id = i + 1,
                Rating = (i + 1) * 100
            });
        }

        var usersRepository = Substitute.For<IUsersRepository>();
        usersRepository.GetAllWithSortedRatingAsync<UserRatingDto>(u => new UserRatingDto())
            .ReturnsForAnyArgs(users);

        var coinsRepository = Substitute.For<ICoinsRepository>();

        var mapper = Substitute.For<IMapper>();

        IEmissionService emissionService = new EmissionService(
            coinsRepository,
            usersRepository,
            mapper);

        // Act
        var result = await emissionService.MakeEmissionAsync(coinsAmount);

        // Assert
        usersCount.Should().BeGreaterThan(coinsAmount);

        result.Should().BeOfType<ErrorOr<bool>>()
            .Which.IsError.Should().BeTrue();

        result.Errors.Should().HaveCount(1);

        result.Errors[0].Should().BeOfType<Error>()
            .Which.Type.Should().Be(ErrorType.Failure);
    }

    [Theory]
    [InlineData(10, 100)]
    [InlineData(4, 6)]
    [InlineData(2, 3)]
    [InlineData(1, 1)]
    public async Task EmissionService_ShouldDistributeAllTheCoins_WhenCoinsAmountIsGreaterOrEqualToUsersCount(
        long usersCount,
        long coinsAmount
    )
    {
        // Assign
        var users = new List<UserRatingDto>();
        for (var i = 0; i < usersCount; i++)
        {
            users.Add(new UserRatingDto
            {
                Id = i + 1,
                Rating = _random.Next(1, 10000)
            });
        }

        var coins = new List<Coin>();
        var moves = new List<Move>();

        var usersRepository = Substitute.For<IUsersRepository>();
        usersRepository.GetAllWithSortedRatingAsync<UserRatingDto>(u => new UserRatingDto())
            .ReturnsForAnyArgs(users);

        var coinsRepository = Substitute.For<ICoinsRepository>();
        coinsRepository.AddAsync(new Coin())
            .ReturnsForAnyArgs(coins.LongCount() + 1)
            .AndDoes(c =>
            {
                var coin = c.Arg<Coin>();
                coins.Add(coin);
                moves.Add(new Move
                {
                    CoinId = coins.LongCount(),
                    UnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    SrcUserId = null,
                    DstUserId = coin.UserId
                });
            });

        var mapper = Substitute.For<IMapper>();

        IEmissionService emissionService = new EmissionService(
            coinsRepository,
            usersRepository,
            mapper);

        // Act
        var result = await emissionService.MakeEmissionAsync(coinsAmount);

        // Assert
        usersCount.Should().BeLessThanOrEqualTo(coinsAmount);

        result.Should().BeOfType<ErrorOr<bool>>()
            .Which.IsError.Should().BeFalse();

        var distributedAmount = coins.LongCount();
        distributedAmount.Should().Be(coinsAmount);
    }
}
