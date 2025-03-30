using System;
using System.Security.Cryptography;
using System.Text;

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
                throw new ArgumentException($"Error in {nameof(EntityUtils)}.{nameof(GetHash)}: id cannot be null.");
            }

            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(rawId));
            var id = Convert.ToBase64String(hash);
            return id;
        }
    }
}
