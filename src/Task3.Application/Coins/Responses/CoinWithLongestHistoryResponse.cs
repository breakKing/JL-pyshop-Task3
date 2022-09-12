using Task3.Application.Coins.Dtos;

namespace Task3.Application.Coins.Responses;

public class CoinWithLongestHistoryResponse
{
    public CoinWithHistoryDto Coin { get; set; } = default!;
}
