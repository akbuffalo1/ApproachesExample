using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Collections.Generic;
using TigerApp.Shared;
using TigerApp.iOS.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

namespace TigerApp.iOS.Pages.EditProfile
{
    [Register("TigerDatePicker")]
    public class TigerDatePicker : TigerDateTimePicker
    {
        public TigerDatePicker(IntPtr p) : base(p)
        {
            _datePicker.Mode = UIDatePickerMode.Date;
        }

        protected override string ToDateString(DateTimeOffset dateTime)
        {
            return dateTime.ToString("d");
        }
    }

    public abstract class TigerDateTimePicker : UITextField
    {
        protected abstract string ToDateString(DateTimeOffset dateTime);
        protected readonly UIDatePicker _datePicker = new UIDatePicker();

        private readonly UIToolbar _toolbar;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private readonly UIBarButtonItem _doneBarButton;
        private readonly UIBarButtonItem _cancelBarButton;
        private DateTimeOffset? _initialDate;

        private ReplaySubject<DateTimeOffset?> _date = new ReplaySubject<DateTimeOffset?>(1);
        public IObservable<DateTimeOffset?> Date => _date;

        public IObservable<Unit> doneObservable => _doneBarButtonSubject.ObserveOnUI();
        public IObservable<Unit> cancelObservable => _cancelBarButtonSubject.ObserveOnUI();

        private readonly ISubject<Unit> _doneBarButtonSubject = new Subject<Unit>();
        private readonly ISubject<Unit> _cancelBarButtonSubject = new Subject<Unit>();

        protected TigerDateTimePicker(IntPtr p) : base(p)
        {
            ShouldBeginEditing = (textField) => false;
            UserInteractionEnabled = true;

            var doneTranslation = Constants.Strings.Done;
            _doneBarButton = new UIBarButtonItem(doneTranslation, UIBarButtonItemStyle.Done, OnDoneBarButton);

            var cancelTranslation = Constants.Strings.Cancel;
            _cancelBarButton = new UIBarButtonItem(cancelTranslation, UIBarButtonItemStyle.Done, OnCancelBarButton);

            _toolbar = new UIToolbar(new CGRect(0, 0, 0, 35));

            SetupDatePicker();
            SetupToolbarAccessoryView();
            AddGestureRecognizer(new UITapGestureRecognizer(OnTouched));
        }

        void OnCancelBarButton(object sender, EventArgs e)
        {
            _cancelBarButtonSubject.OnNext(Unit.Default);
        }

        void OnDoneBarButton(object sender, EventArgs e)
        {
            _doneBarButtonSubject.OnNext(Unit.Default);
        }

        private void SetupDatePicker()
        {
            _datePicker.BackgroundColor = UIColor.FromRGB(240, 240, 240);
            Font = Fonts.FrutigerMedium.WithSize(19F);
        }

        public void SetDateTime(DateTimeOffset? dateTime)
        {
            _initialDate = dateTime;

            if (dateTime != null)
            {
                _datePicker.SetDate((NSDate)DateTime.SpecifyKind(_initialDate.Value.UtcDateTime, DateTimeKind.Utc), false);
                SetDateText(ToDateString(_initialDate.Value.ToLocalTime()));
            }
            else {
                _datePicker.SetDate((NSDate)DateTime.SpecifyKind(DateTimeOffset.Now.UtcDateTime, DateTimeKind.Utc), false);
                SetDateText(string.Empty);
            }
        }

        private void SetupToolbarAccessoryView()
        {
            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);

            _toolbar.UserInteractionEnabled = true;
            _toolbar.Translucent = true;
            _toolbar.BarTintColor = _datePicker.BackgroundColor;
            _toolbar.Items = new[] { _cancelBarButton, spacer, _doneBarButton };
        }

        private void AddToDisposable(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        private void OnTouched() => ShowPicker();

        private void ShowPicker()
        {
            Animate(0.5F, () => BecomeFirstResponder());
        }

        private void HidePicker()
        {
            Animate(0.5F, () => ResignFirstResponder());
        }

        public override bool ResignFirstResponder()
        {
            _disposables.ForEach(x => x.Dispose());
            _disposables.Clear();
            return base.ResignFirstResponder();
        }

        public override bool BecomeFirstResponder()
        {
            AddToDisposable(cancelObservable.Subscribe(_ =>
            {
                if (_initialDate == null)
                {
                    SetDateText(string.Empty);
                }
                else {
                    SetDateText(ToDateString(_initialDate.Value.ToLocalTime()));
                }
                HidePicker();
            }));

            var pickerObservable = Observable.FromEventPattern(h => _datePicker.ValueChanged += h, h => _datePicker.ValueChanged -= h);

            AddToDisposable(pickerObservable.Subscribe(_ =>
            {
                var date = DateTime.SpecifyKind((DateTime)_datePicker.Date, DateTimeKind.Utc);
                SetDateText(ToDateString(date.ToLocalTime()));
            }));

            AddToDisposable(doneObservable.Subscribe(_ =>
            {
                var date = DateTime.SpecifyKind((DateTime)_datePicker.Date, DateTimeKind.Utc);
                _date.OnNext(date.ToLocalTime());
                SetDateText(ToDateString(date.ToLocalTime()));
                HidePicker();
            }));

            _date.OnNext(_initialDate);
            return base.BecomeFirstResponder();
        }

        #region Overrides
        public override UIView InputView => _datePicker;
        public override UIView InputAccessoryView => _toolbar;
        public override bool CanResignFirstResponder => true;
        public override bool CanBecomeFirstResponder => true;
        #endregion

        private void SetDateText(string text)
        {
            Text = text;
        }
    }
}