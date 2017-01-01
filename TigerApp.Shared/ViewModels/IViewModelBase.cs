using System;

using System.Reactive.Subjects;

namespace TigerApp.Shared.ViewModels
{
    public interface IViewModelBase
    {
        IFlagStoreService FlagStore { get; }
    }
}