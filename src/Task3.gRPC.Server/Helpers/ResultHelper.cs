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
}
