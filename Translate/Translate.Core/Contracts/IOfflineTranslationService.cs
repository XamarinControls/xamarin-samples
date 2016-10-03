using System.Threading.Tasks;

namespace Translate.Core.Contracts
{
    public interface IOfflineTranslationService : ITranslationService
    {
        Task Initialize();

    }
}