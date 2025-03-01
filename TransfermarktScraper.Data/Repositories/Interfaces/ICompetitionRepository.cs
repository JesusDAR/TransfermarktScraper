using System.Collections.Generic;
using System.Threading.Tasks;
using TransfermarktScraper.Domain.Entities;

namespace TransfermarktScraper.Data.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for the competition repository, providing methods for accessing and managing competition data.
    /// </summary>
    public interface ICompetitionRepository
    {
        /// <summary>
        /// Asynchronously retrieves a competition by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the competition.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the competition if found; otherwise, null.
        /// </returns>
        Task<Competition?> GetAsync(string id);

        /// <summary>
        /// Asynchronously retrieves all competitions.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains a collection of competitions, or null if no competitions exist.
        /// </returns>
        Task<IEnumerable<Competition>> GetAllAsync();

        /// <summary>
        /// Asynchronously adds a range of competitions to the repository.
        /// </summary>
        /// <param name="competitions">The collection of competitions to be added.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task AddRangeAsync(IEnumerable<Competition> competitions);
    }
}
