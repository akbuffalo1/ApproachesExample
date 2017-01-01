using Android.App;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using TigerApp.Droid.UI;
using TigerApp.Shared.ViewModels;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "@string/app_name")]
    public class EditProfileActivity : BaseReactiveActivity<IEditProfileViewModel>
    {
        const string TAG_DATE_PICKER = nameof(TAG_DATE_PICKER);
        const string TAG_CITY_PICKER = nameof(TAG_CITY_PICKER);
        const string TAG_EDIT_PROFILE = nameof(TAG_EDIT_PROFILE);

        private EditText _nickNameEditText;
        private EditText _nomeEditText;
        private EditText _cognomeEditText;
        private EditText _emailEditText;
        private EditText _phoneEditText;
        private EditText _compleannoEditText;
        private EditText _cityEditText;
        private RelativeLayout _compliteButton;
        private ImageView _backButton;
        private EditProfileDialogFragment _popUp;

        private List<string> _tigerCities;

        public EditProfileActivity()
        {
            this.WhenActivated(dispose =>
            {
                InitListeners(dispose);

                dispose(this.BindCommand(ViewModel, vm => vm.UpdateProfile, x => x._compliteButton));

                dispose(this.Bind(ViewModel, vm => vm.Nickname, vc => vc._nickNameEditText.Text));
                dispose(this.Bind(ViewModel, vm => vm.FirstName, vc => vc._nomeEditText.Text));
                dispose(this.Bind(ViewModel, vm => vm.LastName, vc => vc._cognomeEditText.Text));
                dispose(this.Bind(ViewModel, vm => vm.Email, vc => vc._emailEditText.Text));
                dispose(
                    ViewModel
                        .WhenAnyValue(vm => vm.BirthdayDate.Value)
                        .Select(birthDay => birthDay.ToString("d").Replace(".", "/"))
                        .BindTo(this, vc => vc._compleannoEditText.Text));
                dispose(this.Bind(ViewModel, vm => vm.MobileNumber, vc => vc._phoneEditText.Text));
                dispose(this.Bind(ViewModel, vm => vm.TigerCity, vc => vc._cityEditText.Text));
                dispose(ViewModel.WhenAnyValue(vm => vm.IsProfileComplete).BindTo(this, x => x._compliteButton.Enabled));
                dispose(ViewModel.WhenAnyValue(vm => vm.MissionAlreadyCompleted).Where(_ => _ == true).Subscribe(missionCompleted =>
                {
                    _compliteButton.FindViewById<TigerTextView>(Resource.Id.txtButtonLarge).Visibility = ViewStates.Gone;
                    _compliteButton.FindViewById<TigerTextView>(Resource.Id.txtButtonSmall).Visibility = ViewStates.Gone;
                    _compliteButton.FindViewById<TigerTextView>(Resource.Id.txtButtonConfirm).Visibility = ViewStates.Visible;
                }));
                dispose(ViewModel.WhenAnyValue(vm => vm.UpdateFinished).Where(_ => _ == true).Subscribe(_ => { Finish(); }));
                dispose(ViewModel.WhenAnyValue(vm => vm.TigerStoreCities).Subscribe(cities => _tigerCities = cities));
            });
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_edit_profile);

            _nickNameEditText = FindViewById<EditText>(Resource.Id.nickNameText);
            _nomeEditText = FindViewById<EditText>(Resource.Id.nomeText);
            _cognomeEditText = FindViewById<EditText>(Resource.Id.cognomeText);
            _emailEditText = FindViewById<EditText>(Resource.Id.emailText);
            _compleannoEditText = FindViewById<EditText>(Resource.Id.compleannoText);
            _phoneEditText = FindViewById<EditText>(Resource.Id.phoneText);
            _cityEditText = FindViewById<EditText>(Resource.Id.cityText);
            _compliteButton = FindViewById<RelativeLayout>(Resource.Id.compliteButton);
            _backButton = FindViewById<ImageView>(Resource.Id.btnBack);
            _popUp = EditProfileDialogFragment.NewInstance();
            _compliteButton.Enabled = false;

            OnSwipeRight += Finish;
        }

        private void InitListeners(Action<IDisposable> dispose)
        {
            dispose(Observable.FromEventPattern<View.FocusChangeEventArgs>(
                    subscribe => _phoneEditText.FocusChange += subscribe,
                    unSubscribe => _phoneEditText.FocusChange -= unSubscribe)
                        .Where(args => args.EventArgs.HasFocus && !string.IsNullOrEmpty(((EditText)args.Sender).Text))
                        .Subscribe(args => StartActivity(typeof(ChangeNumberInstructionActivity))));
            dispose(Observable.FromEventPattern<TextChangedEventArgs>(
                    subscribe => _nickNameEditText.TextChanged += subscribe,
                    unSubscribe => _nickNameEditText.TextChanged -= unSubscribe)
                        .Subscribe(args => TextChanged()));
            dispose(Observable.FromEventPattern<TextChangedEventArgs>(
                    subscribe => _nomeEditText.TextChanged += subscribe,
                    unSubscribe => _nomeEditText.TextChanged -= unSubscribe)
                        .Subscribe(args => TextChanged()));
            dispose(Observable.FromEventPattern<TextChangedEventArgs>(
                    subscribe => _cognomeEditText.TextChanged += subscribe,
                    unSubscribe => _cognomeEditText.TextChanged -= unSubscribe)
                        .Subscribe(args => TextChanged()));
            dispose(Observable.FromEventPattern<TextChangedEventArgs>(
                    subscribe => _emailEditText.TextChanged += subscribe,
                    unSubscribe => _emailEditText.TextChanged -= unSubscribe)
                        .Subscribe(args => TextChanged()));
            dispose(Observable.FromEventPattern<TextChangedEventArgs>(
                    subscribe => _phoneEditText.TextChanged += subscribe,
                    unSubscribe => _phoneEditText.TextChanged -= unSubscribe)
                        .Subscribe(args => TextChanged()));
            dispose(Observable.FromEventPattern<TextChangedEventArgs>(
                    subscribe => _compleannoEditText.TextChanged += subscribe,
                    unSubscribe => _compleannoEditText.TextChanged -= unSubscribe)
                        .Subscribe(args => TextChanged()));
            dispose(Observable.FromEventPattern(
                    subscribe => _compleannoEditText.Click += subscribe,
                    unSubscribe => _compleannoEditText.Click -= unSubscribe)
                        .Subscribe(args => ShowDataPicker()));
            dispose(Observable.FromEventPattern<TextChangedEventArgs>(
                    subscribe => _cityEditText.TextChanged += subscribe,
                    unSubscribe => _cityEditText.TextChanged -= unSubscribe)
                        .Subscribe(args => TextChanged()));
            dispose(Observable.FromEventPattern(
                    subscribe => _cityEditText.Click += subscribe,
                    unSubscribe => _cityEditText.Click -= unSubscribe)
                        .Subscribe(args => ShowCityPicker()));
            dispose(Observable.FromEventPattern(
                    subscribe => _backButton.Click += subscribe,
                    unSubscribe => _backButton.Click -= unSubscribe)
                        .Subscribe(args => Finish()));
            dispose(Observable.FromEventPattern(
                    subscribe => _popUp.OnDissmiss += subscribe,
                    unSubscribe => _popUp.OnDissmiss -= unSubscribe)
                        .Subscribe(args => Finish()));
        }

        private void ShowDataPicker()
        {
            var datePicker =
                DatePickerFragment.NewInstance(ViewModel.BirthdayDate, newBirthDay => ViewModel.BirthdayDate = newBirthDay);
            var transaction = FragmentManager.BeginTransaction();
            datePicker.Show(transaction, TAG_DATE_PICKER);
        }

        private void ShowCityPicker()
        {
            var cityPicker = ListPickerFragment.NewInstance(_tigerCities, city => ViewModel.TigerCity = city);
            var transaction = FragmentManager.BeginTransaction();
            cityPicker.Show(transaction, TAG_CITY_PICKER);
        }

        private void ShowPopup()
        {
            var transaction = FragmentManager.BeginTransaction();
            _popUp.Show(transaction, TAG_EDIT_PROFILE);
        }

        public void TextChanged()
        {
            if (!string.IsNullOrEmpty(_nickNameEditText.Text) && !string.IsNullOrEmpty(_nomeEditText.Text) &&
                !string.IsNullOrEmpty(_cognomeEditText.Text) && !string.IsNullOrEmpty(_emailEditText.Text) &&
                !string.IsNullOrEmpty(_phoneEditText.Text) && !string.IsNullOrEmpty(_compleannoEditText.Text) &&
                !string.IsNullOrEmpty(_cityEditText.Text))
            {
                _compliteButton.Enabled = true;
                _compliteButton.SetBackgroundResource(Resource.Drawable.btn_default);
            }
            else
            {
                _compliteButton.Enabled = false;
                _compliteButton.SetBackgroundResource(Resource.Drawable.btn_default_grey);
            }
        }
    }
}