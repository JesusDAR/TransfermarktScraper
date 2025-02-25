using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseExceptionHandler();

            app.MapDefaultEndpoints();

            app.Run();
        }
    }
}
