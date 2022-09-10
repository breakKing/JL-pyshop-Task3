using ErrorOr;
using MediatR;
using Task3.Application.Coins.Responses;

namespace Task3.Application.Coins.Commands;

public class MoveCoinsCommand : IRequest<ErrorOr<MoveCoinsResponse>>
{
    public string SrcUserName { get; set; } = string.Empty;
    public string DstUserName { get; set; } = string.Empty;
    public long Amount { get; set; }

    public MoveCoinsCommand(string srcUserName, string dstUserName, long amount)
    {
        SrcUserName = srcUserName;
        DstUserName = dstUserName;
        Amount = amount;
    }
}
