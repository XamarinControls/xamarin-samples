using System.Collections.Generic;

namespace Translate.Core.Model
{
    public class TranslationResult
    {
        public TranslationEntry[] Results { get; set; }
        public string SourceLanguage { get; set; }

    }
}