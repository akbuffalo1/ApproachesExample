using Android.OS;
using ReactiveUI;
using TigerApp.Shared.ViewModels;
using AD;
using AD.Droid;
using UK.CO.Chrisjenx.Calligraphy;
using System;
using System.Collections.Generic;
using AD.Plugins.Permissions;
using AD.Plugins.Permissions.Droid;
using Android.Content;
using Android.Content.PM;
using Android.Gestures;
using Android.Views;
using ReactiveUI.AndroidSupport;
using TigerApp.Droid.Pages;
using TigerApp.Droid.Tutorial;
using TigerApp.Droid.UI;
using TigerApp.Droid.UI.ToolTips;
using TigerApp.Shared;
using Permission = Android.Content.PM.Permission;
using TigerApp.Shared.Models;
using TigerApp.Shared.Models.PushNotifications;
using TigerApp.Shared.Services.API;
using Android.App;

namespace TigerApp.Droid
{
    public class BaseReactiveActivity<TViewModel> : ReactiveActionBarActivity<TViewModel>
        where TViewModel : class, IViewModelBase
    {
        public enum TransitionWay
        {
            Default,
            RL,
            LR,
            UD,
            DU,
        }
        protected ILogger Logger;
        protected event Action OnSwipeRight;
        protected event Action OnSwipeLeft;
        protected event Action OnSwipeUp;
        protected event Action OnSwipeDown;
        protected event Action OnClick;
        protected event Action OnDoubleClick;
        protected event Action OnLongClick;

        public IFlagStoreService FlagStore => _flagStoreService;

        private IFlagStoreService _flagStoreService;
        private bool _isRestarted;
        private int _slideEnterAnimation;
        private int _slideOutAnimation;
        private int _slideEnterRestartedAnimation;
        private int _slideOutRestartedAnimation;
        private TransitionWay _transitionWay = TransitionWay.Default;
        private TooltipTutorialController _tutorialController;
        private OnSwipeGestureListener _gestureListener;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //TODO Do we support all orientations?
            RequestedOrientation = ScreenOrientation.Portrait;

            _flagStoreService = Resolver.Resolve<IFlagStoreService>();

            AD.ApplicationBase<AppSetup>.CheckInitialized();
            Resolver.Resolve<IDependencyContainer>().Register<IAndroidGlobals>(new AndroidGlobals(ApplicationContext));

            Logger = Resolver.Resolve<ILogger>();

            ViewModel = Resolver.Resolve<TViewModel>();

            if (bundle != null)
            {
                _isRestarted = true;
                _transitionWay = (TransitionWay)bundle.GetInt(nameof(TransitionWay));
            }
            else
            {
                _transitionWay = (TransitionWay)Intent.GetIntExtra(nameof(TransitionWay), (int)TransitionWay.Default);
            }

            switch (_transitionWay)
            {
                case TransitionWay.LR:
                    SlideLeftRightPageTransitions();
                    break;
                case TransitionWay.RL:
                    SlideRightLeftPageTransitions();
                    break;
                case TransitionWay.DU:
                    SlideDownUpPageTransition();
                    break;
                case TransitionWay.UD:
                    SlideUpDownPageTransition();
                    break;
                case TransitionWay.Default:
                    FadeInPageTransitions();
                    break;
            }

            _gestureListener = new OnSwipeGestureListener(this);
            _gestureListener.OnSwipeRight += () => OnSwipeRight?.Invoke();
            _gestureListener.OnSwipeLeft += () => OnSwipeLeft?.Invoke();
            _gestureListener.OnSwipeUp += () => OnSwipeUp?.Invoke();
            _gestureListener.OnSwipeDown += () => OnSwipeDown?.Invoke();
            _gestureListener.OnClick += () => OnClick?.Invoke();
            _gestureListener.OnDoubleClick += () =>
            {
                //TODO remove TestActivity on production version
                #if DEBUG
                StartActivity(typeof(TestActivity));
                #endif

                OnDoubleClick?.Invoke();
            };
            _gestureListener.OnLongClick += () => OnLongClick?.Invoke();

            Window.DecorView.RootView.SetOnTouchListener(_gestureListener);
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            this.SetForegroundState(true);
            _isRestarted = true;
        }

