using System;
using ReactiveUI.Fody.Helpers;
using TigerApp.Shared.Services.API;
using TigerApp.Shared.Models;
using ReactiveUI;
using System.Collections.Generic;
using AD.Plugins.Network.Rest;

namespace TigerApp.Shared.ViewModels
{
    public interface IWallOfAvatarViewModel : IViewModelBase
    {
        List<Avatar> Avatars { get; }
        UserProfile Profile { get; }
        void GetUserAvatars();
        ReactiveCommand<object> UpdateAvatar { get; }
        bool IsBusy { get; }
        bool UpdateFinished { get; }
        string SelectedAvatarId { get; set; }
    }

    public class WallOfAvatarViewModel : ReactiveViewModel, IWallOfAvatarViewModel
    {
        private readonly IProfileApiService _profileApi;
        private readonly IAvatarsApiService _avatarApi;
        readonly ObservableAsPropertyHelper<bool> _isBusy;

        public WallOfAvatarViewModel(IProfileApiService profileApi, IAvatarsApiService avatarApi)
        {
            _profileApi = profileApi;
            _avatarApi = avatarApi;

            var canUpdateAvatar = this.WhenAny(vm => vm.SelectedAvatarId, id => !string.IsNullOrEmpty(id.Value));

            UpdateAvatar = ReactiveCommand.CreateAsyncObservable<object>(canUpdateAvatar, arg => { return _profileApi.UpdateProfileProperty(ProfilePropertyRequest.AvatarPath, SelectedAvatarId.ToString()); });
            UpdateAvatar.IsExecuting.ToProperty(this, x => x.IsBusy, out _isBusy);
            UpdateAvatar.SubscribeOnce(_ => { 
                UpdateFinished = true;
                _profileApi.GetUserInfo(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce(notUsed => { });
            });
        }

        [Reactive]
        public List<Avatar> Avatars
        {
            get;
            private set;
        }

        public UserProfile Profile
        {
            get;
            private set;
        }

        public ReactiveCommand<object> UpdateAvatar
        {
            get;
            protected set;
        }

        public bool IsBusy
        {
            get { return _isBusy.Value; }
        }

        [Reactive]
        public bool UpdateFinished
        {
            get;
            protected set;
        }

        [Reactive]
        public string SelectedAvatarId
        {
            get;
            set;
        }

        public void GetUserAvatars()
        {
            var updateNeeded = _profileApi.UpdateNeeded;
            var priority = updateNeeded ? Priority.Internet : Priority.Cache;
            if (_profileApi.UpdateNeeded)
            {
                _profileApi.GetUserInfo(Priority.Internet).SubscribeOnce(_ =>
                   _avatarApi.GetAvatars(priority).SubscribeOnce(avatars => { Avatars = avatars; })
                );
            }else{
                Profile = _profileApi.LastLoadedProfile;
                _avatarApi.GetAvatars(priority).SubscribeOnce(avatars => { Avatars = avatars; });
            }
        }
    }
}