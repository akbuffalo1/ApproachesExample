using System;
using AD;
using ReactiveUI;

namespace TigerApp.Shared.ViewModels
{
    public class ReactiveViewModel : ReactiveObject, ISupportsActivation, IViewModelBase
    {
        private readonly ViewModelActivator _viewModelActivator = new ViewModelActivator();
        protected readonly ILogger Logger;
        protected readonly IFlagStoreService _flagStoreService;

        public ReactiveViewModel(IFlagStoreService flagStoreService = null, ILogger logger = null)
        {
            Logger = logger ?? Resolver.Resolve<ILogger>();
            _flagStoreService = flagStoreService ?? Resolver.Resolve<IFlagStoreService>();
        }

        public ViewModelActivator Activator
        {
            get { return _viewModelActivator; }
        }

        public IFlagStoreService FlagStore { get { return _flagStoreService; } }
    }
}