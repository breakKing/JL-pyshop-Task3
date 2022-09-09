using ErrorOr;
using MediatR;
using Task3.Application.Coins.Responses;

namespace Task3.Application.Coins.Commands;

public class MoveCoinsCommand : IRequest<ErrorOr<MoveCoinsResponse>>
{
    public long SrcUserId { get; set; }
    public long DstUserId { get; set; }
    public long Amount { get; set; }

    public MoveCoinsCommand(long srcUserId, long dstUserId, long amount)
    {
        SrcUserId = srcUserId;
        DstUserId = dstUserId;
        Amount = amount;
    }
}
