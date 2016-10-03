using System.Threading.Tasks;

namespace Translate.Core.Contracts
{
    public interface IWebRequestService
    {
        Task<string> GetResponseAsync(string url);
    }
}