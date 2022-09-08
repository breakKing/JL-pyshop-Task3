using LanguageExt.Common;
using Task3.Application.Coins.Commands;
using Task3.Application.Coins.Responses;
using Task3.Application.Common.Interfaces.MediatR;
using Task3.Application.Common.Interfaces.Services;

namespace Task3.Application.Coins.Handlers;

public class CoinsEmissionCommandHandler : IResultRequestHandler<CoinsEmissionCommand, CoinsEmissionResponse>
{
    private readonly IEmissionService _emissionService;

    public CoinsEmissionCommandHandler(IEmissionService emissionService)
    {
        _emissionService = emissionService;
    }

    public async Task<Result<CoinsEmissionResponse>> Handle(CoinsEmissionCommand request, CancellationToken ct)
    {
        var emissionResult = await _emissionService.MakeEmissionAsync(request.Amount, ct);

        var response = emissionResult.Match(e =>
        {
            return new Result<CoinsEmissionResponse>(new CoinsEmissionResponse());
        },
        ex =>
        {
            return new Result<CoinsEmissionResponse>(ex);
        });

        return response;
    }
}
