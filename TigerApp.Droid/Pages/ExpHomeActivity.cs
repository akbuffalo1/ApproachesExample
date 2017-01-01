#region using

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AD;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using HockeyApp.Android;
using ReactiveUI;
using TigerApp.Droid.Tutorial;
using TigerApp.Droid.UI;
using TigerApp.Droid.UI.ToolTips;
using TigerApp.Shared;
using TigerApp.Shared.Models;
using TigerApp.Shared.ViewModels;

#endregion

[assembly: MetaData("com.facebook.sdk.ApplicationId", Value = "@string/facebook_app_id")]
namespace TigerApp.Droid.Pages
{
    [Activity]
    public class ExpHomeActivity : BaseReactiveActivity<IExpHomeViewModel>
    {
        public ExpHomeActivity()
        {
            this.WhenActivated(dispose =>
            {
                InitListeners(dispose);
                dispose(ViewModel.WhenAnyValue(vm => vm.ShouldShowTutorial).Where(show => show == true).Subscribe((obj) => { ShowWelcomeMessage(); }));
                
                dispose(ViewModel.WhenAnyValue(vm => vm.Error).Where(error => !string.IsNullOrEmpty(error)).Subscribe((error) =>
                {
                    if (AD.Resolver.Resolve<ITDesAuthStore>().GetAuthData().AuthProviderToken == null) { 
                        StartNewActivity(typeof(HomeActivity), TransitionWay.LR);
                        Finish();
                    }
                    Resolver.Resolve<IDialogProvider>()
                        .DisplayConfirmation(GetString(Resource.String.general_error_title), error, () =>
                        {
                            Finish();
                        });
                }));
                dispose(ViewModel.WhenAnyValue(vm => vm.State).Where(state => state != null).Subscribe((userState) =>
                {
                    int level;
                    if(userState.Current.Level.StartsWith("level-", StringComparison.CurrentCulture))
                        userState.Current.Level = userState.Current.Level.Replace("level-",string.Empty);
                    if (userState.Current.Level.Equals(""))
                        userState.Current.Level = "1";
                    if (Int32.TryParse(userState.Current.Level, out level))
                    {
                        level = level <= AllStates.Count ? level : AllStates.Count; 
                        var defaultState = AllStates[level - 1];
                        var newState = new ExpData
                        {
                            Level = level,
                            Coupon = userState.Current.CouponCount,
                            CheckedPoint = userState.Current.MissionsCount,
                            Current = userState.Current.Points,
                            Next = userState.LevelThreshold.Points,
                            Min = userState.CouponThreshold.Points - 600,
                            CouponThresholdPoints = userState.CouponThreshold.Points,
                            TotalPoints = userState.LevelThreshold.MissionsCount
                        };
                        this.RunOnUiThread(() => { 
                            SetExpView(newState);
                        });
                    }
                    else
                    {
                        Resolver.Resolve<IDialogProvider>()
                            .DisplayConfirmation(GetString(Resource.String.general_error_title), GetString(Resource.String.general_error_message), () =>
                            {
                                Finish();
                            });
                    }
                }));
                //SetExpView(AllStates[0]);
                if (AD.Resolver.Resolve<ITDesAuthStore>().GetAuthData().AuthProviderToken == null)
                {
                    StartNewActivity(typeof(HomeActivity), TransitionWay.LR);
                    Finish();
                }
                else
                    ViewModel.GetUserState();
            });
        }

        private ExpProgressBar _progressBar;
        private ImageView _btnInfo;
        private ImageView _btnUser;
        private ImageView _btnSettings;
        private ImageView _btnNew;
        private ImageView _btnHomeLocation;
        private ImageView _btnNavigate;
        private ImageView _txtAppExpBack;
        private ImageButton _btnQrCode;
        private TextView _txtMaxExp;
        private TextView _txtTxtAppExp;
        private TextView _txtPageMsg;
        private RelativeLayout _container;
        private List<ImageView> _coupons => new List<ImageView>()
            {
                FindViewById<ImageView>(Resource.Id.imgCoupon1),
                FindViewById<ImageView>(Resource.Id.imgCoupon2),
                FindViewById<ImageView>(Resource.Id.imgCoupon3),
                FindViewById<ImageView>(Resource.Id.imgCoupon4),
                FindViewById<ImageView>(Resource.Id.imgCoupon5),
            };
        private List<ImageView> _points => new List<ImageView>()
            {
                FindViewById<ImageView>(Resource.Id.point1),
                FindViewById<ImageView>(Resource.Id.point2),
                FindViewById<ImageView>(Resource.Id.point3),
                FindViewById<ImageView>(Resource.Id.point4),
                FindViewById<ImageView>(Resource.Id.point5),
                FindViewById<ImageView>(Resource.Id.point6),
            };

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_exp_home);

