using Task3.Application.Coins.Responses;
using Task3.Application.Common.Interfaces.MediatR;

namespace Task3.Application.Coins.Commands;

public class CoinsEmissionCommand : IResultRequest<CoinsEmissionResponse>
{
    public long Amount { get; set; }
}
