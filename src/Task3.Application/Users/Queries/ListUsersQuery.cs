using ErrorOr;
using MediatR;
using Task3.Application.Users.Responses;

namespace Task3.Application.Users.Queries;

public class ListUsersQuery : IRequest<ErrorOr<ListUsersResponse>>
{
    
}
