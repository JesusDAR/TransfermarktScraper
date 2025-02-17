using Aspire.Hosting;

namespace TransfermarktScraper.AppHost
{
    class Program
    {
        static void Main(string[] args)
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

