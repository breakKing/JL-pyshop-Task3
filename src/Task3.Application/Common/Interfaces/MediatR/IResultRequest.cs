using LanguageExt.Common;
using MediatR;

namespace Task3.Application.Common.Interfaces.MediatR;

public interface IResultRequest<TResponse> : IRequest<Result<TResponse>>
{

}
