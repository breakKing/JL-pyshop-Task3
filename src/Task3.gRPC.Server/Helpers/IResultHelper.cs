using LanguageExt.Common;

namespace Task3.gRPC.Server.Helpers;

public interface IResultHelper
{
    TData GetDataFromResult<TResulted, TData>(Result<TResulted> result,
        Func<TResulted, TData> dataPredicate)
        where TData : new();
}