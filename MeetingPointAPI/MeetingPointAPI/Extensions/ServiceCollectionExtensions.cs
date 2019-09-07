using MeetingPointAPI.Models;
using MeetingPointAPI.Repositories;
using MeetingPointAPI.Services;
using MeetingPointAPI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;

namespace MeetingPointAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigOptions<TOptions>(this IServiceCollection services, IConfiguration configuration, string section)
            where TOptions : class, new()
        {
            services.Configure<TOptions>(configuration.GetSection(section));
            services.AddSingleton(cfg => cfg.GetService<IOptions<TOptions>>().Value);
            return services;
        }

        public static IServiceCollection AddSettingsAndOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.AddConfigOptions<AppSettings>(configuration, "AppSettings");

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services, string connectionString)
        {
            services.AddTransient(provider => new DBRepository(connectionString));

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<IHereService, HereService>();
            services.AddTransient<IRouteSearcher, RouteSearcher>();

            return services;
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "API", Version = "v1" });
                c.OperationFilter<ExamplesOperationFilter>();
                c.OperationFilter<DescriptionOperationFilter>();
                var xmlPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "MeetingPointAPI.xml");
                c.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}
