namespace TransfermarktScraper.Domain.DTOs.Response.Scraper
{
    /// <summary>
    /// Represents the base response DTO.
    /// </summary>
    public class BaseResponse
    {
        /// <summary>
        /// Gets or sets the unique Transfermarkt identifier.
        /// </summary>
        required public string TransfermarktId { get; set; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// Equality is based on the <see cref="TransfermarktId"/> property.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// true if the specified object has the same reference, is a <see cref="BaseResponse"/> or has the same
        /// <see cref="TransfermarktId"/> as the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is not BaseResponse other)
            {
                return false;
            }

            return TransfermarktId == other.TransfermarktId;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// The hash code is based on the <see cref="TransfermarktId"/> property.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return TransfermarktId.GetHashCode();
        }
    }
}
