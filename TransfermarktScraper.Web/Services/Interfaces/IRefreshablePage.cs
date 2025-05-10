using System.Threading.Tasks;

namespace TransfermarktScraper.Web.Services.Interfaces
{
    /// <summary>
    /// Represents a page that supports manually triggering its initialization logic.
    /// </summary>
    public interface IRefreshablePage
    {
        /// <summary>
        /// Forces the execution of the page's <c>OnInitializedAsync</c> lifecycle method resetting its values to defaul.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ForceOnInitializedAsync();
    }
}
