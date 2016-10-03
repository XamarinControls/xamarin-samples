using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Translate.Core.Contracts;
using Translate.Core.Services;
using Translate.Core.ViewModels;

namespace Translate.Core
{
    public class TranslateApp : MvxApplication
    {

        public TranslateApp()
        {
            Mvx.LazyConstructAndRegisterSingleton<IOfflineTranslationService, OfflineTranslationService>();
            Mvx.LazyConstructAndRegisterSingleton<IOnlineTranslationService, OnlineTranslationService>();
            Mvx.RegisterSingleton<IMvxAppStart>(new MvxAppStart<MainViewModel>());
        }
    }
}
