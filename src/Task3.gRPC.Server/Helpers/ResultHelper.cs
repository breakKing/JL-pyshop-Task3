using LanguageExt.Common;

namespace Task3.gRPC.Server.Helpers;

public class ResultHelper : IResultHelper
{
    public TData GetDataFromResult<TResulted, TData>(Result<TResulted> result,
        Func<TResulted, TData> dataPredicate)
            where TData : new()
    {
        var data = result.Match(r => dataPredicate(r), _ => new());

        return data;
    }

    public bool IsResultSucceeded<T>(Result<T> result)
    {
        return result.IsSuccess;
    }

    public Exception GetExceptionFromResult<T>(Result<T> result)
    {
        var exception = result.Match(_ => new(), ex => ex);

        return exception;
    }
}
