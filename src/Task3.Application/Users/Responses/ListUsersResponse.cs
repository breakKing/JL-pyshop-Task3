using Task3.Application.Users.Dtos;

namespace Task3.Application.Users.Responses;

public class ListUsersResponse
{
    public List<UserDto> Users { get; set; } = new();
}
