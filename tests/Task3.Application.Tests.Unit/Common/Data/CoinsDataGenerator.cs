using Task3.Domain.Entities;

namespace Task3.Application.Tests.Unit.Common.Data;

public class CoinsDataGenerator
{
    public static List<Coin> CreateCoinsForUser(long userId, long amount, long startCoinId = 1)
    {
        var coins = new List<Coin>();

        for (var i = 0; i < amount; i++)
        {
            var coin = new Coin
            {
                Id = startCoinId + i,
                UserId = userId
            };

            coins.Add(coin);
        }

        return coins;
    }
}
