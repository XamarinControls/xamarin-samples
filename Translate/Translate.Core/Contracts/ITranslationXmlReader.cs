using System.Collections.Generic;
using System.Threading.Tasks;
using Translate.Core.Model;

namespace Translate.Core.Contracts
{
    public interface ITranslationXmlReader
    {
        Task<IEnumerable<TranslationRecord>> LoadTranslationsAsync();
    }
}