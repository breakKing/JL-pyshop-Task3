using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Task3.Application.Common.Behaviors;

public class UnhandledExceptionBehavior<TRequest, TResponse> :
    IPipelineBehavior<TRequest, Result<TResponse>>
        where TRequest : IRequest<Result<TResponse>>
        where TResponse : class
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TResponse>> Handle(TRequest request,
        CancellationToken ct,
        RequestHandlerDelegate<Result<TResponse>> next)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogError(ex, "Unhandled Exception for Request {Name} {@Request}", requestName, request);

            return new Result<TResponse>(ex);
        }
    }
}
