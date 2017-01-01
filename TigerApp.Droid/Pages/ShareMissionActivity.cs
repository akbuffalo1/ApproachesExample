using System;
using System.Reactive.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using ReactiveUI;
using TigerApp.Shared;
using TigerApp.Shared.ViewModels;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "ShareMissionActivity")]
    public class ShareMissionActivity : BaseReactiveActivity<IShareMissionViewModel>
    {
        private ImageView _ivBtnBack;
        private Button _btnShareMissionComplete;

        public ShareMissionActivity()
        {
            this.WhenActivated(dispose => {
                InitListeners(dispose);
            });
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_share_mission);

            InitViews();
        }

        private void InitViews()
        {
            _ivBtnBack = FindViewById<ImageView>(Resource.Id.btnActionBarBack);
            _btnShareMissionComplete = FindViewById<Button>(Resource.Id.btnShareMissionCompleteCommand);
            FindViewById<TextView>(Resource.Id.txtActionBarTitle).Text = Constants.Strings.ShareMissionPageTitle;
            FindViewById<TextView>(Resource.Id.txtShareMissionMessage).Text = Constants.Strings.ShareMissionPageMessage;
        }

        private void InitListeners(Action<IDisposable> dispose)
        { 
            dispose(Observable.FromEventPattern(
                subscribe => _ivBtnBack.Click += subscribe,
                unSubscribe => _ivBtnBack.Click -= unSubscribe)
                    .Subscribe(evArgs => Finish()));
            dispose(Observable.FromEventPattern(
                subscribe => _btnShareMissionComplete.Click += subscribe,
                unSubscribe => _btnShareMissionComplete.Click -= unSubscribe)
                    .Subscribe(evArgs => StartNewActivity(typeof(ProductsCatalogueActivity), TransitionWay.DU)));
        }
    }
}