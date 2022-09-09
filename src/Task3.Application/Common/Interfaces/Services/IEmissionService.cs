using ErrorOr;

namespace Task3.Application.Common.Interfaces.Services;

public interface IEmissionService
{
    Task<ErrorOr<bool>> MakeEmissionAsync(long amount, CancellationToken ct = default);
}
