using ErrorOr;

namespace Task3.gRPC.Server.Helpers;

public interface IErrorOrHelper
{
    TData GetDataFromErrorOr<TResulted, TData>(ErrorOr<TResulted> result,
        Func<TResulted, TData> dataPredicate)
        where TData : new();

    bool IsErrorOrStateSucceeded<T>(ErrorOr<T> result);

    List<Error> GetErrorsFromErrorOr<T>(ErrorOr<T> result);
}
