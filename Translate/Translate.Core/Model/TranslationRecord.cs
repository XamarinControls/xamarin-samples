using System.Collections.Generic;

namespace Translate.Core.Model
{
    public class TranslationRecord
    {
        public TranslationRecord()
        {
            TranslationLinks = new List<TranslationRecord>();
        }

        public TranslationEntry Entry { get; set; }

        public IList<TranslationRecord> TranslationLinks { get; private set; }
        public TranslationRecord ParentRecord { get; set; }

    }
}