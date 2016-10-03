using System;
using System.Collections.Generic;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.Core.ViewModels;
using UIKit;

namespace Translate.Controls
{
    public sealed class CustomTextField : UITextField
    {
        private bool _isValid;
        private MvxStandardTableViewSource _autocompleteTableViewSource;
        private UITableView AutoCompleteTableView { get; set; }

        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                _isValid = value;
                UpdateValidationState();
            }
        }

        public IMvxCommand ReturnCommand { get; set; }

        public IEnumerable<string> AutoCompleteItemsSource
        {
            get { return (IEnumerable<string>)_autocompleteTableViewSource.ItemsSource; }
            set
            {
                _autocompleteTableViewSource.ItemsSource = value;

                if (value == null || !value.Any())
                    AutoCompleteTableView.Hidden = true;

            }
        }

        private void UpdateValidationState()
        {
            if (!string.IsNullOrEmpty(this.Text))
            {
                BackgroundColor = IsValid ?
                     UIColor.FromRGB(174, 176, 93) :
                     UIColor.Orange;
            }
        }

        private void ClearValidationState()
        {
            BackgroundColor = UIColor.Clear;
        }

        public CustomTextField()
        {
            AutocorrectionType = UITextAutocorrectionType.No;
            AutocapitalizationType = UITextAutocapitalizationType.None;

            EditingDidBegin += (sender, args) =>
            {
                ClearValidationState();
            };

            ShouldReturn += (textField) =>
            {
                CloseAndExecute();

                return true;
            };

            ShouldChangeCharacters += (field, range, replacementString) =>
            {
                if (AutoCompleteTableView.Hidden)
                    AutoCompleteTableView.Hidden = false;

                return true;
            };

            base.ClipsToBounds = false;

            CreateAutoCompleteTableView();
        }

        private void CloseAndExecute()
        {
            AutoCompleteTableView.Hidden = true;

            ResignFirstResponder();

            Animate();

            if (ReturnCommand != null)
                ReturnCommand.Execute();
        }

        private void CreateAutoCompleteTableView()
        {
            AutoCompleteTableView = new UITableView(new CGRect(0, 40, 320, 120), UITableViewStyle.Plain);

            AutoCompleteTableView.TableFooterView = new UIView();
            AutoCompleteTableView.BackgroundColor = UIColor.White;
            AutoCompleteTableView.Layer.BorderWidth = 1;
            AutoCompleteTableView.Layer.BorderColor = UIColor.LightGray.CGColor;
            _autocompleteTableViewSource = new MvxStandardTableViewSource(AutoCompleteTableView, UITableViewCellStyle.Default,
                new NSString("AutocompleteCell"), "TitleText .");
            AutoCompleteTableView.Source = _autocompleteTableViewSource;
            AutoCompleteTableView.Hidden = true;
            _autocompleteTableViewSource.SelectedItemChanged += AutocompleteTableViewSource_SelectedItemChanged;


            Add(AutoCompleteTableView);


        }

        private void AutocompleteTableViewSource_SelectedItemChanged(object sender, EventArgs e)
        {
            this.ReplaceText(GetTextRange(BeginningOfDocument, EndOfDocument), (string)_autocompleteTableViewSource.SelectedItem);

            CloseAndExecute();
        }

        private void Animate()
        {
            var rotationAnimation = CABasicAnimation.FromKeyPath(@"transform.rotation.z");
            rotationAnimation.To = FromObject(Math.PI * 2.0);
            rotationAnimation.Duration = 1;
            rotationAnimation.AutoReverses = false;
            rotationAnimation.RepeatCount = 1;
            rotationAnimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);
            Layer.AddAnimation(rotationAnimation, "rotationAnimation");

        }

        public override bool PointInside(CGPoint point, UIEvent uievent)
        {
            if (uievent.Type == UIEventType.Touches)
            {
                var rect = new CGRect(0, 0, Frame.Width, Frame.Height + AutoCompleteTableView.Frame.Height);
                return rect.Contains(point);
            }

            return base.PointInside(point, uievent);

        }
    }
}