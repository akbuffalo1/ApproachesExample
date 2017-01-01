using System;
using System.Collections.Generic;
using System.Text;
using AD.Plugins.Permissions;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace TigerApp.Shared.ViewModels
{
    public interface IEnableGeolocationViewModel : IViewModelBase
    {
        bool AskedForLocation { get; }
        IReactiveCommand<PermissionStatus> RequestLocationPermission { get; }
        PermissionStatus PermissionStatus { get; }
    }

    public class EnableGeolocationViewModel : ReactiveViewModel, IEnableGeolocationViewModel
    {
        public bool AskedForLocation => FlagStore.IsSet(Constants.Flags.ASKED_FOR_LOCATION);

        public EnableGeolocationViewModel(IPermissions permissions)
        {
            RequestLocationPermission = ReactiveCommand.CreateAsyncObservable(Observable.Return(true), (arg) =>
            {
                return Observable.StartAsync(async () =>
                {
                    FlagStore.Set(Constants.Flags.ASKED_FOR_LOCATION);
                    var dict = await permissions.RequestPermissionsAsync(Permission.Location);
                    return dict[Permission.Location];
                });
            });

            RequestLocationPermission.ToPropertyEx(this, _ => _.PermissionStatus, PermissionStatus.Unknown);
        }

        public extern PermissionStatus PermissionStatus
        {
            [ObservableAsProperty]
            get;
        }

        public IReactiveCommand<PermissionStatus> RequestLocationPermission
        {
            get;
            protected set;
        }
    }
}
