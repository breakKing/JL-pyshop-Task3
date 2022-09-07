using LanguageExt.Common;

namespace Task3.Application.Common.Interfaces.Services;

public interface IEmissionService
{
    Task<Result<bool>> MakeEmissionAsync(long amount, CancellationToken ct = default);
}
