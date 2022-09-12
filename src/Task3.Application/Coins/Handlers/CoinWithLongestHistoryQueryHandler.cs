using ErrorOr;
using MapsterMapper;
using MediatR;
using Task3.Application.Coins.Dtos;
using Task3.Application.Coins.Queries;
using Task3.Application.Coins.Responses;
using Task3.Application.Common.Interfaces.Repositories;

namespace Task3.Application.Coins.Handlers;

public class CoinWithLongestHistoryQueryHandler :
    IRequestHandler<CoinWithLongestHistoryQuery, ErrorOr<CoinWithLongestHistoryResponse>>
{
    private readonly string NO_COINS_YET_MESSAGE = "There is no coins in the system yet";

    private readonly ICoinsRepository _coinsRepository;
    private readonly IMapper _mapper;

    public CoinWithLongestHistoryQueryHandler(
        ICoinsRepository coinsRepository,
        IMapper mapper)
    {
        _coinsRepository = coinsRepository;
        _mapper = mapper;
    }

    public async Task<ErrorOr<CoinWithLongestHistoryResponse>> Handle(
        CoinWithLongestHistoryQuery request,
        CancellationToken ct)
    {
        var coin = await _coinsRepository.GetCoinWithLongestHistoryOrDefaultAsync(
            c => _mapper.Map<CoinWithHistoryDto>(c),
            ct
        );

        if (coin is null)
        {
            return Error.NotFound("Coin.NotFound",
                NO_COINS_YET_MESSAGE);
        }

        return new CoinWithLongestHistoryResponse
        {
            Coin = coin
        };
    }
}
