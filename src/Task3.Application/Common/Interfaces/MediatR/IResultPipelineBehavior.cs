using LanguageExt.Common;
using MediatR;

namespace Task3.Application.Common.Interfaces.MediatR;

public interface IResultPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, Result<TResponse>>
        where TRequest : IResultRequest<TResponse>
{

}
