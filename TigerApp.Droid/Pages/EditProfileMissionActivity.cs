using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ReactiveUI;
using TigerApp.Shared.ViewModels;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "@string/app_name")]
    public class EditProfileMissionActivity : BaseReactiveActivity<IEditProfileMissionViewModel>
    {
        private Button _btnOpenEditProfile;
        private ImageView _btnBack;

        public EditProfileMissionActivity()
        {
            this.WhenActivated(dispose =>
            {
                InitListeners(dispose);
            });
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_edit_profile_mission);

            InitViews();
        }

        private void InitViews()
        {
            _btnBack = FindViewById<ImageView>(Resource.Id.btnBack);
            _btnOpenEditProfile = FindViewById<Button>(Resource.Id.btnOpenEditProfile);
        }

        private void InitListeners(Action<IDisposable> dispose)
        {
            dispose(Observable.FromEventPattern(
                    subscribe => _btnBack.Click += subscribe,
                    unSubscribe => _btnBack.Click -= unSubscribe)
                        .Subscribe(evArgs => Finish()));
            dispose(Observable.FromEventPattern(
                    subscribe => _btnOpenEditProfile.Click += subscribe,
                    unSubscribe => _btnOpenEditProfile.Click -= unSubscribe)
                        .Subscribe(evArgs => StartNewActivity(typeof(EditProfileActivity), TransitionWay.DU)));
        }
    }
}