using ErrorOr;
using MediatR;
using Task3.Application.Coins.Responses;

namespace Task3.Application.Coins.Queries;

public class CoinWithLongestHistoryQuery : IRequest<ErrorOr<CoinWithLongestHistoryResponse>>
{
    
}
