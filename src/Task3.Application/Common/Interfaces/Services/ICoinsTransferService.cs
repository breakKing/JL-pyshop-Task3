using ErrorOr;

namespace Task3.Application.Common.Interfaces.Services;

public interface ICoinsTransferService
{
    Task<ErrorOr<bool>> MoveCoinsAsync(
        long srcUserId,
        long dstUserId,
        long amount,
        CancellationToken ct = default);
}
