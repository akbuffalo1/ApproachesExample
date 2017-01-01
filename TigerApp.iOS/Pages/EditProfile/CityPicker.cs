using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TigerApp.iOS.Utils;
using TigerApp.Shared;
using UIKit;

namespace TigerApp.iOS.Pages.EditProfile
{
    [Register("TigerCityPicker")]
    public class TigerCityPicker : UITextField
    {
        private readonly UIPickerView _cityPicker = new UIPickerView();

        private readonly UIToolbar _toolbar;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private readonly UIBarButtonItem _doneBarButton;
        private readonly UIBarButtonItem _cancelBarButton;
        private string _initialCity;
        private List<string> _cities = new List<string>();

        private ReplaySubject<string> _city = new ReplaySubject<string>(1);
        public IObservable<string> City => _city;

        public IObservable<Unit> doneObservable => _doneBarButtonSubject.ObserveOnUI();
        public IObservable<Unit> cancelObservable => _cancelBarButtonSubject.ObserveOnUI();

        private readonly ISubject<Unit> _doneBarButtonSubject = new Subject<Unit>();
        private readonly ISubject<Unit> _cancelBarButtonSubject = new Subject<Unit>();

        private string _selectedCity = string.Empty;


        public TigerCityPicker(IntPtr p) : base(p)
        {
            ShouldBeginEditing = (textField) => false;
            UserInteractionEnabled = true;

            var doneTranslation = Constants.Strings.Done;
            _doneBarButton = new UIBarButtonItem(doneTranslation, UIBarButtonItemStyle.Done, OnDoneBarButton);

            var cancelTranslation = Constants.Strings.Cancel;
            _cancelBarButton = new UIBarButtonItem(cancelTranslation, UIBarButtonItemStyle.Done, OnCancelBarButton);

            _toolbar = new UIToolbar(new CGRect(0, 0, 0, 35));

            _cityPicker.BackgroundColor = UIColor.FromRGB(240, 240, 240);
            Font = Fonts.FrutigerMedium.WithSize(19F);
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

        public void SetCity(string city)
        {
            _initialCity = city;
            SetCityText(city ?? string.Empty);
        }

        public void SetCities(List<string> cities)
        {
            var cityPickerModel = new CityPickerModel(cities);
            cityPickerModel.ItemSelected += (s, e) => { _selectedCity = e; };
            _cityPicker.Model = cityPickerModel;
        }

        private void SetupToolbarAccessoryView()
        {
            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);

            _toolbar.UserInteractionEnabled = true;
            _toolbar.Translucent = true;
            _toolbar.BarTintColor = _cityPicker.BackgroundColor;
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

        public override bool BecomeFirstResponder()
        {
            AddToDisposable(cancelObservable.Subscribe(_ =>
            {
                SetCityText(_initialCity ?? string.Empty);
                HidePicker();
            }));

            AddToDisposable(doneObservable.Subscribe(_ =>
            {
                _city.OnNext(_selectedCity);
                HidePicker();
            }));

            _city.OnNext(_initialCity);
            return base.BecomeFirstResponder();
        }

        #region Overrides
        public override UIView InputView => _cityPicker;
        public override UIView InputAccessoryView => _toolbar;
        public override bool CanResignFirstResponder => true;
        public override bool CanBecomeFirstResponder => true;
        #endregion

        private void SetCityText(string text)
        {
            Text = text;
        }

        class CityPickerModel : UIPickerViewModel
        {
            private List<string> _cities;
            public event EventHandler<string> ItemSelected;

            public CityPickerModel(List<string> cities)
            {
                _cities = cities;
            }

            public override nint GetComponentCount(UIPickerView pickerView)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return _cities.Count;
            }

            public override string GetTitle(UIPickerView pickerView, nint row, nint component)
            {
                return _cities[(int)row];
            }

            public override void Selected(UIPickerView pickerView, nint row, nint component)
            {
                ItemSelected?.Invoke(this, _cities[(int)row]);
            }
        }
    }
}
