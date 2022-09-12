namespace Task3.Application.Coins.Dtos;

public class CoinWithHistoryDto
{
    public long Id { get; set; }
    public List<MoveWithUserNamesDto> Moves { get; set; } = new();
}