            _btnInfo = FindViewById<ImageView>(Resource.Id.btnInfo);
            _btnSettings = FindViewById<ImageView>(Resource.Id.btnSettings);
            _btnUser = FindViewById<ImageView>(Resource.Id.btnUser);

            _btnNew = FindViewById<ImageView>(Resource.Id.btnNew);
            _btnHomeLocation = FindViewById<ImageView>(Resource.Id.btnHomeLocation);
            _btnNavigate = FindViewById<ImageView>(Resource.Id.btnArrowDown);
            _btnQrCode = FindViewById<ImageButton>(Resource.Id.btnQrCode);
            
            _progressBar = FindViewById<ExpProgressBar>(Resource.Id.expProgressbar);
            _txtMaxExp = FindViewById<TextView>(Resource.Id.txtMaxExp);
            _txtTxtAppExp = FindViewById<TextView>(Resource.Id.txtAppExp);
            _txtAppExpBack = FindViewById<ImageView>(Resource.Id.txtAppExpBack);
            _txtPageMsg = FindViewById<TextView>(Resource.Id.txtPageMsg);

            _container = FindViewById<RelativeLayout>(Resource.Id.container);

            OnSwipeDown += () => { StartNewActivity(typeof(ProfileActivity), TransitionWay.UD); };
            OnSwipeUp += () => { StartNewActivity(typeof(MissionList.MissionsListActivity), TransitionWay.DU); };
            OnSwipeLeft += () => { StartNewActivity(typeof(StoreLocatorActivity), TransitionWay.RL); };
            OnSwipeRight += () => { StartNewActivity(typeof(ProductsCatalogueActivity), TransitionWay.LR); };

