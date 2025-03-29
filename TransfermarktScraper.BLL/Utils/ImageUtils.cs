using System.Text.RegularExpressions;

namespace TransfermarktScraper.BLL.Utils
{
    /// <summary>
    /// Provides util methods to manage scraped image data.
    /// </summary>
    public static class ImageUtils
    {
        /// <summary>
        /// Extracts the Transfermarkt ID from an image URL.
        /// </summary>
        /// <returns>
        /// The extracted Transfermarkt ID as a string if found in the URL pattern.
        /// Returns an empty string if no ID is found or if the URL doesn't match the expected pattern.
        /// </returns>
        public static string GetTransfermarktIdFromImageUrl(string url)
        {
            Match match = Regex.Match(url, @"(\d+)\.png");

            if (match.Success)
            {
                var transfermarktId = match.Groups[1].Value;
                return transfermarktId;
            }

            return string.Empty;
        }
    }
}
