using System.Collections.Generic;
using System.Threading.Tasks;
using Translate.Core.Model;

namespace Translate.Core.Contracts
{
    public interface ITranslationService
    {
        Task<TranslationResult> TranslateAync(string inputText);
        Task<TranslationResult> FindSynonymsAync(string inputText);
        Task<string[]> FindAutoCompleteSuggestionsAync(string inputText);
    }
}