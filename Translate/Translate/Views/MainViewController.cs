using System.Collections.Generic;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.iOS.Views;
using Translate.Controls;
using Translate.Core.ViewModels;
using Translate.Utils;
using UIKit;

namespace Translate.Views
{
    public class MainViewController : MvxViewController<MainViewModel>
    {
        #region Private members
        private CAGradientLayer _gradient;
        private CustomTextField _inputTextFiled;
        private UIActivityIndicatorView _indicatorView;
        private UITableView _resultTableView;
        private UITableView _synonymsTableView;
        private UILabel _titleLabel;
        private UILabel _sourceLabel;
        private Dictionary<string, UIView> _constraintDictionary;
        private NSLayoutConstraint[] _constraints;
        private UILabel _useGoogleLabel;
        private UISwitch _useGoogleSwitch;
        #endregion

        public bool ShouldAnimateBusyIndicator
        {
            get { return true; }
            set
            {
                if (value)
                {
                    _indicatorView.StartAnimating();
                }
                else
                {
                    _indicatorView.StopAnimating();
                }

            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.NavigationBarHidden = true;

            CreateGradient();

            CreateControls();

            CreateBindings();
        }

        private void CreateGradient()
        {
            _gradient = new CAGradientLayer
            {
                Frame = View.Bounds,
                NeedsDisplayOnBoundsChange = true,
                MasksToBounds = true,
                Colors = new[] { UIColor.FromRGB(244, 107, 69).CGColor, UIColor.FromRGB(238, 168, 73).CGColor }
            };
            View.Layer.InsertSublayer(_gradient, 0);
        }

        private void CreateBindings()
        {
            //useMvxStandardTableViewCell
            var resultTableViewSource = new MvxStandardTableViewSource(_resultTableView, UITableViewCellStyle.Value2, new NSString("ResultCell"), "TitleText Culture; DetailText Word");
            _resultTableView.Source = resultTableViewSource;

            var synonymsTableViewSource = new MvxStandardTableViewSource(_synonymsTableView, UITableViewCellStyle.Value2, new NSString("SynonymResultCell"), "TitleText Culture; DetailText Word");
            _synonymsTableView.Source = synonymsTableViewSource;

            var bindingSet = this.CreateBindingSet<MainViewController, MainViewModel>();
            bindingSet.Bind(this).For(a => a.ShouldAnimateBusyIndicator).To(vm => vm.IsBusy).OneWay();
            bindingSet.Bind(_inputTextFiled).For(f => f.Text).To(vm => vm.Input).TwoWay();
            bindingSet.Bind(_inputTextFiled).For(f => f.IsValid).To(vm => vm.IsSuccessful).TwoWay();
            bindingSet.Bind(_inputTextFiled).For(f => f.ReturnCommand).To(vm => vm.SearchCommand).TwoWay();
            bindingSet.Bind(_inputTextFiled).For(f => f.AutoCompleteItemsSource).To(vm => vm.AutoCompleteSuggestions).TwoWay();
            bindingSet.Bind(resultTableViewSource).To(vm => vm.TranslationResults).OneWay();
            bindingSet.Bind(synonymsTableViewSource).To(vm => vm.Synonyms).OneWay();
            bindingSet.Bind(_sourceLabel).For(f => f.Text).To(vm => vm.SourceLanguage).TwoWay();
            bindingSet.Bind(_useGoogleSwitch).For(s => s.On).To(vm => vm.ShouldUseGoogle).TwoWay();

            bindingSet.Apply();
        }

        private void CreateControls()
        {
            _constraintDictionary = new Dictionary<string, UIView>();

            _titleLabel = new UILabel()
            {
                Text = "Translator App",
                TextColor = UIColor.White,
                Font = UIFont.BoldSystemFontOfSize(24)
            };
            AddView("titleLabel", _titleLabel);

            _useGoogleLabel = new UILabel()
            {
                Text = "Use Google",
                TextColor = UIColor.White,
            };
            AddView("useGoogleLabel", _useGoogleLabel);

            _useGoogleSwitch = new UISwitch() { OnTintColor = UIColor.FromRGB(174, 176, 93) };
            AddView("useGoogleSwitch", _useGoogleSwitch);

            _indicatorView = new UIActivityIndicatorView { HidesWhenStopped = true };
            AddView("indicatorView", _indicatorView);

            _sourceLabel = new UILabel() { TextColor = UIColor.White };
            AddView("sourceLabel", _sourceLabel);

            _resultTableView = new UITableView
            {
                TableFooterView = new UIView(),
                BackgroundColor = UIColor.Clear,
                TableHeaderView = new UILabel(new CGRect(0, 0, 200, 40))
                {
                    Text = "Results",
                    TextColor = UIColor.White
                }
            };
            AddView("resultTableView", _resultTableView);

            _synonymsTableView = new UITableView
            {
                TableFooterView = new UIView(),
                BackgroundColor = UIColor.Clear,
                TableHeaderView = new UILabel(new CGRect(0, 0, 200, 40))
                {
                    Text = "Synonyms",
                    TextColor = UIColor.White
                }
            };
            AddView("synonymsTableView", _synonymsTableView);

            _inputTextFiled = new CustomTextField
            {
                Placeholder = "Type a word",
                TextColor = UIColor.White
            };
            _inputTextFiled.Layer.BorderColor = UIColor.White.CGColor;
            _inputTextFiled.Layer.BorderWidth = 1;
            AddView("inputTextFiled", _inputTextFiled);

            UpdateLayout(InterfaceOrientation);
        }

        private void AddView(string viewId, UIView view)
        {
            View.Add(view);
            _constraintDictionary.Add(viewId, view);
        }

        #region Handle layout & rotation
        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);

