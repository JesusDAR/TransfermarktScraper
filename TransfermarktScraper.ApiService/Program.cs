using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scalar.AspNetCore;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.Data.Configuration;
using TransfermarktScraper.Domain.Configuration;

namespace TransfermarktScraper.ApiService
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

            // Add service defaults & Aspire client integrations.
            builder.AddServiceDefaults();

            // Add services to the container.
            builder.Services.AddProblemDetails();

            builder.Services.AddDomainServices();
            builder.Services.AddDataServices(builder.Configuration);
            builder.Services.AddBusinessLogicServices(builder.Configuration);
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(
                    options =>
                        options
                        .WithTitle("TransfermarktScraper API")
                        .WithTheme(ScalarTheme.BluePlanet)
                        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                        .Servers = [] // workaround to prevent scalar from taking a random generated localhost server as base url
                );
            }

            app.MapControllers();

            // Configure the HTTP request pipeline.
            app.UseExceptionHandler();

            app.MapDefaultEndpoints();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Starting Transfermarkt.ApiService...");

            app.Run();
        }
    }
}
