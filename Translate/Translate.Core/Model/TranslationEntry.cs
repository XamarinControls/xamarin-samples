using System;

namespace Translate.Core.Model
{
    public class TranslationEntry
    {
        public TranslationEntry(string word, string culture)
        {
            Word = word;
            Culture = culture;
        }

        public string Word { get; }
        public string Culture { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is TranslationEntry))
                return false;

            return Equals((TranslationEntry)obj);
        }

        protected bool Equals(TranslationEntry other)
        {
            return string.Equals(Word, other.Word) && string.Equals(Culture, other.Culture);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Word != null ? Word.GetHashCode() : 0) * 397) ^ (Culture != null ? Culture.GetHashCode() : 0);
            }
        }
    }
}