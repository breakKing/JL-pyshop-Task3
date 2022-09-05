using LanguageExt.Common;
using MediatR;

namespace Task3.Application.Common.Interfaces.MediatR;

public interface IResultRequestHandler<TRequest, TResponse> :
    IRequestHandler<TRequest, Result<TResponse>>
        where TRequest : IResultRequest<TResponse>
{

}
