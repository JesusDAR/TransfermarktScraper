using System.Text.RegularExpressions;

namespace TransfermarktScraper.BLL.Utils
{
    /// <summary>
    /// Provides utility methods for working with images.
    /// </summary>
    public static class ImageUtils
    {
        /// <summary>
        /// Converts a collection of Transfermarkt country IDs into corresponding image URLs using the provided base URL.
        /// </summary>
        /// <param name="countryTransfermarktIds">The list of country Transfermarkt IDs.</param>
        /// <param name="flagUrl">The base Transfermarkt URL where the flag images are hosted.</param>
        /// <returns>A collection of image URLs corresponding to the given country IDs.</returns>
        public static IEnumerable<string> ConvertCountryTransfermarktIdsToImageUrls(IEnumerable<string> countryTransfermarktIds, string flagUrl)
        {
            var imageUrls = new List<string>();

            foreach (var countryTransfermarktId in countryTransfermarktIds)
            {
                var imageUrl = flagUrl + "/" + countryTransfermarktId + ".png";

                imageUrls.Add(imageUrl);
            }

            return imageUrls;
        }

        /// <summary>
        /// Converts a collection of image URLs into Transfermarkt country IDs by extracting them from the URLs.
        /// </summary>
        /// <param name="imageUrls">The list of image URLs.</param>
        /// <returns>A collection of Transfermarkt country IDs extracted from the image URLs.</returns>
        public static IEnumerable<string> ConvertImageUrlsToCountryTransfermarktIds(IEnumerable<string> imageUrls)
        {
            var countryTransfermarktIds = new List<string>();

            foreach (var imageUrl in imageUrls)
            {
                var countryTransfermarktId = GetTransfermarktIdFromImageUrl(imageUrl);

                countryTransfermarktIds.Add(countryTransfermarktId);
            }

            return countryTransfermarktIds;
        }

        /// <summary>
        /// Extracts the Transfermarkt ID from an image URL.
        /// </summary>
        /// <param name="imageUrl">The url of the image from where to extract the Transfermarkt ID.</param>
        /// <returns>
        /// The extracted Transfermarkt ID as a string if found in the URL pattern.
        /// Returns an empty string if no ID is found or if the URL doesn't match the expected pattern.
        /// </returns>
        public static string GetTransfermarktIdFromImageUrl(string imageUrl)
        {
            Match match = Regex.Match(imageUrl, @"(\d+)\.png");

            if (match.Success)
            {
                var transfermarktId = match.Groups[1].Value;
                return transfermarktId;
            }

            return string.Empty;
        }
    }
}
