using ErrorOr;
using MapsterMapper;
using Task3.Application.Common.Interfaces.Repositories;
using Task3.Application.Common.Interfaces.Services;
using Task3.Application.Users.Dtos;
using Task3.Domain.Entities;

namespace Task3.Application.Coins.Services;

public class EmissionService : IEmissionService
{
    private const string INSUFFICIENT_AMOUNT_MESSAGE = "{0} coins is not enough for {1} users";

    private readonly ICoinsRepository _coinsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IMapper _mapper;

    public EmissionService(ICoinsRepository coinsRepository, IUsersRepository usersRepository, IMapper mapper)
    {
        _coinsRepository = coinsRepository;
        _usersRepository = usersRepository;
        _mapper = mapper;
    }

    public async Task<ErrorOr<bool>> MakeEmissionAsync(long amount, CancellationToken ct = default)
    {
        var users = await _usersRepository.GetAllWithSortedRatingAsync(u => _mapper.Map<UserRatingDto>(u), ct);
        var usersCount = users.LongCount();

        if (usersCount > amount)
        {
            var message = string.Format(INSUFFICIENT_AMOUNT_MESSAGE, amount, usersCount);
            return Error.Failure($"{typeof(EmissionService)}.Failure", message);
        }

        var amounts = DistributeCoinAmounts(users, amount);
        await GiveCoinsToUsersAsync(users, amounts, ct);

        return true;
    }

    private List<long> DistributeCoinAmounts(List<UserRatingDto> users, long amount)
    {
        var totalRating = users.Sum(u => u.Rating);

        // Выделяем каждому пользователю по одной гарантированной монете
        var amounts = new List<long>(users.Select(u => 1L));
        amount -= users.LongCount();

        var initialAmount = amount;
        for (var i = 0; i < amounts.LongCount(); i++)
        {
            if (amount == 0L)
            {
                break;
            }

            // Высчитываем нужное кол-во монет согласно рейтингу, при этом не превышая
            // кол-во монет, которые осталось распределить
            var givenAmount = Math.Min(
                (long)Math.Floor(initialAmount * ((users[i].Rating + 0.0) / totalRating)),
                amount);

            amounts[i] += givenAmount;
            amount -= givenAmount;
        }

        // Дораздаём оставшиеся монеты согласно рейтингу
        for (var i = 0; i < amounts.LongCount(); i++)
        {
            if (amount == 0L)
            {
                break;
            }

            amounts[i]++;
            amount--;
        }

        return amounts;
    }

    private async Task GiveCoinsToUsersAsync(List<UserRatingDto> users, List<long> amounts,
        CancellationToken ct = default)
    {
        for (var i = 0; i < users.LongCount(); i++)
        {
            await _coinsRepository.AddCoinsToUserAsync(users[i].Id, amounts[i], ct);
        }
    }
}
