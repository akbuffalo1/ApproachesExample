using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Widget;
using ReactiveUI;
using TigerApp.Droid.Services.Platform;
using TigerApp.Droid.UI.ExpandableRecyclerView.Models;
using TigerApp.Droid.UI.Stores;
using TigerApp.Droid.Utils;
using TigerApp.Shared;
using TigerApp.Shared.Models;
using TigerApp.Shared.Services.API;
using TigerApp.Shared.ViewModels;
using Xamarin.Facebook;
using Xamarin.Facebook.Share.Model;
using Xamarin.Facebook.Share.Widget;

namespace TigerApp.Droid.Pages.MissionList
{
    [Activity]
    public class MissionsListActivity : BaseReactiveActivity<IMissionListViewModel>
    {
        public const int MISSION_LIST_ACTIVITY = 0xCCCCC;
        private const string TAG = nameof(MissionsListActivity);

        private RecyclerView.LayoutManager _categoriesLayoutManager;
        private RecyclerView _categoriesList;
        private MissionsAdapter _categoriesAdapter;
        private RelativeLayout _layoutActionBar;
        private FacebookCallback<AppInviteDialog.Result> _inviteCallback;
        private AppInviteDialog _inviteDialog;
        private ICallbackManager _callbackManager { get; set; }

        public MissionsListActivity()
        {
            this.WhenActivated(dispose =>
            {
                dispose(ViewModel.WhenAnyValue(vm => vm.Missions).Where(miss => miss != null).Subscribe(missions =>
                {
                    _categoriesAdapter.Missions = missions;
                    _categoriesList.SetAdapter(_categoriesAdapter);
                    var missionService = AD.Resolver.Resolve<IMissionApiService>();
                    _categoriesList.PostDelayed(new Java.Lang.Runnable(() => {
                        if (missionService.LastSelectedMission != null) { 
                            _categoriesList.SmoothScrollToPosition(missionService.LastSelectedMission.Order);
                            missionService.LastSelectedMission = null;
                        }
                    }),500);
                }));

                InitListeners(dispose);
                ViewModel.GetMissions();
            });
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_missions_list);
            //InitInviteDialog();

            InitViews();
        }

        private void InitViews()
        {
            _layoutActionBar = FindViewById<RelativeLayout>(Resource.Id.layoutActionBar);
            _categoriesLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            _categoriesList = FindViewById<RecyclerView>(Resource.Id.listOfMissions);
            var separatorHeight = Resources.GetDimensionPixelSize(Resource.Dimension.mission_list_item_separator_height);
            _categoriesList.AddItemDecoration(new SpacingItemDecoration(separatorHeight));
            _categoriesList.SetLayoutManager(_categoriesLayoutManager);
            _categoriesAdapter = new MissionsAdapter(ViewModel.Missions);
            _categoriesList.SetAdapter(_categoriesAdapter);

            OnSwipeDown += () => { Finish(); };
        }

        private void InitListeners(Action<IDisposable> dispose)
        {
            dispose(Observable.FromEventPattern(
                    subscribe => _layoutActionBar.Click += subscribe,
                    unSubscribe => _layoutActionBar.Click -= unSubscribe)
                        .Subscribe(evArgs => Finish()));
            dispose(Observable.FromEventPattern<Objective>(_categoriesAdapter, "Click")
                        .Subscribe(Event => HandleListItemClick(Event.EventArgs)));
        }

        private void HandleListItemClick(Objective objective)
        {
            AD.Resolver.Resolve<IMissionApiService>().LastSelectedMission = objective;
            switch (objective.Mark) {
                case "add-10pt-when-user-invites-friend-on-fb":
                    InviteFriends();
                    break;
                case "add-10pt-when-user-shares-product-on-fb":
                    StartNewActivity(typeof(ShareMissionActivity), TransitionWay.RL);
                    break;
                case "add-10pt-when-user-fills-his-profile":
                    StartNewActivity(typeof(EditProfileMissionActivity), TransitionWay.RL);
                    break;
                case "add-10pt-when-user-checkin-tiger-store":
                    StartNewActivity(typeof(CheckInMissionActivity), TransitionWay.RL);
                    break;
                case "add-15pt-when-user-checkin-in-two-different-cities":
                    StartNewActivity(typeof(TwoCityMissionActivity), TransitionWay.RL);
                    break;
                case "add-10pt-when-user-scan-receipt-with-more-than-10-eur":
                    var intent10 = new Intent(this, typeof(ScanMissionActivity));
                    intent10.PutExtra(ScanMissionActivity.EurosKey, 10);
                    StartNewActivity(intent10, TransitionWay.RL);
                    break;
                case "add-22pt-when-user-scan-receipt-with-more-than-20-eur":
                    var intentEuros20 = new Intent(this, typeof(ScanMissionActivity));
                    intentEuros20.PutExtra(ScanMissionActivity.EurosKey, 20);
                    StartNewActivity(intentEuros20, TransitionWay.RL);
                    break;
                case "add-35pt-when-user-scan-receipt-with-more-than-30-eur":
                    var intent30 = new Intent(this, typeof(ScanMissionActivity));
                    intent30.PutExtra(ScanMissionActivity.EurosKey, 30);
                    StartNewActivity(intent30, TransitionWay.RL);
                    break;
                case "add-40pt-when-user-checkin-5-different-store":
                    StartNewActivity(typeof(TigerTrotterMissionActivity), TransitionWay.RL);
                    break;
                case "add-10pt-when-user-complete-survey":
                    StartNewActivity(typeof(SurveyMissionActivity),TransitionWay.RL);
                    break;
            }
        }

        private void InitInviteDialog()
        {
            Xamarin.Facebook.FacebookSdk.SdkInitialize(ApplicationContext);
            _callbackManager = AD.Resolver.Resolve<ICallbackManager>();
            if (_callbackManager == null)
            {
                var ioc = AD.Resolver.Resolve<AD.IDependencyContainer>();
                _callbackManager = CallbackManagerFactory.Create();
                ioc.Register(_callbackManager);
            }

            _inviteCallback = new FacebookCallback<AppInviteDialog.Result>
            {
                HandleSuccess = inviteResult =>
                {
                    ViewModel.SendInviteFBActionToServer();
                },
                HandleCancel = () => { Logger.Debug(TAG, "HelloFacebook: Canceled"); },
                HandleError =
                    shareError =>
                    {
                        DialogUtil.ShowAlert(this, Resources.GetString(Resource.String.msg_facebook_post_error));
                    }
            };

            _inviteDialog = new AppInviteDialog(this);
            _inviteDialog.RegisterCallback(_callbackManager, _inviteCallback);
        }

        private void InviteFriends()
        {
            InitInviteDialog();
            if (AppInviteDialog.CanShow())
            {
                AppInviteContent content = (Xamarin.Facebook.Share.Model.AppInviteContent)new AppInviteContent.Builder()
                            .SetApplinkUrl(Constants.FacebookAppUrl)
                            .Build();
                _inviteDialog.Show(content);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            _callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

    }
}