using Android.App;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;
using TigerApp.Shared.ViewModels;
using HockeyApp.Android;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "@string/app_name")]
    public class SettingsActivity : BaseReactiveActivity<ISettingsViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_settings);

            var listView = FindViewById<ListView>(Resource.Id.settingsList);
            var backButton = FindViewById<ImageView>(Resource.Id.btnBack);

            backButton.Click += (sender, args) =>
            {
                Finish();
            };

            OnSwipeRight += Finish;
            //Temporary adapter
            var listDataset = new List<string>
            {
                "Regolamento / Termini di servizi",
                "Informativa sulla privacy",
                "Feedback",
                "Logout"
            };


            var listAdapter = new ArrayAdapter<string>(this, Resource.Layout.item_settings, Resource.Id.text1, listDataset);

            listView.Adapter = listAdapter;

            listView.ItemClick += ListView_ItemClick;

            FeedbackManager.Register(this);
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            switch (e.Position)
            {
                case 0:
                    //TODO: add content
                    break;
                case 1:
                    StartActivity(typeof(PrivacyActivity));
                    break;
                case 2:
                    StartActivity(typeof(SettingsFeedbackActivity));
                    //FeedbackManager.ShowFeedbackActivity(ApplicationContext);
                    break;
                case 3:
                    ShowPopup();
                    break;
            }
        }

        protected void ShowPopup()
        {
            var transaction = FragmentManager.BeginTransaction();
            var logoutPopup = new LogoutPopupDialogFragment();
            logoutPopup.OnLogout += (sender, e) =>
            {
                AD.Resolver.Resolve<AD.ITDesAuthService>().Logout();
                Finish();
            };
            logoutPopup.Show(transaction, "logout popup");
        }
    }
}