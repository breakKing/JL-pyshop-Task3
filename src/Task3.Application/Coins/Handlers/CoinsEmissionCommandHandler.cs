using ErrorOr;
using MediatR;
using Task3.Application.Coins.Commands;
using Task3.Application.Coins.Responses;
using Task3.Application.Common.Interfaces.Services;

namespace Task3.Application.Coins.Handlers;

public class CoinsEmissionCommandHandler : IRequestHandler<CoinsEmissionCommand, ErrorOr<CoinsEmissionResponse>>
{
    private readonly IEmissionService _emissionService;

    public CoinsEmissionCommandHandler(IEmissionService emissionService)
    {
        _emissionService = emissionService;
    }

    public async Task<ErrorOr<CoinsEmissionResponse>> Handle(CoinsEmissionCommand request, CancellationToken ct)
    {
        var emissionResult = await _emissionService.MakeEmissionAsync(request.Amount, ct);

        if (emissionResult.IsError)
        {
            return emissionResult.Errors;
        }

        return new CoinsEmissionResponse();
    }
}
