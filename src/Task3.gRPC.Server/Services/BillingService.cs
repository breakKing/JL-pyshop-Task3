using Grpc.Core;
using LanguageExt.Common;
using MediatR;
using Task3.Application.Coins.Commands;
using Task3.Application.Coins.Responses;
using Task3.Application.Users.Dtos;
using Task3.Application.Users.Queries;
using Task3.Domain.Exceptions;
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

    public override async Task<Response> CoinsEmission(
        EmissionAmount request,
        ServerCallContext context)
    {
        var command = new CoinsEmissionCommand(request.Amount);
        var response = await _mediator.Send(command, context.CancellationToken);

        if (!_resultHelper.IsResultSucceeded(response))
        {
            return CreateFailedResponseForEmission(response);
        }

        return new Response
        {
            Status = Response.Types.Status.Ok
        };
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

    private Response CreateFailedResponseForEmission(Result<CoinsEmissionResponse> result)
    {
        var statusResponse = new Response
        {
            Status = Response.Types.Status.Failed
        };

        var ex = _resultHelper.GetExceptionFromResult(result);
        if (ex is CoinsEmissionException coinsEx)
        {
            statusResponse.Comment = coinsEx.Message;
        }
        else
        {
            statusResponse.Comment = "Some error occurred during execution of emission";
        }
        return statusResponse;
    }
}
