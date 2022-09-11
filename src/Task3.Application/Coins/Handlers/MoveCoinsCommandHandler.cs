using ErrorOr;
using MediatR;
using Task3.Application.Coins.Commands;
using Task3.Application.Coins.Responses;
using Task3.Application.Common.Interfaces.Services;

namespace Task3.Application.Coins.Handlers;

public class MoveCoinsCommandHandler : IRequestHandler<MoveCoinsCommand, ErrorOr<MoveCoinsResponse>>
{
    private readonly ICoinsTransferService _coinsTransferService;

    public MoveCoinsCommandHandler(ICoinsTransferService coinsTransferService)
    {
        _coinsTransferService = coinsTransferService;
    }

    public async Task<ErrorOr<MoveCoinsResponse>> Handle(
        MoveCoinsCommand request,
        CancellationToken ct)
    {
        var moveResult = await _coinsTransferService.MoveCoinsAsync(
            request.SrcUserName,
            request.DstUserName,
            request.Amount,
            ct);

        if (moveResult.IsError)
        {
            return moveResult.Errors;
        }

        return new MoveCoinsResponse(); 
    }
}
