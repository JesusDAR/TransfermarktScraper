using System;
using System.Security.Cryptography;
using System.Text;
using TransfermarktScraper.Domain.Exceptions;

namespace TransfermarktScraper.Domain.Utils
{
    /// <summary>
    /// Provides utility methods for entity level operations.
    /// </summary>
    public static class EntityUtils
    {
        /// <summary>
        /// Generates a SHA256 hash of the input id and returns it as a Base64-encoded string.
        /// </summary>
        /// <param name="rawId">The input id to hash. Cannot be null or empty.</param>
        /// <returns>A Base64-encoded SHA256 hash of the input string.</returns>
        public static string GetHash(string rawId)
        {
            if (string.IsNullOrEmpty(rawId))
            {
                var message = $"{nameof(rawId)} cannot be null or empty.";
                throw UtilException.LogError(nameof(GetHash), nameof(EntityUtils), message);
            }

            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(rawId));
            var id = Convert.ToBase64String(hash);
            return id;
        }
    }
}
