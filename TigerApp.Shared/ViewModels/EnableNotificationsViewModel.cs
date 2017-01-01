using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI.Fody.Helpers;
using AD.Plugins.Permissions;
using System.ComponentModel;

namespace TigerApp.Shared.ViewModels
{
    public interface IEnableNotificationsViewModel : IViewModelBase
    {
        IReactiveCommand RequestRemoteNotifications { get; }
        PermissionStatus PermissionStatus { get; }
    }

    public class EnableNotificationsViewModel : ReactiveViewModel, IEnableNotificationsViewModel
    {
        private readonly INotificationsService PermissionsService;

        public EnableNotificationsViewModel(INotificationsService permissionsService)
        {
            PermissionsService = permissionsService;

            PermissionsService.WhenAnyValue(_ => _.PermissionStatus).ToPropertyEx(this, _ => _.PermissionStatus, PermissionStatus.Unknown);
        }

        public extern PermissionStatus PermissionStatus
        {
            [ObservableAsProperty]
            get;
        }

        IReactiveCommand IEnableNotificationsViewModel.RequestRemoteNotifications
        {
            get
            {
                return PermissionsService.RequestPermissions;
            }
        }
    }
}