using Task3.gRPC.Server.Helpers;

namespace Task3.gRPC.Server;

public static class DependencyInjection
{
    public static IServiceCollection AddHelpers(this IServiceCollection services)
    {
        services.AddSingleton<IErrorOrHelper, ErrorOrHelper>();

        return services;
    }
}
