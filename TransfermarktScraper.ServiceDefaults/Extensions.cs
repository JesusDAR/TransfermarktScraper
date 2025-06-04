using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using TransfermarktScraper.ServiceDefaults.Logging.Services.Impl;
using TransfermarktScraper.ServiceDefaults.Logging.Services.Interfaces;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
    /// This project should be referenced by each service project in your solution.
    /// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Adds default services to the specified application builder.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the application builder.</typeparam>
        /// <param name="builder">The application builder to which the default services will be added.</param>
        /// <returns>The updated application builder.</returns>
        public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder)
            where TBuilder : IHostApplicationBuilder
        {
            try
            {
                builder.ConfigureOpenTelemetry();

                builder.AddDefaultHealthChecks();

                builder.AddLogging();

                builder.Services.AddServiceDiscovery();

                builder.Services.ConfigureHttpClientDefaults(http =>
                {
                    http.AddServiceDiscovery();
                });

                return builder;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in {nameof(AddServiceDefaults)}", ex);
            }
        }

        /// <summary>
        /// Configures OpenTelemetry for the specified application builder.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the application builder.</typeparam>
        /// <param name="builder">The application builder to configure with OpenTelemetry.</param>
        /// <returns>The updated application builder with OpenTelemetry configured.</returns>
        public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder)
            where TBuilder : IHostApplicationBuilder
        {
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation();
                })
                .WithTracing(tracing =>
                {
                    tracing.AddSource(builder.Environment.ApplicationName)
                        .AddAspNetCoreInstrumentation()

                        // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                        //.AddGrpcClientInstrumentation()
                        .AddHttpClientInstrumentation();
                });

            builder.AddOpenTelemetryExporters();

            return builder;
        }

        /// <summary>
        /// Adds default health checks to the specified application builder.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the application builder.</typeparam>
        /// <param name="builder">The application builder to which default health checks will be added.</param>
        /// <returns>The updated application builder with the default health checks configured.</returns>
        public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder)
            where TBuilder : IHostApplicationBuilder
        {
            builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]); // Add a default liveness check to ensure app is responsive

            return builder;
        }

        /// <summary>
        /// Maps the default health check endpoints for the application.
        /// </summary>
        /// <param name="app">The instance of <see cref="WebApplication"/> to configure with health check endpoints.</param>
        /// <returns>The updated instance of <see cref="WebApplication"/> with health check endpoints mapped.</returns>
        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            // Adding health checks endpoints to applications in non-development environments has security implications.
            // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
            if (app.Environment.IsDevelopment())
            {
                // All health checks must pass for app to be considered ready to accept traffic after starting
                app.MapHealthChecks("/health");

                // Only health checks tagged with the "live" tag must pass for app to be considered alive
                app.MapHealthChecks("/alive", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains("live"),
                });
            }

            var appName = app.Configuration.GetSection("applicationName").Value;

            if (appName == "TransfermarktScraper.ApiService")
            {
                // Logs
                app.MapHub<LogHub>("/logs");
            }

            return app;
        }

        /// <summary>
        /// Extension method to configure logging services in the <see cref="IHostApplicationBuilder"/>.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the application builder that implements <see cref="IHostApplicationBuilder"/>.</typeparam>
        /// <param name="builder">The application builder where logging services will be added.</param>
        /// <returns>The same application builder with configured logging services.</returns>
        public static TBuilder AddLogging<TBuilder>(this TBuilder builder)
            where TBuilder : IHostApplicationBuilder
        {
            builder.Services.AddSignalR();

            builder.Services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole(options =>
                {
                    options.FormatterName = "custom"; // to use custom format
                });

                logging.AddConsoleFormatter<CustomConsoleFormatter, SimpleConsoleFormatterOptions>();

                logging.AddDebug();
            });

            builder.Services.AddSingleton<ILogStorageService, LogStorageService>();

            return builder;
        }

        private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder)
            where TBuilder : IHostApplicationBuilder
        {
            var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

            if (useOtlpExporter)
            {
                builder.Services.AddOpenTelemetry().UseOtlpExporter();
            }

            return builder;
        }
    }
}
