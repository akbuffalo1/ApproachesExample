using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AD.Plugins.Network.Rest;
using TigerApp.Shared.Models;
using TigerApp.Shared.Models.Requests;

namespace TigerApp.Shared.Services.API
{
    [ApiResourcePath("me")]
    public interface IProfileApiService : IBaseApiService
    {
        bool IsLoggedIn { get; }

        bool UpdateNeeded { get; }

        void UpdateUserLoginStatus(bool isLogged);

        UserProfile LastLoadedProfile { get; }

        [ApiResourcePath("")]
        IObservable<UserProfile> GetUserInfo(Priority priority = Priority.Cache);

        [ApiResourcePath("")]
        IObservable<object> UpdateProfileProperty(string propertyPath, string propertyValue);

        [ApiResourcePath("")]
        IObservable<object> UpdateProfileProperties(UserProfileDataUpdateRequest properties);
    }

    public class ProfilePropertyRequest : ServerAction
    {
        public const string NicknamePath = "/nickname";
        public const string FirstNamePath = "/first_name";
        public const string LastNamePath = "/last_name";
        public const string EmailPath = "/email";
        public const string MobileNumberPath = "/mobile_number";
        public const string BirthdayPath = "/birthday";
        public const string AvatarPath = "/avatar";
        public const string TigerCityPath = "/city";

        public ProfilePropertyRequest(string propertyPath, string propertyValue)
        {
            Action = "replace";
            Path = propertyPath;
            Value = propertyValue;
        }
    }

    public class ProfileApiService : ApiServiceProvider, IProfileApiService
    {
        public bool IsLoggedIn
        {
            get;
            protected set;
        }

        public UserProfile LastLoadedProfile
        {
            get;
            protected set;
        }

        public IObservable<UserProfile> GetUserInfo(Priority priority = Priority.Cache)
        {
            var userInfoObs = Observable.Create<UserProfile>(obs =>
            {
                var fullUri = this.GetFullUri("GetUserInfo", new object());

                AD.Resolver.Resolve<AD.IApiClient>().MakeFileCacheableRequest<UserProfile>(priority, fullUri, "profile.dat", (newProfile) =>
                {
                    var stateService = AD.Resolver.Resolve<IStateApiService>();
                    IsLoggedIn = newProfile != null;
                    if (IsLoggedIn) {
                        if (stateService.LastUserLevel == 0)
                            stateService.GetUserState().SubscribeOnce(newState =>
                            {
                                _saveNewProfile(obs, newProfile, stateService);
                            });
                        else{
                            _saveNewProfile(obs, newProfile, stateService);
                        }
                    }
                        
                }, (ex) =>
                {
                    try
                    {
                        var isUnauthorized = false;

                        if (ex is BetterHttpResponseException)
                        {
                            isUnauthorized = ((BetterHttpResponseException)ex).StatusCode == System.Net.HttpStatusCode.Unauthorized || ((BetterHttpResponseException)ex).StatusCode == System.Net.HttpStatusCode.Forbidden;
                        }

                        if (!isUnauthorized)
                        {
                            throw ex;
                        }
                        else {
                            obs.OnNext(null);
                            obs.OnCompleted();
                            IsLoggedIn = false;
                        }
                    }
                    catch (Exception propagatedEx)
                    {
                        //obs.OnError(propagatedEx);
                    }
                });

                return Disposable.Empty;
            });
            userInfoObs.Catch((_) => { });
            return userInfoObs;
        }

        public bool UpdateNeeded {
            get {
                if (LastLoadedProfile == null || LastLoadedProfile.Level == null)
                    return true;
                int level = -1;
                if (Int32.TryParse(LastLoadedProfile.Level, out level))
                    _updateUserAvatar(LastLoadedProfile);
                return level < AD.Resolver.Resolve<IStateApiService>().LastUserLevel;
            }
        }

