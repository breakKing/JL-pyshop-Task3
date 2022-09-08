using ErrorOr;
using MediatR;
using Task3.Application.Coins.Responses;

namespace Task3.Application.Coins.Commands;

public class CoinsEmissionCommand : IRequest<ErrorOr<CoinsEmissionResponse>>
{
    public long Amount { get; set; }

    public CoinsEmissionCommand(long amount)
    {
        Amount = amount;
    }
}
