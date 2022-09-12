using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Task3.Application.Coins.Dtos;
using Task3.Application.Coins.Services;
using Task3.Application.Common.Behaviors;
using Task3.Application.Common.Interfaces.Services;
using Task3.Application.Common.Services;
using Task3.Domain.Entities;

namespace Task3.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddPipelineBehaviors();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMappers();

        services.AddSingleton<IDateTimeService, DateTimeService>();

        services.AddTransient<IEmissionService, EmissionService>();
        services.AddTransient<ICoinsTransferService, CoinsTransferService>();

        return services;
    }

    private static IServiceCollection AddPipelineBehaviors(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));

        return services;
    }

    private static IServiceCollection AddMappers(this IServiceCollection services)
    {
        var config = GetMappersConfiguration();
        services.AddSingleton(config);

        services.AddTransient<IMapper, ServiceMapper>();

        return services;
    }

    private static TypeAdapterConfig GetMappersConfiguration()
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.Default.PreserveReference(true);
        config.ForType<Move, MoveWithUserNamesDto>()
            .Map(dst => dst.SrcUserName,
                src => src.SrcUser == null ? string.Empty : src.SrcUser.Name)
            .Map(dst => dst.DstUserName,
                src => src.DstUser == null ? string.Empty : src.DstUser.Name);
        
        return config;
    }
}
