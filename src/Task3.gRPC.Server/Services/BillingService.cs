using Grpc.Core;
using MediatR;
using Task3.Application.Users.Dtos;
using Task3.Application.Users.Queries;
using Task3.Domain.Entities;
using Task3.gRPC.Server.Helpers;

namespace Billing;

public class BillingService : Billing.BillingBase
{
    private readonly ISender _mediator;
    private readonly IResultHelper _resultHelper;

    public BillingService(ISender mediator, IResultHelper resultHelper)
    {
        _mediator = mediator;
        _resultHelper = resultHelper;
    }

    public override async Task ListUsers(None request,
        IServerStreamWriter<UserProfile> responseStream,
        ServerCallContext context)
    {
        var query = new ListUsersQuery();
        var response = await _mediator.Send(query, context.CancellationToken);

        var users = _resultHelper.GetDataFromResult(response, r => r.Users);
        await WriteListUsersAsync(users, responseStream, context.CancellationToken);
    }

    private async Task WriteListUsersAsync(List<UserDto> users,
        IServerStreamWriter<UserProfile> responseStream,
        CancellationToken ct)
    {
        bool written;
        for (int i = 0; i < users.Count; i++)
        {
            written = await WriteUserProfileAsync(users[i],
                responseStream,
                ct);

            if (!written)
            {
                break;
            }
        }
    }

    private async Task<bool> WriteUserProfileAsync(UserDto user,
        IServerStreamWriter<UserProfile> responseStream,
        CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
        {
            return false;
        }

        var profile = new UserProfile
        {
            Name = user.Name,
        };
        if (user.Amount != 0)
        {
            profile.Amount = user.Amount;
        }

        await responseStream.WriteAsync(profile);
        return true;
    }
}
