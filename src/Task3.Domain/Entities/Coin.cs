namespace Task3.Domain.Entities;

public class Coin
{
    public long Id { get; set; }
    public long UserId { get; set; }

    public List<Move> Moves { get; set; } = new();
}