            CheckForUpdates();
            //ViewModel.GetUserState();
        }

        protected override void OnPause()
        {
            base.OnPause();

            UnregisterManagers();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            UnregisterManagers();
        }

        private void InitListeners(Action<IDisposable> dispose)
        { 
            dispose(Observable.FromEventPattern(
                    subscribe => _btnInfo.Click += subscribe,
                    unSubscribe => _btnInfo.Click -= unSubscribe)
                        .Subscribe(args => ShowTutorial()));

            dispose(Observable.FromEventPattern(
                subscribe => _btnUser.Click += subscribe,
                unSubscribe => _btnUser.Click -= unSubscribe)
                    .Subscribe(args => StartNewActivity(typeof(ProfileActivity), TransitionWay.UD)));

            dispose(Observable.FromEventPattern(
                subscribe => _btnNew.Click += subscribe,
                unSubscribe => _btnNew.Click -= unSubscribe)
                    .Subscribe(args => StartNewActivity(typeof(ProductsCatalogueActivity), TransitionWay.LR)));

            dispose(Observable.FromEventPattern(
                subscribe => _btnHomeLocation.Click += subscribe,
                unSubscribe => _btnHomeLocation.Click -= unSubscribe)
                    .Subscribe(args => StartNewActivity(typeof(StoreLocatorActivity), TransitionWay.RL)));

            dispose(Observable.FromEventPattern(
                subscribe => _btnSettings.Click += subscribe,
                unSubscribe => _btnSettings.Click -= unSubscribe)
                    .Subscribe(args => StartNewActivity(typeof(SettingsActivity), TransitionWay.RL)));

            dispose(Observable.FromEventPattern(
                subscribe => _btnQrCode.Click += subscribe,
                unSubscribe => _btnQrCode.Click -= unSubscribe)
                    .Subscribe(args => StartNewActivity(typeof(ScanReceiptActivity), TransitionWay.Default)));

            dispose(Observable.FromEventPattern(
                subscribe => _btnNavigate.Click += subscribe,
                unSubscribe => _btnNavigate.Click -= unSubscribe)
                    .Subscribe(args => StartNewActivity(typeof(MissionList.MissionsListActivity), TransitionWay.DU)));
        }

        public override void OnBackPressed()
        {
            Finish();
        }

        private void ShowTutorial()
        {
            var tips = new List<SimpleTooltip>()
            {
                 new TooltipBuilder(this)
                {
                    AnchorView = this._progressBar,
                    Text = Resources.GetString(Resource.String.tut_exp_s1),
                    Gravity = GravityFlags.Bottom,
                }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                .Build(),
                 new TooltipBuilder(this)
                {
                    AnchorView = this._points[2],
                    Text = Resources.GetString(Resource.String.tut_exp_s2),
                    Gravity = GravityFlags.Top,
                }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                .Build(),
                  new TooltipBuilder(this)
                {
                    AnchorView = this._btnUser,
                    Text = Resources.GetString(Resource.String.tut_exp_s3),
                    Gravity = GravityFlags.Bottom,
                }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                .Build(),
                   new TooltipBuilder(this)
                {
                    AnchorView = this._btnQrCode,
                    Text = Resources.GetString(Resource.String.tut_exp_s4),
                    Gravity = GravityFlags.Top,
                }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                .Build(),
                    new TooltipBuilder(this)
                {
                    AnchorView = this._btnNavigate,
                    Text = Resources.GetString(Resource.String.tut_exp_s5),
                    Gravity = GravityFlags.Top,
                }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                .Build(),
                    new TooltipBuilder(this)
                {
                    AnchorView = this._btnNew,
                    Text = Resources.GetString(Resource.String.tut_exp_s6),
                    Gravity = GravityFlags.Top,
                }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                .Build(),
            };

            TooltipTutorialController.ShowTips(tips);
        }

        private void ShowWelcomeMessage()
        {
            var tips = new List<SimpleTooltip>()
            {
                 new TooltipBuilder(this)
                {
                    AnchorView = this._txtTxtAppExp,
                    Text = Resources.GetString(Resource.String.tut_exp_s0),
                    Gravity = GravityFlags.Top,
                }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                .Build()
            };

            ShowTipsAndSetFlagWhenFinish(tips, FlagStore, Shared.Constants.Flags.EXP_PAGE_TUTORIAL_SHOWN);
        }

        private void SetExpView(ExpData expData)
        {
            _progressBar.MinValue = expData.Min >= 0 ? expData.Min : 0;
            _progressBar.MaxValue = expData.CouponThresholdPoints;
            var currentValue = AD.Resolver.Resolve<Shared.Services.API.IStateApiService>().LastUserPoints;
            _progressBar.CurrentValue = currentValue >= expData.Min ? currentValue : expData.Min;
            _progressBar.HintText = GetString(Resource.String.exp_progress_bar_hint_msg) + expData.Level;
            _progressBar.ProgresTextSize = getProgressTextFontSize(expData.Current.ToString().Length);

            //TODO ANIMATION
            _progressBar.UpdateProgressWithAnimation(expData.Current, 2000);

            _txtMaxExp.Text = expData.CouponThresholdPoints.ToString();
            _txtTxtAppExp.Text = expData.Next.ToString();
            _txtTxtAppExp.SetTextSize(ComplexUnitType.Pt, getAppProgressTextSizes(expData.Next.ToString().Length));

            _txtPageMsg.Text = Resources.GetString(Resources.GetIdentifier(string.Format("exp_message_lv_{0}", expData.Level), "string", PackageName));
            var expColors = ExpColors.LevelColors[expData.Level - 1];
            var progressColor = new Color(expColors.ProgressColor);
            progressColor.A = 0xff;
            var progressBackColor = new Color(expColors.ProgressBackColor);
            progressBackColor.A = 0xff;
            var backColor = new Color(expColors.BackColor);
            backColor.A = 0xFF;
            _progressBar.Color = progressColor;
            _progressBar.BackgroundColor = progressBackColor;

            _container.SetBackgroundColor(backColor);

            for (int i = 0; i < _coupons.Count; i++)
            {
                if (i < expData.Coupon)
                    _coupons[i].Visibility = ViewStates.Visible;
                else
                    _coupons[i].Visibility = ViewStates.Gone;
            }

            LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams((int)Resources.GetDimension(Resource.Dimension.point_size), (int)Resources.GetDimension(Resource.Dimension.point_size));
            lp.Gravity = GravityFlags.CenterVertical;
            lp.Weight = 10;
            lp.LeftMargin = (int)Resources.GetDimension(Resource.Dimension.point_margin_left);
            for (int i = 0; i < _points.Count; i++)
            {
                if (i < expData.TotalPoints)
                {
                    _points[i].Visibility = ViewStates.Visible;
                    if (expData.Level == 1)
                        _points[i].LayoutParameters = lp;
                    if (i < expData.CheckedPoint)
                        _points[i].SetColorFilter(progressColor, PorterDuff.Mode.Multiply);
                    else
                        _points[i].SetColorFilter(Color.White, PorterDuff.Mode.Multiply);
                        
                }
                else {
                    _points[i].Visibility = ViewStates.Gone;
                }
            }

            if(expData.Current >= expData.Next)
                _txtAppExpBack.SetColorFilter(progressColor, PorterDuff.Mode.Multiply);
            else
                _txtAppExpBack.SetColorFilter(Color.White, PorterDuff.Mode.Multiply);
            FindViewById(Resource.Id.loading).Visibility = ViewStates.Gone;

        }

        private float getProgressTextFontSize(int length) {
            if (length >= 5)
                return Resources.GetDimension(Resource.Dimension.exp_progress_text_5d);
            if (length == 4)
                return Resources.GetDimension(Resource.Dimension.exp_progress_text_4d);
            return Resources.GetDimension(Resource.Dimension.exp_progress_text_3d);
        }

        private float getAppProgressTextSizes(int length)
        {
            if (length >= 5)
                return Resources.GetInteger(Resource.Integer.exp_app_max_text_5d);
            if (length == 4)
                return Resources.GetInteger(Resource.Integer.exp_app_max_text_4d);
            return Resources.GetInteger(Resource.Integer.exp_app_max_text_3d);
        }

        private IList<ExpData> AllStates => new List<ExpData>()
            {
                new ExpData {Current = 50, Next = 350, Min = 0, Level = 1, TotalPoints = 3, CheckedPoint = 0},
                new ExpData {Current = 700, Next = 800, Min = 600, Level = 2, Coupon = 0, TotalPoints = 4, CheckedPoint = 0},
                new ExpData {Current = 1300, Next = 2000, Min = 1200, Level = 3, Coupon = 0, TotalPoints = 5, CheckedPoint = 0},
                new ExpData {Current = 2300, Next = 5000, Min = 1800, Level = 4, Coupon = 0, TotalPoints = 5, CheckedPoint = 0},
                new ExpData {Current = 5300, Next = 10000, Min = 2400, Level = 5, Coupon = 0, TotalPoints = 6, CheckedPoint = 0},
            };

        private class ExpData
        {
            public int Current;
            public int Next;
            public int Min;
            public int Level;
            public int Coupon;
            public int TotalPoints;
            public int CheckedPoint;
            public int CouponThresholdPoints;
        }

        private class ExpColors
        {
            public static readonly List<ExpColors> LevelColors = new List<ExpColors>
            {
                new ExpColors {BackColor = 0x75DAD0, ProgressColor = 0xef2c2c, ProgressBackColor = 0x3fc8b8},
                new ExpColors {BackColor = 0xFFE345, ProgressColor = 0x3a81f1, ProgressBackColor = 0xeacc33},
                new ExpColors {BackColor = 0xF5AB34, ProgressColor = 0x9a00bb, ProgressBackColor = 0xe09927},
                new ExpColors {BackColor = 0xBAE051, ProgressColor = 0xef2c2c, ProgressBackColor = 0xa2cb44},
                new ExpColors {BackColor = 0xB2ACE8, ProgressColor = 0xfce332, ProgressBackColor = 0x9e93d5},
            };

            public int BackColor;
            public int ProgressColor;
            public int ProgressBackColor;
        }

        // HockeyApp Update Integration
        void CheckForUpdates()
        {
            // Remove this for store builds!
            UpdateManager.Register(this);
        }

        void UnregisterManagers()
        {
            UpdateManager.Unregister();
        }
    }
}