            if (!InterfaceOrientation.Equals(toInterfaceOrientation))
            {
                UpdateLayout(toInterfaceOrientation);
            }
        }

        private void UpdateLayout(UIInterfaceOrientation toInterfaceOrientation)
        {
            if (_constraints != null)
            {
                this.View.RemoveConstraints(_constraints);
            }

            if (toInterfaceOrientation.IsPortrait())
            {
                var formats = new[]
                {
                    "H:|-[titleLabel]-|",
                    "H:|-[inputTextFiled]-|",
                    "H:[indicatorView]-|",
                    "H:|-[sourceLabel]-|",
                    "H:|-[resultTableView]-|",
                    "H:|-[synonymsTableView]-|",
                    "H:[useGoogleLabel]-[useGoogleSwitch]-|",
                    "V:|-(25)-[titleLabel]-[inputTextFiled(40)]-[sourceLabel]-[resultTableView]-[synonymsTableView(==resultTableView)]-(15)-[useGoogleSwitch]-(25)-|"
                };


                _constraints = LayoutConstraints.AddConstraints(View, formats, _constraintDictionary).Union(new[]
                {
                    LayoutConstraints.AddEqualityConstraint(View, _useGoogleSwitch, _useGoogleLabel, NSLayoutAttribute.CenterY),
                    LayoutConstraints.AddEqualityConstraint(View, _indicatorView, _inputTextFiled, NSLayoutAttribute.CenterY)
               }).ToArray();
            }
            else
            {
                var formats = new[]
              {
                    "H:|-[titleLabel]-(25)-[sourceLabel(==titleLabel)]-|",
                    "H:|-[inputTextFiled]-(25)-[synonymsTableView(==sourceLabel)]-|",
                    "H:[indicatorView]-(27)-[synonymsTableView(==sourceLabel)]-|",
                    "H:[useGoogleLabel]-[useGoogleSwitch]-(25)-[synonymsTableView]-|",
                    "H:|-[resultTableView(==titleLabel)]",
                    "V:|-(25)-[titleLabel]-[inputTextFiled(40)]-[resultTableView]-(15)-[useGoogleSwitch]-(25)-|",
                    "V:|-(25)-[sourceLabel]-[synonymsTableView]-(25)-|"
                };

                _constraints = LayoutConstraints.AddConstraints(View, formats, _constraintDictionary).Union(new[]
              {
                    LayoutConstraints.AddEqualityConstraint(View, _useGoogleSwitch, _useGoogleLabel, NSLayoutAttribute.CenterY),
                    LayoutConstraints.AddEqualityConstraint(View, _indicatorView, _inputTextFiled, NSLayoutAttribute.CenterY)
               }).ToArray();
            }
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            _gradient.Frame = View.Bounds;
        }
        #endregion
    }
}