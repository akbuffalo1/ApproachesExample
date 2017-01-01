using System;
using ReactiveUI.Fody.Helpers;
using TigerApp.Shared.Services.API;
using System.Collections.Generic;
using TigerApp.Shared.Models;
using System.Linq;
using ReactiveUI;
using AD;

namespace TigerApp.Shared.ViewModels
{
    public interface IProfileViewModel : IViewModelBase
    {
        bool ShouldShowTutorial { get; }
        string Level { get; }
        string Status { get; }
        string Username { get; }
        string ProfileImageUrl { get; }
        List<Badge> Badges { get; }
    }

    public class ProfileViewModel : ReactiveViewModel, IProfileViewModel
    {
        const int MAX_BADGES = 10;
        const string ErrorMessage = "Si è verificato un errore durante il caricamento del profilo!";

        [Reactive]
        public List<Badge> Badges
        {
            get;
            protected set;
        }

        [Reactive]
        public string Level
        {
            get;
            protected set;
        }

        [Reactive]
        public string Status
        {
            get;
            protected set;
        }

        [Reactive]
        public string Username
        {
            get;
            protected set;
        }

        public const string PLACEHOLDER_PROFILE_IMAGE_URL = "DinoPlaceholder";

        [Reactive]
        public string ProfileImageUrl
        {
            get;
            protected set;
        } = PLACEHOLDER_PROFILE_IMAGE_URL;

        public ProfileViewModel(IProfileApiService profileApiService, IBadgesApiService badgesService, IDialogProvider dialogProvider)
        {
            this.WhenActivated(dis =>
            {
                if (!profileApiService.UpdateNeeded) { 
                    UpdateUserInfo(profileApiService.LastLoadedProfile);
                    badgesService.GetBadges().SubscribeOnce((badges) => {
                        UpdateBadges(badges);
                        badgesService.GetBadges(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce(_=> { }/*UpdateBadges*/);
                    });
                }
                else { 
                    dis(profileApiService.GetUserInfo(AD.Plugins.Network.Rest.Priority.Internet).Subscribe(userInfo =>
                        {
                            if (userInfo == null)
                            {
                                dialogProvider.DisplayError(ErrorMessage);
                            }
                            else
                            {
                                UpdateUserInfo(userInfo);
                                badgesService.GetBadges(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce(UpdateBadges);
                            } 
                        }));
                }
            });
        }

        private void UpdateUserInfo(UserProfile userInfo)
        {
            ProfileImageUrl = userInfo.Avatar?.ImageUrl ?? PLACEHOLDER_PROFILE_IMAGE_URL;

            Username = userInfo.NickName ?? "";
            if (userInfo.Level.StartsWith("level-", StringComparison.CurrentCulture))
                userInfo.Level = userInfo.Level.Replace("level-", String.Empty);
            Level = $"Liv: {userInfo.Level ?? ""}";
            _setUserStatus(userInfo.Level);

            var badges = userInfo.Badges ?? new List<Badge>();
        }

        private void UpdateBadges(List<Badge> badges) 
        { 
            badges.AddRange(Enumerable.Range(0, MAX_BADGES - badges.Count).Select(i => new Badge
            {
                ImageUrl = "no.png"
            }));

            Badges = badges;
        }

        public bool ShouldShowTutorial => !FlagStore.IsSet(Constants.Flags.PROFILE_PAGE_TUTORIAL_SHOWN);

        private void _setUserStatus(string level) {
            switch (level) {
                case "1": {
                        Status = "TIGER PUPPY";
                }break;
                case "2": {
                        Status = "TIGER CUB";
                }break;
                case "3":{
                        Status = "YOUNG TIGER";
                }break;
                case "4":{
                        Status = "WILD TIGER";
                }break;
                case "5":{
                        Status = "ROARING TIGER";
                }break;
                case "6":{
                        Status = "FLYING TIGER";
                }break;
            }
        }
    }
}