        protected override void OnPause()
        {
            base.OnPause();
            this.SetForegroundState(false);
        }

        protected override void OnStart()
        {
            base.OnStart();
            this.SetForegroundState(true);
            if (_isRestarted)
                OverridePendingTransition(_slideEnterRestartedAnimation, _slideOutRestartedAnimation);
            else
                OverridePendingTransition(_slideEnterAnimation, _slideOutAnimation);
        }

        public override Window Window
        {
            get
            {
                return base.Window;
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt(nameof(TransitionWay), (int)_transitionWay);
            base.OnSaveInstanceState(outState);
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

        public override void Finish()
        {
            _tutorialController?.Cancel();

            base.Finish();
            GC.Collect();
        }

        public override void FinishActivity(int requestCode)
        {
            base.FinishActivity(requestCode);
            GC.Collect();
        }

        protected void StartNewActivity(Type type, TransitionWay from)
        {
            Intent intent = new Intent(this, type);
            StartNewActivity(intent, from);
        }

        protected void StartNewActivity(Intent intent, TransitionWay from)
        {
            switch (from)
            {
                case TransitionWay.LR:
                    SlideLeftRightPageTransitions();
                    break;
                case TransitionWay.RL:
                    SlideRightLeftPageTransitions();
                    break;
                case TransitionWay.DU:
                    SlideDownUpPageTransition();
                    break;
                case TransitionWay.UD:
                    SlideUpDownPageTransition();
                    break;
                case TransitionWay.Default:
                    FadeInPageTransitions();
                    break;
            }

            intent.PutExtra(nameof(TransitionWay), (int)from);
            StartActivity(intent);
        }

        private void FadeInPageTransitions()
        {
            _slideEnterAnimation = Resource.Animation.abc_fade_in;
            _slideOutAnimation = Resource.Animation.abc_fade_out;
            _slideEnterRestartedAnimation = Resource.Animation.abc_fade_in;
            _slideOutRestartedAnimation = Resource.Animation.abc_fade_out;
        }

        private void SlideRightLeftPageTransitions()
        {
            _slideEnterAnimation = Resource.Animation.slide_in_left;
            _slideOutAnimation = Resource.Animation.slide_out_left;
            _slideEnterRestartedAnimation = Resource.Animation.slide_in_right;
            _slideOutRestartedAnimation = Resource.Animation.slide_out_right;
        }

        private void SlideLeftRightPageTransitions()
        {
            _slideEnterAnimation = Resource.Animation.slide_in_right;
            _slideOutAnimation = Resource.Animation.slide_out_right;
            _slideEnterRestartedAnimation = Resource.Animation.slide_in_left;
            _slideOutRestartedAnimation = Resource.Animation.slide_out_left;
        }

        private void SlideDownUpPageTransition()
        {
            _slideEnterAnimation = Resource.Animation.slide_in_down;
            _slideOutAnimation = Resource.Animation.slide_out_up;
            _slideEnterRestartedAnimation = Resource.Animation.slide_in_up;
            _slideOutRestartedAnimation = Resource.Animation.slide_out_down;
        }

        private void SlideUpDownPageTransition()
        {
            _slideEnterAnimation = Resource.Animation.slide_in_up;
            _slideOutAnimation = Resource.Animation.slide_out_down;
            _slideEnterRestartedAnimation = Resource.Animation.slide_in_down;
            _slideOutRestartedAnimation = Resource.Animation.slide_out_up;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            var perm = (PermissionsDroid)AD.Resolver.Resolve<IPermissions>();
            perm.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected void ShowTipsAndSetFlagWhenFinish(List<SimpleTooltip> tips,
            IFlagStoreService flagStore, string flag)
        {
            _tutorialController = TooltipTutorialController.ShowTipsAndSetFlagWhenFinish(tips, FlagStore, flag);
            _tutorialController.OnComplete += OnTutorialComplete;
        }

        private void OnTutorialComplete()
        {
            if (_tutorialController == null)
                return;
            _tutorialController.OnComplete -= OnTutorialComplete;
            _tutorialController = null;
        }

        protected void CheckProfile()
        {
            AD.Resolver.Resolve<Shared.Services.API.IProfileApiService>().GetUserInfo(AD.Plugins.Network.Rest.Priority.Internet).Subscribe(profile =>
           {
               if (profile == null || profile.Equals(UserProfile.Empty))
               {
                    SetStartPageForUnauthorized();
               }
               else {
                   if (profile.Avatar == null)
                       StartNewActivity(typeof(WallOfAvatarActivity), TransitionWay.RL);
                   else if (string.IsNullOrEmpty(profile.NickName))
                       StartNewActivity(typeof(NicknameEnrollmentActivity), TransitionWay.RL);
                   else
                       StartNewActivity(typeof(ExpHomeActivity), TransitionWay.RL);
               }
               Finish();
           });
        }

        protected void SetStartPageForUnauthorized()
        {
            var flagStorageService = AD.Resolver.Resolve<IFlagStoreService>();
            var TAG = "";
            flagStorageService.ExecuteIfNotSet(Constants.Flags.ASKED_FOR_NOTIFICATIONS, () =>
           {
               Logger.Debug(TAG, "starting EnablePush, perm not granted");
               StartActivity(typeof(EnableNotificationsActivity));
           });

            flagStorageService.ExecuteIfSet(Constants.Flags.ASKED_FOR_NOTIFICATIONS, async () =>
            {
                var status = await AD.Resolver.Resolve<IPermissions>().CheckPermissionStatusAsync(AD.Plugins.Permissions.Permission.Location);

                flagStorageService.ExecuteIfSet(Constants.Flags.ASKED_FOR_LOCATION, () =>
                {
                    if (status == AD.Plugins.Permissions.PermissionStatus.Granted)
                    {
                        Logger.Debug(TAG, "starting Home");
                        StartActivity(typeof(HomeActivity));
                    }
                    else
                    {
                        Logger.Debug(TAG, "starting EnableGeo, perm not granted");
                        StartActivity(typeof(EnableGeolocationActivity));
                    }
                });

                flagStorageService.ExecuteIfNotSet(Constants.Flags.ASKED_FOR_LOCATION, () =>
                {
                    Logger.Debug(TAG, "starting EnableGeo");
                    StartActivity(typeof(EnableGeolocationActivity));
                });
            });
        }

        /*public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus)
            {
                var flags = Android.Views.SystemUiFlags.LayoutStable
                       | Android.Views.SystemUiFlags.LayoutHideNavigation
                       | Android.Views.SystemUiFlags.LayoutFullscreen
                       | Android.Views.SystemUiFlags.HideNavigation
                       | Android.Views.SystemUiFlags.Fullscreen
                       | Android.Views.SystemUiFlags.ImmersiveSticky;
                Window.DecorView.SystemUiVisibility = (Android.Views.StatusBarVisibility)flags;
            }
        }*/

       
    }

    public static class ActivityPushNotificationsEx
    {
        private static List<SimpleTooltip> _notifcations;
        private static bool _isInForeground;
        public static void HandleNotification(this Android.App.Activity activity,  GCMPushNotification notification)
        {
            _showNotification(activity, notification);

            var expHomeActivty = activity as ExpHomeActivity;
            if (expHomeActivty != null)
                expHomeActivty.ViewModel.GetUserState();
            else
                AD.Resolver.Resolve<Shared.Services.API.IStateApiService>().GetUserState().SubscribeOnce(_ => { });
        }

        private static void _showNotification(Android.App.Activity activity, GCMPushNotification notification)
        {
            var tigerNotification = notification as TigerPushNotification;
            var showDefault = true;
            if (tigerNotification != null) {
                if (tigerNotification.NotificationType == TigerNotificationsType.Badge)
                {
                    showDefault = false;
                    _showBadgeNotification(activity, tigerNotification);
                }
                else if (tigerNotification.NotificationType == TigerNotificationsType.Coupon)
                {
                    showDefault = false;
                    _showCouponNotification(activity, tigerNotification);
                }
                else if (tigerNotification.NotificationType == TigerNotificationsType.CheckInSurvey)
                {
                    var storeId = (tigerNotification as CheckInSurveyPushNotification).StoreId;
                    showDefault = string.IsNullOrEmpty(storeId);
                    if (!showDefault)
                    {
                        _showCheckInSurveyNotification(activity, tigerNotification);
                    }
                }
                else if (tigerNotification.NotificationType == TigerNotificationsType.LevelUp)
                {
                    showDefault = false;
                    _showLevelUpNotification(activity, (tigerNotification as LevelUpPushNotification));
                }

            }
            if (showDefault) { 
                _notifcations = new List<SimpleTooltip>() { new TooltipBuilder(activity)
                    {   
                        RootView = activity.Window.DecorView.RootView,
                        Text = notification.Message,
                        Gravity = GravityFlags.Top,
                    }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                    .Build()
                };
                TooltipTutorialController.ShowTips(_notifcations);
            }
        }

        static void _showLevelUpNotification(Activity activity,LevelUpPushNotification notification)
        {
            var profileService = AD.Resolver.Resolve<IProfileApiService>();
            profileService.GetUserInfo(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce(profile =>
            {
                if (profile != null)
                {
                    int level = 0;
                    if (Int32.TryParse(profile.Level, out level ))
                    {
                        var avatarSlug = profile.Avatar.Slug.Replace("avatar-", string.Empty);
                        var avatarType = avatarSlug.Substring(0, 1);
                        AD.Resolver.Resolve<IAvatarsApiService>().GetAvatars(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce(avatars =>
                        {
                            var newAvatar = avatars.Find(a => a.Slug.StartsWith(string.Format("avatar-{0}", avatarType), StringComparison.CurrentCulture));
                            if (newAvatar != null)
                                
                            {
                                activity.RunOnUiThread(() =>
                                {
                                    var avatarUrl = string.Format("{0}{1}", AD.Resolver.Resolve<IHttpServerConfig>().BaseAddress, profile.Avatar.ImageUrl);
                                    CardFactory.ShowLevelUpCardDialog(activity, level, avatarUrl);
                                });
                            }
                        });
                    }
                }
            });
        }

        static void _showCheckInSurveyNotification(Activity activity, TigerPushNotification tigerNotification)
        {
            activity.RunOnUiThread(() =>
                                    {
                                        AlertDialog alertDialog = null;
                                        var alert = new AlertDialog.Builder(activity);
                                        alert.SetMessage(tigerNotification.Message);
                                        alert.SetPositiveButton("Vai al sondaggio", (sender, e) =>
                                        {
                                            if (alertDialog != null)
                                                alertDialog.Dismiss();
                                            var intent = new Intent(activity, typeof(CheckInSurveyActivity));
                                            intent.PutExtra(CheckInSurveyActivity.StoreIdIntentKey, (tigerNotification as CheckInSurveyPushNotification).StoreId);
                                            activity.StartActivity(intent);
                                        });
                                        alert.SetNegativeButton("Annulla", (sender, e) =>
                                        {
                                            if (alertDialog != null)
                                                alertDialog.Dismiss();
                                        });
                                        alertDialog = alert.Show();
                                    });
        }

        static void _showCouponNotification(Activity activity, TigerPushNotification tigerNotification)
        {
            var isSpecial = (tigerNotification as DiscountCouponPushNotification) == null;
            int amount = 0;
            if (!isSpecial)
                Int32.TryParse((tigerNotification as DiscountCouponPushNotification).Amount, out amount);
            var couponBubble = new TooltipBuilder(activity).CreateCouponBubble(tigerNotification.NickName, tigerNotification.Message, amount, isSpecial);
            couponBubble.RootView = activity.Window.DecorView.RootView;
            couponBubble.Gravity = GravityFlags.Center;
            TooltipTutorialController.ShowTips(new List<SimpleTooltip>() { couponBubble.Build() });
        }

        static void _showBadgeNotification(Activity activity, TigerPushNotification tigerNotification)
        {
            AD.Resolver.Resolve<IBadgesApiService>().GetBadges(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce(badges =>
                                {
                                    var badge = badges.Find(b => b.Name.Equals((tigerNotification as BadgePushNotification).BadgeName));
                                    if (badge != null)
                                        activity.RunOnUiThread(() =>
                                        {
                                            CardFactory.ShowBadgeCardDialog(activity, badge);
                                        });
                                });
        }

        public static void SetForegroundState (this Android.App.Activity activity, bool isInForeground)
        {
            _isInForeground = isInForeground;
        }

        public static bool GetForegroundState(this Android.App.Activity activity)
        {
            return _isInForeground;
        }
    }
}