using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using ProdTemplate.Api.Exceptions;
using ProdTemplate.Api.Services;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

namespace ProdTemplate.Api;

public static class AppConfigureServicesExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddOpenApi()
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "so",
                });
                var executingAssembly = Assembly.GetExecutingAssembly();
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{executingAssembly.GetName().Name}.xml"));

                var referencedProjectsXmlDocPaths = executingAssembly.GetReferencedAssemblies()
                    .Where(assembly => assembly.Name != null &&
                                       assembly.Name.StartsWith("ProdTemplate", StringComparison.InvariantCultureIgnoreCase))
                    .Select(assembly => Path.Combine(AppContext.BaseDirectory, $"{assembly.Name}.xml"))
                    .Where(File.Exists);
                foreach (var xmlDocPath in referencedProjectsXmlDocPaths)
                {
                    options.IncludeXmlComments(xmlDocPath);
                }

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. <br/>
                          Enter 'Bearer' [space] and then your token in the text input below. <br/>
                          Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        []
                    }
                });
            });

            return services;
        }

        public IServiceCollection AddPostgres(string connStr)
        {
            services.AddDbContext<ProdTemplateContext>(o =>
                o.UseNpgsql(connStr,
                    options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
            return services;
        }

        public IServiceCollection AddOTel()
        {
            services.AddMetrics();
            services.AddOpenTelemetry()
                .WithMetrics(options =>
                {
                    options.AddPrometheusExporter();
                    options
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddMeter("Microsoft.AspNetCore.Hosting")
                        .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                        .AddMeter("System.Net.Http")
                        .AddMeter("ProdTemplate");
                })
                .WithTracing(options =>
                {
                    options.AddHttpClientInstrumentation();
                    options.AddAspNetCoreInstrumentation();
                });
            return services;
        }

        public IServiceCollection AddLogger(ConfigurationManager configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.GrafanaLoki("http://loki:3100", new List<LokiLabel>
                {
                    new() { Key = "app", Value = "webapi" }
                }, propertiesAsLabels: ["app"])
                .Enrich.FromLogContext()
                .CreateLogger();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            services.AddSingleton(logger);

            return services;
        }

        public IServiceCollection AddExceptionFilter()
        {
            services.AddControllers(o => o.Filters.Add<ExceptionFilter>());
            return services;
        }
    }
}