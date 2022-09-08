using ErrorOr;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Task3.Application.Coins.Commands;
using Task3.Application.Coins.Responses;
using Task3.Application.Users.Dtos;
using Task3.Application.Users.Queries;
using Task3.gRPC.Server.Helpers;

namespace Billing;

public class BillingService : Billing.BillingBase
{
    private readonly ISender _mediator;
    private readonly IErrorOrHelper _errorOrHelper;

    public BillingService(ISender mediator, IErrorOrHelper errorOrHelper)
    {
        _mediator = mediator;
        _errorOrHelper = errorOrHelper;
    }

    public override async Task ListUsers(None request,
        IServerStreamWriter<UserProfile> responseStream,
        ServerCallContext context)
    {
        var query = new ListUsersQuery();
        var response = await _mediator.Send(query, context.CancellationToken);

        var users = _errorOrHelper.GetDataFromErrorOr(response, r => r.Users);
        await WriteListUsersAsync(users, responseStream, context.CancellationToken);
    }

    public override async Task<Response> CoinsEmission(
        EmissionAmount request,
        ServerCallContext context)
    {
        var command = new CoinsEmissionCommand(request.Amount);
        var response = await _mediator.Send(command, context.CancellationToken);

        if (!_errorOrHelper.IsErrorOrStateSucceeded(response))
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

    private Response CreateFailedResponseForEmission(ErrorOr<CoinsEmissionResponse> result)
    {
        var statusResponse = new Response
        {
            Status = Response.Types.Status.Failed
        };

        var errors = _errorOrHelper.GetErrorsFromErrorOr(result);
        var messages = errors.ConvertAll(e => e.Description);

        statusResponse.Comment = string.Join(';', messages);

        return statusResponse;
    }
}
