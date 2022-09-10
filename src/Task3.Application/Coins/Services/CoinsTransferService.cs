using ErrorOr;
using MapsterMapper;
using Task3.Application.Common.Interfaces.Repositories;
using Task3.Application.Common.Interfaces.Services;
using Task3.Domain.Entities;

namespace Task3.Application.Coins.Services;

public class CoinsTransferService : ICoinsTransferService
{
    private const string SRC_USER_DOESNT_EXIST_MESSAGE = "Source user doesn't exist";
    private const string DST_USER_DOESNT_EXIST_MESSAGE = "Destination user doesn't exist";
    private const string INSUFFICIENT_AMOUNT_MESSAGE = "Source user doesn't have enough coins";
    private const string SAVE_ERROR_MESSAGE = "An error occurred during saving changes";

    private readonly IUsersRepository _usersRepository;
    private readonly ICoinsRepository _coinsRepository;
    private readonly IMapper _mapper;

    public CoinsTransferService(
        IUsersRepository usersRepository,
        ICoinsRepository coinsRepository,
        IMapper mapper)
    {
        _usersRepository = usersRepository;
        _coinsRepository = coinsRepository;
        _mapper = mapper;
    }

    public async Task<ErrorOr<bool>> MoveCoinsAsync(
        string srcUserName,
        string dstUserName,
        long amount,
        CancellationToken ct = default)
    {
        var potentialSrcUser = await GetUserIfExistsAsync(srcUserName,
            SRC_USER_DOESNT_EXIST_MESSAGE, ct);

        if (potentialSrcUser.IsError)
        {
            return potentialSrcUser.Errors;
        }

        var srcUser = potentialSrcUser.Value;

        var potentialDstUser = await GetUserIfExistsAsync(dstUserName,
            DST_USER_DOESNT_EXIST_MESSAGE, ct);

        if (potentialDstUser.IsError)
        {
            return potentialDstUser.Errors;
        }

        var dstUser = potentialDstUser.Value;

        if (srcUser.Amount < amount)
        {
            return Error.Failure($"{nameof(CoinsTransferService)}.Amount.Failure",
                INSUFFICIENT_AMOUNT_MESSAGE);
        }

        var result = await _coinsRepository.AddMovesAsync(
            srcUser.Id,
            dstUser.Id,
            amount,
            ct);

        if (!result)
        {
            return Error.Failure($"{nameof(CoinsTransferService)}.Failure",
                SAVE_ERROR_MESSAGE);
        }

        return true;
    }

    private async Task<ErrorOr<User>> GetUserIfExistsAsync(string userName,
        string messageForFail,
        CancellationToken ct = default)
    {
        var user = await _usersRepository.GetOneWithCoinsAsync(
            userName,
            ct);

        if (user is null)
        {
            return Error.NotFound($"{nameof(CoinsTransferService)}.User.NotFound",
                messageForFail);
        }

        return user;
    }
}
