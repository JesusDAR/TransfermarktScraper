# Transfermarkt Scraper

A structured web scraping and analysis tool for football data from Transfermarkt, built using .NET Aspire, Blazor Server, MudBlazor, MongoDB, and Playwright.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- [MongoDB](https://www.mongodb.com/try/download/community)
- [Playwright CLI](https://playwright.dev/dotnet/docs/intro)
- Docker (optional for MongoDB or Playwright container)

## Running the Project

1. Clone the repository:

   ```bash
   git clone https://github.com/your-username/transfermarkt-scraper.git
   cd transfermarkt-scraper
   ```

2. Restore dependencies:

   ```bash
   dotnet restore
   ```

3. Install Playwright browsers:

   ```bash
   playwright install
   ```

4. Update configuration:

   Ensure the following settings exist in `appsettings.json`:

   ```json
   {
     "DbSettings": {
       "ConnectionString": "mongodb://localhost:27017",
       "DatabaseName": "TransfermarktScraper"
     },
     "ScraperSettings": {
       "BaseUrl": "https://www.transfermarkt.com"
     }
   }
   ```

5. Run the app:

   ```bash
   dotnet run --project TransfermarktScraper.AppHost
   ```
