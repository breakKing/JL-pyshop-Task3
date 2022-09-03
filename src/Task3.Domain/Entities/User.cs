namespace Task3.Domain.Entities;

public class User
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public int Rating { get; set; }
}
