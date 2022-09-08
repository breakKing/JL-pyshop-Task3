using ErrorOr;
using MapsterMapper;
using MediatR;
using Task3.Application.Common.Interfaces.Repositories;
using Task3.Application.Users.Dtos;
using Task3.Application.Users.Queries;
using Task3.Application.Users.Responses;

namespace Task3.Application.Users.Handlers;

public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, ErrorOr<ListUsersResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUsersRepository _repository;

    public ListUsersQueryHandler(IMapper mapper, IUsersRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<ErrorOr<ListUsersResponse>> Handle(ListUsersQuery request,
        CancellationToken ct)
    {
        var users = await _repository.GetAllWithCoinsAsync(ct);
        var userDtos = users.ConvertAll(u => _mapper.Map<UserDto>(u));

        return new ListUsersResponse
        {
            Users = userDtos
        };
    }
}
