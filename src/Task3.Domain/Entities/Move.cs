namespace Task3.Domain.Entities;

public class Move
{
    public long CoinId { get; set; }
    public long UnixTimestamp { get; set; }
    public long? SrcUserId { get; set; }
    public long DstUserId { get; set; }
}