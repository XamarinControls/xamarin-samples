using System;
using System.Collections.Generic;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using Translate.Core.Contracts;
using Translate.Core.Model;

namespace Translate.Core.ViewModels
{
    public class MainViewModel : MvxViewModel
    {
        #region Private fields

        private readonly IOfflineTranslationService _offlineTranslationService;
        private readonly IOnlineTranslationService _onlineTranslationService;
        private ITranslationService _translationService;
        private bool _isBusy;
        private string _input;
        private IEnumerable<TranslationEntry> _translationResults;
        private string _sourceLanguage;
        private IEnumerable<TranslationEntry> _synonyms;
        private bool _isSuccessful;
        private bool _shouldUseGoogle;
        private IEnumerable<string> _autoCompleteSuggestions;

        #endregion

        #region Bindable props
        public MvxCommand SearchCommand
        {
            get
            {
                return new MvxCommand(Translate);
            }
        }


        public IEnumerable<TranslationEntry> Synonyms
        {
            get { return _synonyms; }
            set
            {
                _synonyms = value;
                RaisePropertyChanged(() => Synonyms);
            }
        }

        public IEnumerable<TranslationEntry> TranslationResults
        {
            get { return _translationResults; }
            set
            {
                _translationResults = value;
                RaisePropertyChanged(() => TranslationResults);
            }
        }

        public IEnumerable<string> AutoCompleteSuggestions
        {
            get { return _autoCompleteSuggestions; }
            private set
            {
                _autoCompleteSuggestions = value;
                RaisePropertyChanged(() => AutoCompleteSuggestions);
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);

            }
        }

        public string Input
        {
            get { return _input; }
            set
            {
                _input = value;
                RaisePropertyChanged(() => Input);

                UpdateAutoCompleteSuggestions();
            }
        }

        public string SourceLanguage
        {
            get { return _sourceLanguage; }
            set
            {
                _sourceLanguage = value;
                RaisePropertyChanged(() => SourceLanguage);
            }
        }

        public bool IsSuccessful
        {
            get { return _isSuccessful; }
            set
            {
                _isSuccessful = value;
                RaisePropertyChanged(() => IsSuccessful);
            }
        }

        public bool ShouldUseGoogle
        {
            get { return _shouldUseGoogle; }
            set
            {
                _shouldUseGoogle = value;
                _translationService = _shouldUseGoogle ? (ITranslationService)_onlineTranslationService : _offlineTranslationService;
            }
        }
        #endregion bindable props

        #region Action methods
        private async void UpdateAutoCompleteSuggestions()
        {
            AutoCompleteSuggestions = await _translationService.FindAutoCompleteSuggestionsAync(Input);
        }
        private async void Translate()
        {
            try
            {
                IsBusy = true;

                var translationResults = await _translationService.TranslateAync(Input);
                TranslationResults = translationResults.Results;
                SourceLanguage = string.Format("Detected language: {0}", translationResults.SourceLanguage);

                IsSuccessful = true;

                var synonymResults = await _translationService.FindSynonymsAync(Input);
                Synonyms = synonymResults.Results;

            }
            catch (Exception ex)
            {
                IsSuccessful = false;
                SourceLanguage = string.Format("'{0}' not found", Input);
                TranslationResults = new TranslationEntry[0];
                Synonyms = new TranslationEntry[0];

                Mvx.Trace(MvxTraceLevel.Error, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override void Start()
        {
            base.Start();

            try
            {
                IsBusy = true;
                _offlineTranslationService.Initialize();
            }
            catch (Exception ex)
            {
                Mvx.Trace(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

        #region C'tor
        public MainViewModel(IOfflineTranslationService offlineTranslationService, IOnlineTranslationService onlineTranslationService)
        {
            _offlineTranslationService = offlineTranslationService;
            _onlineTranslationService = onlineTranslationService;
            _translationService = offlineTranslationService;
            _shouldUseGoogle = false;
        }
        #endregion
    }
}