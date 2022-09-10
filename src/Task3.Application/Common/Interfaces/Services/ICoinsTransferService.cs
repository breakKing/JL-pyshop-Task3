using ErrorOr;

namespace Task3.Application.Common.Interfaces.Services;

public interface ICoinsTransferService
{
    Task<ErrorOr<bool>> MoveCoinsAsync(
        string srcUserName,
        string dstUserName,
        long amount,
        CancellationToken ct = default);
}
