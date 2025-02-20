using Aspire.Hosting;

namespace TransfermarktScraper.AppHost
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
            var builder = DistributedApplication.CreateBuilder(args);

            var apiService = builder.AddProject<Projects.TransfermarktScraper_ApiService>("apiservice");

            builder.AddProject<Projects.TransfermarktScraper_Web>("webfrontend")
                .WithExternalHttpEndpoints()
                .WithReference(apiService)
                .WaitFor(apiService);

            builder.Build().Run();
        }
    }
}