using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TigerApp.Shared.Services.API;
using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Disposables;

namespace TigerApp.Shared.ViewModels
{
    public interface INicknameEnrollmentViewModel : IViewModelBase
    {
        IReactiveCommand UpdateNickname { get; }
        bool NicknameUpdated { get; }
        string UserNickname { get; set; }
        string AvatarImageUrl { get; }
    }

    public class NicknameEnrollmentViewModel : ReactiveViewModel, INicknameEnrollmentViewModel
    {
        public IReactiveCommand UpdateNickname
        {
            get;
            protected set;
        }

        [Reactive]
        public bool NicknameUpdated
        {
            get;
            protected set;
        }

        [Reactive]
        public string UserNickname
        {
            get;
            set;
        }

        [Reactive]
        public string AvatarImageUrl
        {
            get;
            protected set;
        }

        public NicknameEnrollmentViewModel(IProfileApiService profileService)
        {
            UpdateNickname = ReactiveCommand.CreateAsyncObservable<object>((arg) =>
            {
                var obs = profileService.UpdateProfileProperty(ProfilePropertyRequest.NicknamePath, UserNickname);

                obs.SubscribeOnce(data =>
                {
                    FlagStore.Unset(Constants.Flags.EXP_PAGE_TUTORIAL_SHOWN);
                    NicknameUpdated = true;
                });

                return obs;
            });
            if (profileService.LastLoadedProfile == null || profileService.LastLoadedProfile.Avatar == null)
            {
                profileService.GetUserInfo().SubscribeOnce((profile) =>
                {
                    if (profile.Avatar != null)
                        AvatarImageUrl = AD.Resolver.Resolve<AD.IHttpServerConfig>().BaseAddress + profile.Avatar.ImageUrl;
                    else {
                        profileService.GetUserInfo(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce((updatedProfile) =>
                        {
                            if (profile.Avatar != null)
                                AvatarImageUrl = AD.Resolver.Resolve<AD.IHttpServerConfig>().BaseAddress + profile.Avatar.ImageUrl;
                        });
                    }
                });
            }else{
                AvatarImageUrl = AD.Resolver.Resolve<AD.IHttpServerConfig>().BaseAddress + profileService.LastLoadedProfile.Avatar.ImageUrl;
            }
        }
    }
}
