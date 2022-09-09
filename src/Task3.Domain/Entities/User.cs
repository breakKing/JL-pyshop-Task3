using System.ComponentModel.DataAnnotations.Schema;

namespace Task3.Domain.Entities;

public class User
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public int Rating { get; set; }

    public List<Coin> Coins { get; set; } = new();

    [NotMapped]
    public long Amount => Coins.LongCount();
}
