using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Translate.Core.Contracts;
using Translate.Core.Model;

namespace Translate.Core.Services
{
    public class OfflineTranslationService : IOfflineTranslationService
    {
        private readonly ITranslationXmlReader _xmlReader;
        private List<TranslationRecord> _flatTranslationModel;
        private bool _isInitialized;

        public OfflineTranslationService(ITranslationXmlReader xmlReader)
        {
            _xmlReader = xmlReader;
        }

        public async Task Initialize()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;

                var nonFlat = (await _xmlReader.LoadTranslationsAsync()).ToList();
                _flatTranslationModel = nonFlat.Union(nonFlat.SelectMany(r => r.TranslationLinks)).ToList();
            }
        }

        public async Task<TranslationResult> TranslateAync(string inputText)
        {
            return await Task.Run(() =>
            {
                var matches = FindWordMatches(inputText);

                var result = new TranslationResult();

                var directMatch = matches.Find(m => m.ParentRecord == null);
                if (directMatch != null)
                {
                    result.SourceLanguage = directMatch.Entry.Culture;
                    result.Results = directMatch.TranslationLinks.Select(m => m.Entry).ToArray();
                    return result;
                }

                //have no direct match
                var indirectMatches = matches.Where(m => m.ParentRecord != null).ToList();
                result.SourceLanguage = indirectMatches.First().Entry.Culture;
                result.Results = indirectMatches.Select(m => m.ParentRecord.Entry).ToArray();
                return result;
            });
        }

        private List<TranslationRecord> FindWordMatches(string inputText)
        {
            var matches =
                _flatTranslationModel.FindAll(
                    t => string.Equals(t.Entry.Word, inputText, StringComparison.CurrentCultureIgnoreCase));

            if (!matches.Any())
            {
                throw new KeyNotFoundException(string.Format("{0} not found.", inputText));
            }

            return matches;
        }

        public async Task<TranslationResult> FindSynonymsAync(string inputText)
        {
            return await Task.Run(() =>
            {
                var matches = FindWordMatches(inputText);

                var result = new TranslationResult();

                var indirectMatches = matches.Where(m => m.ParentRecord != null)
                    .Select(translationRecord =>
                        _flatTranslationModel.FindAll(
                            t => t.ParentRecord == translationRecord.ParentRecord && t != translationRecord))
                    .SelectMany(s => s);

                result.Results = indirectMatches.Select(m => m.Entry).ToArray();
                //result.SourceLanguage = result.Results.First().Culture;
                return result;
            });
        }

        public Task<string[]> FindAutoCompleteSuggestionsAync(string inputText)
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrEmpty(inputText))
                    return new string[0];

                var matches =
                    _flatTranslationModel.FindAll(
                        t => t.Entry.Word.StartsWith(inputText, StringComparison.CurrentCultureIgnoreCase)).Select(t => t.Entry.Word).Distinct();
                return matches.ToArray();
            });
        }
    }
}