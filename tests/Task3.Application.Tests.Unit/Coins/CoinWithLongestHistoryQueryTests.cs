using ErrorOr;
using FluentAssertions;
using MapsterMapper;
using NSubstitute;
using Task3.Application.Coins.Dtos;
using Task3.Application.Coins.Handlers;
using Task3.Application.Coins.Queries;
using Task3.Application.Coins.Responses;
using Task3.Application.Common.Interfaces.Repositories;

namespace Task3.Application.Tests.Unit.Coins;

public class CoinWithLongestHistoryQueryTests
{
    [Fact]
    public async Task Handler_ShouldReturnErrorNotFound_WhenThereIsNoCoins()
    {
        // Assign
        var coinsRepository = Substitute.For<ICoinsRepository>();
        var mapper = Substitute.For<IMapper>();

        coinsRepository.GetCoinWithLongestHistoryOrDefaultAsync(
            c => mapper.Map<CoinWithHistoryDto>(c)
        ).ReturnsForAnyArgs(null as CoinWithHistoryDto);

        var query = new CoinWithLongestHistoryQuery();
        var handler = new CoinWithLongestHistoryQueryHandler(
            coinsRepository,
            mapper);
        var ctSource = new CancellationTokenSource();
        
        // Act
        var response = await handler.Handle(query, ctSource.Token);
        
        // Assert
        response.Should().BeOfType<ErrorOr<CoinWithLongestHistoryResponse>>()
            .Which.IsError.Should().BeTrue();
        
        response.Errors.Should().HaveCount(1);

        response.Errors[0].Should().BeOfType<Error>()
            .Which.Type.Should().Be(ErrorType.NotFound);
    }
}
