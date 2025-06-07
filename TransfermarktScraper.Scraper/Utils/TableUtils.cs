namespace TransfermarktScraper.Scraper.Utils
{
    /// <summary>
    /// Provides util methods to manage scraped table data.
    /// </summary>
    public static class TableUtils
    {
        /// <summary>
        /// Determines whether a given string is empty or contains a placeholder value in a table data cell.
        /// </summary>
        /// <param name="content">The string to evaluate.</param>
        /// <returns><c>true</c> if the content is empty or equals "-"; otherwise, <c>false</c>.</returns>
        public static bool IsTableDataCellEmpty(string content)
        {
            return string.IsNullOrWhiteSpace(content) || content.Equals("-");
        }
    }
}