        private void _saveNewProfile(IObserver<UserProfile> obs, UserProfile newProfile, IStateApiService stateService)
        {
            newProfile.Level = stateService.LastUserLevel.ToString();
            LastLoadedProfile = newProfile;
            _updateUserAvatar(LastLoadedProfile);
            obs.OnNext(LastLoadedProfile);
            obs.OnCompleted();
        }

        private void _updateUserAvatar(UserProfile profile) {
            if (profile.Avatar == null)
                return;
            var avatarSlug = profile.Avatar.Slug.Replace("avatar-",string.Empty);
            var avatarType = avatarSlug.Substring(0, 1);
            var avatarLevel = avatarSlug.Substring(1, avatarSlug.Length -1);
            if (!avatarLevel.Equals(profile.Level)) {
                AD.Resolver.Resolve<IAvatarsApiService>().GetAvatars(Priority.Internet).SubscribeOnce(avatars => { 
                    var newAvatar = avatars.Find(a => a.Slug.StartsWith(string.Format("avatar-{0}", avatarType), StringComparison.CurrentCulture));
                    if (newAvatar != null) { 
                        profile.Avatar = newAvatar;
                        UpdateProfileProperty("avatar", newAvatar.Id);
                    }
                });
            }
        }

        public void UpdateUserLoginStatus(bool isLogged) {
            IsLoggedIn = isLogged;
        }

        public IObservable<object> UpdateProfileProperty(string propertyPath, string propertyValue)
        {
            var updateRequest = this.CreateObservableRequest<object, ProfilePropertyRequest>(
            new ProfilePropertyRequest(propertyPath, propertyValue),
            verb: Verbs.Patch);
            _localUpdate(propertyPath, propertyValue);
            return updateRequest;
        }

        public IObservable<object> UpdateProfileProperties(UserProfileDataUpdateRequest properties)
        { 
            var updateRequest = this.CreateObservableRequest<object, UserProfileDataUpdateRequest>(
                properties,
                verb: Verbs.Put);
            _localUpdate(properties);
            return updateRequest;
        }

        private void _localUpdate(UserProfileDataUpdateRequest properties)
        {
            LastLoadedProfile.NickName = properties.NickName;
            LastLoadedProfile.FirstName = properties.FirstName;
            LastLoadedProfile.LastName = properties.LastName;
            LastLoadedProfile.Email = properties.Email;
            LastLoadedProfile.MobileNumber = properties.MobileNumber;
            LastLoadedProfile.Birthday = properties.Birthday;
            LastLoadedProfile.TigerCity = properties.TigerCity;
        }

        private void _localUpdate(string propertyPath,string propertyValue) {
            switch (propertyPath) {
                case ProfilePropertyRequest.NicknamePath : 
                    LastLoadedProfile.NickName = propertyValue;
                    break;
                case ProfilePropertyRequest.FirstNamePath:
                    LastLoadedProfile.FirstName = propertyValue;
                    break;
                case ProfilePropertyRequest.LastNamePath:
                    LastLoadedProfile.LastName = propertyValue;
                    break;
                case ProfilePropertyRequest.EmailPath:
                    LastLoadedProfile.Email = propertyValue;
                    break;
                case ProfilePropertyRequest.MobileNumberPath:
                    LastLoadedProfile.MobileNumber = propertyValue;
                    break;
                case ProfilePropertyRequest.BirthdayPath:
                    LastLoadedProfile.Birthday = propertyValue;
                    break;
                case ProfilePropertyRequest.AvatarPath:
                    AD.Resolver.Resolve<IAvatarsApiService>().GetAvatars().SubscribeOnce(avatars => { 
                        LastLoadedProfile.Avatar = avatars.Find(a => a.Id.Equals(propertyValue));
                    });
                    break;
                case ProfilePropertyRequest.TigerCityPath:
                    LastLoadedProfile.TigerCity = propertyValue;
                    break;
            }
        }
    }
}
