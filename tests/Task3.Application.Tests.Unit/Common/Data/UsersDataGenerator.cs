using Task3.Application.Users.Dtos;
using Task3.Domain.Entities;

namespace Task3.Application.Tests.Unit.Common.Data;

public class UsersDataGenerator
{
    public static List<UserRatingDto> CreateUsersWithRandomRating(long count, Random random)
    {
        var users = new List<UserRatingDto>();
        for (var i = 0; i < count; i++)
        {
            users.Add(new UserRatingDto
            {
                Id = i + 1,
                Rating = random.Next(1, 10000)
            });
        }
        return users;
    }

    public static User CreateUserWithCoins(long userId, string userName, long coinsAmount, long startCoinId = 1)
    {
        return new User
        {
            Id = userId,
            Name = userName,
            Coins = CoinsDataGenerator.CreateCoinsForUser(userId, coinsAmount, startCoinId)
        };
    }
}
