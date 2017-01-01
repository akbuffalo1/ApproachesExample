using System;
using System.Reactive;
using AD.Plugins.Permissions;
using ReactiveUI;

namespace TigerApp
{
    public interface INotificationsService : IHandleObservableErrors
    {
        PermissionStatus PermissionStatus { get; }
        IReactiveCommand RequestPermissions { get; }
    }
}