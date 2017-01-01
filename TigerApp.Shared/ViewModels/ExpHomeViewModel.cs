using System;
using ReactiveUI.Fody.Helpers;
using TigerApp.Shared.Models;
using TigerApp.Shared.Services.API;

namespace TigerApp.Shared.ViewModels
{
    public interface IExpHomeViewModel : IViewModelBase
    {
        bool ShouldShowTutorial { get; }
        UserProfile Profile { get; }
        UserState State { get; }
        void GetUserInfo();
        void GetUserState();
        string Error { get; }
    }

    public class ExpHomeViewModel : ReactiveViewModel, IExpHomeViewModel
    {
        private readonly IProfileApiService _profileApi;
        private readonly IStateApiService _stateApi;

        public ExpHomeViewModel(IProfileApiService profileApi, IStateApiService stateApi)
        {
            _profileApi = profileApi;
            _stateApi = stateApi;
        }

        [Reactive]
        public UserProfile Profile
        {
            get;
            private set;
        }

        public bool ShouldShowTutorial => !FlagStore.IsSet(Constants.Flags.EXP_PAGE_TUTORIAL_SHOWN);

        [Reactive]
        public UserState State
        {
            get;
            private set;
        }

        public void GetUserInfo()
        {
            _profileApi.GetUserInfo()
                       .SubscribeOnce(profile => Profile = profile ?? UserProfile.Empty); // Used because of no emitted value during setting that to null
        }

        public void GetUserState()
        {
            _stateApi.GetUserState()
                     .Catch(ex => Error = ex.Message)
                     .SubscribeOnce(state => State = state);
        }

        [Reactive]
        public string Error
        {
            get;
            private set;
        }
    }
}