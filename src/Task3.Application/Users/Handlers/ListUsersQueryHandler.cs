using LanguageExt.Common;
using MapsterMapper;
using Task3.Application.Common.Interfaces.MediatR;
using Task3.Application.Common.Interfaces.Repositories;
using Task3.Application.Users.Dtos;
using Task3.Application.Users.Queries;
using Task3.Application.Users.Responses;

namespace Task3.Application.Users.Handlers;

public class ListUsersQueryHandler : IResultRequestHandler<ListUsersQuery, ListUsersResponse>
{
    private readonly IMapper _mapper;
    private readonly IUsersRepository _repository;

    public ListUsersQueryHandler(IMapper mapper, IUsersRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<ListUsersResponse>> Handle(ListUsersQuery request,
        CancellationToken ct)
    {
        var userDtos = await _repository.GetAllAsync(u => _mapper.Map<UserDto>(u), ct);

        return new ListUsersResponse
        {
            Users = userDtos
        };
    }
}
