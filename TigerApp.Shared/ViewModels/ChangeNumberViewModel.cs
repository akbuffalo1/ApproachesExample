using System;
using System.Reactive.Linq;
using AD;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TigerApp.Shared.ViewModels
{
    public interface IChangeNumberViewModel : IViewModelBase
    {
        IReactiveCommand PerformApplyPress { get; }
    }

    public class ChangeNumberViewModel : ReactiveViewModel, IChangeNumberViewModel
    {
        [Reactive]
        public IReactiveCommand PerformApplyPress
        {
            get;
            protected set;
        }

        public ChangeNumberViewModel()
        {
            PerformApplyPress = ReactiveCommand.CreateAsyncObservable((arg) =>
            {
                return Observable.Start(() => {
                    // TODO: Implement number save logic
                });
            });
        }
    }
}