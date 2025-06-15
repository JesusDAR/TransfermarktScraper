using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using TransfermarktScraper.Web.Components;
using TransfermarktScraper.Web.Configuration;

namespace TransfermarktScraper.Web
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main function that serves as the entry point for the application.
        /// </summary>
        /// <param name="args">An array of command-line arguments.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            // Add MudBlazor services
            builder.Services.AddMudServices();

            // Add service defaults & Aspire client integrations.
            builder.AddServiceDefaults();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddOutputCache();

            builder.Services.AddWebServices(builder.Configuration);

            var app = builder.Build();

            app.UseExceptionHandler(builder =>
            {
                builder.Run(context =>
                {
                    var logger = app.Services.GetRequiredService<ILogger<Program>>();
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;
                    logger.LogError($"Exception Message: {exception?.Message}.");
                    return Task.CompletedTask;
                });
            });
            app.UseHsts();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();
            app.UseOutputCache();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapDefaultEndpoints();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Current environment: {EnvironmentName}", builder.Environment.EnvironmentName);
            logger.LogInformation("Starting Transfermarkt.Web...");

            app.Run();
        }
    }
}
