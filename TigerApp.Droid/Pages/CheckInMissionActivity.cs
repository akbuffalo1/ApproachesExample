#region using

using System;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using AD.Plugins.Permissions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using TigerApp.Droid.Utils;
using TigerApp.Shared;
using TigerApp.Shared.Services.Platform;
using TigerApp.Shared.ViewModels;

#endregion

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "CheckInMissionActivity")]
    public class CheckInMissionActivity : BaseReactiveActivity<ICheckInMissionViewModel>
    {
        private bool _permissionDialogShowed;
        private ILocationService _locationService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_check_in_mission);

            var back = FindViewById<ImageView>(Resource.Id.btnActionBarBack);
            back.Click += delegate { Finish(); };

            FindViewById<TextView>(Resource.Id.txtActionBarTitle).Text = Constants.Strings.CheckInMissionPageTitle;
            FindViewById<TextView>(Resource.Id.txtCheckInMissionMessage).Text =
                Constants.Strings.CheckInMissionPageMessage;

            var completeCommand = FindViewById<Button>(Resource.Id.btnCheckInMissionCompleteCommand);
            completeCommand.Click += delegate { TryToCompleteMission(); };


            _locationService = AD.Resolver.Resolve<ILocationService>();
        }

        private async void TryToCompleteMission()
        {
            var locationEnabled = await CheckLocationEnabled();
            if (!locationEnabled)
                return;

            Shared.Models.Location userLocation = null;
            try
            {
                userLocation = await _locationService.CurrentLocationAsync();
            }
            catch (Shared.Services.Platform.GpsTimeoutException ex)
            {
                AlertDialog alertDialog = null;
                var alert = new AlertDialog.Builder(this);
                alert.SetMessage(Shared.Constants.Strings.GpsTimeoutMessage);
                alert.SetPositiveButton("Riprova", (sender, e) =>
                {
                    if (alertDialog != null)
                        alertDialog.Dismiss();
                    TryToCompleteMission();
                });
                alert.SetNegativeButton("Annulla", (sender, e) =>
                {
                    if (alertDialog != null)
                        alertDialog.Dismiss();
                    Finish();
                });
                alertDialog = alert.Show();
            }
            finally {
                if (userLocation != null) { 
                    ViewModel.TryToCompleteMission.ExecuteAsync(userLocation).SubscribeOnce(missionStatus =>
                    {
                        if (missionStatus == MissionStatus.Completed)
                        {
                            StartNewActivity(typeof(ExpHomeActivity), TransitionWay.RL);
                            Finish();
                        }
                        else if (missionStatus == MissionStatus.Failed)
                        {
                            StartNewActivity(typeof(CheckInMissionTooFarActivity), TransitionWay.RL);
                        }
                    });
                }
            }
            /*
            _locationService.CurrentLocationAsync().ToObservable().SubscribeOnce(userLocation =>
            {
                ViewModel.TryToCompleteMission.ExecuteAsync(userLocation).SubscribeOnce(missionStatus =>
                {
                    if (missionStatus == MissionStatus.Completed)
                    {
                        StartNewActivity(typeof(ExpHomeActivity), TransitionWay.RL);
                        Finish();
                    }
                    else if (missionStatus == MissionStatus.Failed)
                    {
                        StartNewActivity(typeof(CheckInMissionTooFarActivity), TransitionWay.RL);
                    }
                });
            });*/
        }

        private async Task<bool> CheckLocationEnabled()
        {
            var perm = AD.Resolver.Resolve<IPermissions>();

            var status = await perm.CheckPermissionStatusAsync(Permission.Location);

            if (status != PermissionStatus.Granted)
            {
                if (!_permissionDialogShowed)
                {
                    _permissionDialogShowed = true;
                    DialogUtil.ShowAlert(this, Resources.GetString(Resource.String.msg_loc_you_have_to_enable), "Ok",
                        () => { perm.RequestPermissionsAsync(Permission.Location); });
                }
                return false;
            }

            if (!_locationService.IsLocationEnabled)
            {
                DialogUtil.ShowConfirmation(this,
                    Resources.GetString(Resource.String.msg_location_disabled_please_enable),
                    () =>
                    {
                        var callGpsSettingIntent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                        StartActivity(callGpsSettingIntent);
                    });
                return false;
            }

            return true;
        }
    }
}