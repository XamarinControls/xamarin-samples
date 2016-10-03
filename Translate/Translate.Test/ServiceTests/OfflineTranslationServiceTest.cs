using System;
using System.Linq;
using NUnit.Framework;
using Translate.Core.Model;
using Translate.Core.Services;
using Translate.Services;

namespace Translate.Test.ServiceTests
{
    [TestFixture]
    public class OfflineTranslationServiceTest
    {
        private OfflineTranslationService _translationService;

        [SetUp]
        public void Setup()
        {
            //ToDo use mock here
            _translationService = new OfflineTranslationService(new TranslationXmlReader());
        }

        [Test]
        public async void WHEN_not_initialized_THEN_exception_is_thrown()
        {
            Exception result = null;
            try
            {
                await _translationService.TranslateAync("success");
            }
            catch (Exception exception)
            {
                result = exception;
            }

            Assert.IsNotNull(result);
        }

        [Test]
        public async void WHEN_initialized_THEN_existing_word_is_found()
        {
            //Arrange
            await _translationService.Initialize();

            //act
            var translationResult = await _translationService.TranslateAync("success");

            //assert
            Assert.IsNotNull(translationResult);
            CollectionAssert.IsNotEmpty(translationResult.Results);
        }

        [Test]
        public async void WHEN_initialized_THEN_existing_word_source_language_is_detected()
        {
            //Arrange
            await _translationService.Initialize();

            //act
            var translationResult = await _translationService.TranslateAync("success");

            //assert
            Assert.IsNotNull(translationResult);
            Assert.AreEqual("en-US", translationResult.SourceLanguage);
        }

        [Test]
        public async void WHEN_initialized_THEN_correct_transaltions_are_found_for_existing_word()
        {
            //Arrange
            await _translationService.Initialize();

            //act
            var translationResult = await _translationService.TranslateAync("success");

            //assert
            var expected = new[]
            {
                new TranslationEntry("Erfolg", "de-DE"),
                new TranslationEntry("Gelingen", "de-DE"),
                new TranslationEntry("Gedeihen", "de-DE")
            };

            CollectionAssert.AreEqual(expected, translationResult.Results);
        }

        [Test]
        public async void WHEN_initialized_THEN_correct_transaltions_are_found_for_words_present_only_as_links()
        {
            //Arrange
            await _translationService.Initialize();

            //act
            var translationResult = await _translationService.TranslateAync("episode");

            //assert
            var expected = new[]
            {
                new TranslationEntry("Folge", "de-DE"),
            };

            Assert.AreEqual("en-US", translationResult.SourceLanguage);
            CollectionAssert.AreEqual(expected, translationResult.Results);
        }

        [Test]
        public async void WHEN_initialized_THEN_exception_thrown_for_words_not_existing()
        {
            //Arrange
            await _translationService.Initialize();
            Exception exception = null;

            //act
            //assert 
            try
            {
                await _translationService.TranslateAync("adada");
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception);
        }

        [Test]
        public async void WHEN_initialized_THEN_auto_complete_suggestions_are_provided()
        {
            //Arrange
            await _translationService.Initialize();

            //act
            var suggestions = await _translationService.FindAutoCompleteSuggestionsAync("e");

            //assert

            CollectionAssert.IsNotEmpty(suggestions);
        }

        [Test]
        public async void WHEN_initialized_THEN_auto_complete_suggestions_start_with_the_given_letters()
        {
            //Arrange
            await _translationService.Initialize();

            //act
            var suggestions = await _translationService.FindAutoCompleteSuggestionsAync("e");

            //assert

            foreach (var suggestion in suggestions)
            {
                Assert.IsTrue(suggestion.StartsWith("e", StringComparison.CurrentCultureIgnoreCase));
            }
        }

        [Test]
        public async void WHEN_initialized_THEN_auto_complete_suggestions_decrease_after_more_letters()
        {
            //Arrange
            await _translationService.Initialize();

            //act
            var suggestions1 = await _translationService.FindAutoCompleteSuggestionsAync("e");
            var suggestions2 = await _translationService.FindAutoCompleteSuggestionsAync("er");

            //assert
            Assert.Greater(suggestions1.Count(), suggestions2.Count());

        }

        [Test]
        public async void WHEN_initialized_THEN_auto_complete_suggestions_are_distinct()
        {
            //Arrange
            await _translationService.Initialize();

            //act
            var suggestions = await _translationService.FindAutoCompleteSuggestionsAync("e");

            //assert
            CollectionAssert.AreEqual(suggestions, suggestions.Distinct());
        }

        [Test]
        public async void WHEN_initialized_THEN_synonyms_are_found_for_existing_word()
        {
            //Arrange
            await _translationService.Initialize();

            //act
            var synonyms = await _translationService.FindSynonymsAync("success");

            //assert
            //assert
            var expected = new[]
            {
                new TranslationEntry("result", "en-US"),
                new TranslationEntry("achievement", "en-US"),
                new TranslationEntry("thrive", "en-US"),
                new TranslationEntry("flourishing", "en-US"),
                new TranslationEntry("successful outcome", "en-US"),

            };

            CollectionAssert.AreEqual(expected, synonyms.Results);
        }

        [Test]
        public async void WHEN_initialized_THEN_synoynms_exception_thrown_for_words_not_existing()
        {
            //Arrange
            await _translationService.Initialize();
            Exception exception = null;

            //act
            //assert 
            try
            {
                await _translationService.FindSynonymsAync("adada");
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception);
        }

        [Test]
        public async void WHEN_initialized_THEN_no_suggestions_for_non_exisitng()
        {
            //Arrange
            await _translationService.Initialize();

            //act
            var suggestions = await _translationService.FindAutoCompleteSuggestionsAync("easddasfasfas");

            //assert
            CollectionAssert.IsEmpty(suggestions);
        }


    }
}