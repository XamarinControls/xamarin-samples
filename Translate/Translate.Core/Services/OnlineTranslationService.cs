using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Translate.Core.Contracts;
using Translate.Core.Model;

namespace Translate.Core.Services
{
    public class OnlineTranslationService : IOnlineTranslationService
    {
        private readonly IWebRequestService _webRequestService;

        private const string GoogleTranslateApiKey = "AIzaSyCHzzOoLUVajJUTctfMtii17LHkGKa50XU";
        //ios"AIzaSyBsLA7FxLc3waWGb0MtB2e1nBRfEZIeZ6Q";

        public OnlineTranslationService(IWebRequestService webRequestService)
        {
            _webRequestService = webRequestService;
        }

        public async Task<TranslationResult> TranslateAync(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                return null;

            const string template = @"https://www.googleapis.com/language/translate/v2?key={0}&target=en&q={1}";
            var url = string.Format(template, GoogleTranslateApiKey, WebUtility.HtmlEncode(inputText));

            var json = await _webRequestService.GetResponseAsync(url);

            var jsonTemplate =
                new { data = new { translations = new[] { new { translatedText = "", detectedSourceLanguage = "" } } } };
            var result = JsonConvert.DeserializeAnonymousType(json, jsonTemplate);


            if (!result.data.translations.Any())
            {
                throw new KeyNotFoundException("No results");
            }

            return new TranslationResult
            {
                Results = result.data.translations.Select(t => new TranslationEntry(t.translatedText, "en")).ToArray(),
                SourceLanguage = result.data.translations[0].detectedSourceLanguage
            };

        }

        public Task Initialize()
        {
            return Task.FromResult(true);
        }

        public Task<TranslationResult> FindSynonymsAync(string inputText)
        {
            return Task.FromResult(new TranslationResult() { Results = new TranslationEntry[0] });
        }

        public Task<string[]> FindAutoCompleteSuggestionsAync(string inputText)
        {
            return Task.FromResult(new string[0]);
        }

        public async Task<string> DetectLanguageAsync(string inputText)
        {
            const string template = @"https://www.googleapis.com/language/translate/v2/detect?key={0}&q={1}";

            var json =
                await
                    _webRequestService.GetResponseAsync(string.Format(template, GoogleTranslateApiKey,
                        WebUtility.HtmlEncode(inputText)));
            return json;
        }

    }
};