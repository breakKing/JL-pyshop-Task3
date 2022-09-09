using ErrorOr;

namespace Task3.gRPC.Server.Helpers;

public class ErrorOrHelper : IErrorOrHelper
{
    public TData GetDataFromErrorOr<TResulted, TData>(
        ErrorOr<TResulted> result,
        Func<TResulted, TData> dataPredicate)
            where TData : new()
    {
        var data = result.Match(r => dataPredicate(r), _ => new());

        return data;
    }

    public bool IsErrorOrStateSucceeded<T>(ErrorOr<T> result)
    {
        return !result.IsError;
    }

    public List<Error> GetErrorsFromErrorOr<T>(ErrorOr<T> result)
    {
        var errors = result.Match(_ => new(), e => e);

        return errors;
    }
}
