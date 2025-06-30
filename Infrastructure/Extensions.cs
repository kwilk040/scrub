using Core.Model;
using Core.Repositories;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class Extensions
{
    private const string ConfigurationSectionName = "postgres";

    public static IServiceCollection AddTransientInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = GetPostgresOptions(configuration);

        services.AddDbContext<ApplicationDbContext>(builder =>
        {
            builder.UseNpgsql(options.ConnectionString);
            builder.EnableSensitiveDataLogging();
        }, ServiceLifetime.Transient);

        services.AddTransient<IScrobbleRepository, ScrobbleRepository>();
        services.AddTransient<IAlbumRepository, AlbumRepository>();
        services.AddTransient<IArtistRepository, ArtistRepository>();
        services.AddTransient<ITrackRepository, TrackRepository>();
        services.AddTransient<IApiKeyRepository, ApiKeyRepository>();

        return services;
    }

    public static IServiceCollection AddScopedInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = GetPostgresOptions(configuration);

        services.AddDbContext<ApplicationDbContext>(builder =>
        {
            builder.UseNpgsql(options.ConnectionString);
            builder.EnableSensitiveDataLogging();
        });

        services.AddScoped<IScrobbleRepository, ScrobbleRepository>();
        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddScoped<IArtistRepository, ArtistRepository>();
        services.AddScoped<ITrackRepository, TrackRepository>();
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();

        return services;
    }

    public static IServiceCollection AddSingletonInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = GetPostgresOptions(configuration);

        services.AddDbContext<ApplicationDbContext>(builder =>
        {
            builder.UseNpgsql(options.ConnectionString);
            builder.EnableSensitiveDataLogging();
        }, ServiceLifetime.Singleton);

        services.AddSingleton<IScrobbleRepository, ScrobbleRepository>();
        services.AddSingleton<IAlbumRepository, AlbumRepository>();
        services.AddSingleton<IArtistRepository, ArtistRepository>();
        services.AddSingleton<ITrackRepository, TrackRepository>();
        services.AddSingleton<IApiKeyRepository, ApiKeyRepository>();

        return services;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddRoles<IdentityRole>()
            .AddSignInManager()
            .AddDefaultTokenProviders();
        return services;
    }

    private static PostgresOptions GetPostgresOptions(IConfiguration configuration)
    {
        var section = configuration.GetSection(ConfigurationSectionName);
        var options = new PostgresOptions();
        section.Bind(options);
        return options;
    }
}