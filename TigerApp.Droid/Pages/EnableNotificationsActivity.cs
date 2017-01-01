using System;
using AD;
using AD.Plugins.Permissions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using ReactiveUI;
using TigerApp.Droid.Services.Platform;
using TigerApp.Shared;
using TigerApp.Shared.ViewModels;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "EnableNotificationsPage")]
    public class EnableNotificationsActivity : BaseReactiveActivity<IEnableNotificationsViewModel>
    {
        private Button _btnOK;
        static string TAG = nameof(EnableNotificationsActivity);

        public EnableNotificationsActivity()
        {
            this.WhenActivated(dis =>
            {
                dis(this.BindCommand(ViewModel, vm => vm.RequestRemoteNotifications, v => v._btnOK));
                dis(ViewModel.WhenAnyValue(vm => vm.PermissionStatus).Subscribe(permStatus =>
                {
                    Console.WriteLine("#############Perm Status : {0}",permStatus);
                    if (permStatus == AD.Plugins.Permissions.PermissionStatus.Granted)
                    {
                        Console.WriteLine("#############Perm Granted!!!!");
                        (Resolver.Resolve<INotificationsService>() as Services.Platform.NotificationsService)?.UnRegisterReceiver(this);
                        Finish();
                    }
                }));
            }, this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_enable_notifycations);
            _btnOK = FindViewById<Button>(Resource.Id.btnOk);
            _btnOK.Click += (sender, e) => { 
                StartNewActivity(typeof(EnableGeolocationActivity), TransitionWay.RL);
            };
            FindViewById<TextView>(Resource.Id.tvNotificationTitle).Text = Constants.Strings.EnableNotificationsMessage;
        }

        protected override void OnResume()
        {
            base.OnResume();
            (Resolver.Resolve<INotificationsService>() as Services.Platform.NotificationsService)?.RegisterReceiver(this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            //(Resolver.Resolve<INotificationsService>() as Services.Platform.NotificationsService)?.UnRegisterReceiver(this);
        }
    }
}