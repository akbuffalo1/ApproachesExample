using System;
using ReactiveUI;
using System.Reactive;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;
using TigerApp.Shared.Services.API;
using System.Collections.Generic;
using System.Linq;

namespace TigerApp.Shared.ViewModels
{
    public interface IEditProfileViewModel : IViewModelBase
    {
        string Nickname { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string MobileNumber { get; set; }
        string TigerCity { get; set; }
        List<string> TigerStoreCities { get; set; }
        //string Birthday { get; set; }
        DateTimeOffset? BirthdayDate { get; set; }
        bool MissionAlreadyCompleted { get; }
        bool UpdateFinished { get; }
        bool IsProfileComplete { get; }
        IReactiveCommand UpdateProfile { get; }
    }

    public class EditProfileViewModel : ReactiveViewModel, IEditProfileViewModel
    {
        [Reactive]
        public string Nickname
        {
            get;
            set;
        }

        [Reactive]
        public string MobileNumber
        {
            get;
            set;
        }

        [Reactive]
        public string FirstName
        {
            get;
            set;
        }

        [Reactive]
        public string LastName
        {
            get;
            set;
        }

        [Reactive]
        public string Email
        {
            get;
            set;
        }

        [Reactive]
        public string TigerCity
        {
            get;
            set;
        }

        [Reactive]
        public List<string> TigerStoreCities
        {
            get;
            set;
        }

        public string Birthday
        {
            get
            {
                if (BirthdayDate == null)
                    return null;

                return BirthdayDate.Value.ToString("d");
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    BirthdayDate = null;
                }
                else {
                    BirthdayDate = DateTimeOffset.Parse(value);
                }
            }
        }

        [Reactive]
        public bool UpdateFinished
        {
            get;
            protected set;
        }

        [Reactive]
        public bool MissionAlreadyCompleted
        {
            get;
            set;
        }

        public extern bool IsProfileComplete
        {
            [ObservableAsProperty]
            get;
        }

        public IReactiveCommand UpdateProfile
        {
            get;
            protected set;
        }

        [Reactive]
        public DateTimeOffset? BirthdayDate
        {
            get;
            set;
        }

        private string FormattedBirthdayString //server accept only date in format YYYY-MM-DD
        {
            get
            {
                var birthdayArray = Birthday.Split('/');
                if (birthdayArray.Length != 3)
                    return null;
                if (birthdayArray[1].Length == 1)
                    birthdayArray[1] = string.Format("0{1}",birthdayArray[1]);
                if (birthdayArray[0].Length == 1)
                    birthdayArray[0] = string.Format("0{1}", birthdayArray[0]);
                return string.Format("{0}-{1}-{2}", birthdayArray[2], birthdayArray[1], birthdayArray[0]);
            }
        }

        public EditProfileViewModel(IProfileApiService profileApiService)
        {
            this.WhenActivated(dis =>
            {
                dis(this.WhenAnyValue(
                    vm => vm.MobileNumber,
                    vm => vm.Nickname,
                    vm => vm.BirthdayDate,
                    vm => vm.FirstName,
                    vm => vm.LastName,
                    vm => vm.Email
                ).Select((Tuple<string, string, DateTimeOffset?, string, string, string> args) =>
                {
                    return args.Item1?.Length > 0 &&
                    args.Item2?.Length > 0 &&
                    args.Item3 != null &&
                    args.Item4?.Length > 0 &&
                    args.Item5?.Length > 0 &&
                    args.Item6?.Length > 0;
                }).ToPropertyEx(this, vm => vm.IsProfileComplete));

                dis(profileApiService.GetUserInfo(AD.Plugins.Network.Rest.Priority.Internet).Where(profile => profile != null).Subscribe(userProfile =>
                {
                    if (userProfile.MobileNumber?.Length > 0 &&
                    userProfile.Email?.Length > 0 &&
                    userProfile.Birthday?.Length > 0 &&
                    userProfile.FirstName?.Length > 0 &&
                    userProfile.LastName?.Length > 0 &&
                    userProfile.TigerCity.Length >0 &&
                    userProfile.NickName?.Length > 0)
                    {
                        MissionAlreadyCompleted = true;
                    }

                    MobileNumber = userProfile.MobileNumber;
                    Email = userProfile.Email;
                    Birthday = userProfile.Birthday;
                    FirstName = userProfile.FirstName;
                    LastName = userProfile.LastName;
                    Nickname = userProfile.NickName;
                    var lastCheckin = AD.Resolver.Resolve<IStoreCheckInApiService>().LastCheckin;
                    var lastCheckInCity = lastCheckin != null ? lastCheckin.Location.City.Name : string.Empty;
                    TigerCity = string.IsNullOrEmpty(userProfile.TigerCity) ? lastCheckInCity : userProfile.TigerCity;
                }));

                dis(AD.Resolver.Resolve<IStoreApiService>().GetStoreList().Where(stores => stores != null).Subscribe(stores =>
                {
                    var cities = stores.Select(s => s.Location.City.Name).Distinct().ToList();
                    cities.Sort();
                    TigerStoreCities = cities;
                }));
            });

            UpdateProfile = ReactiveCommand.CreateAsyncObservable(arg =>
            {
                var propertiesUpdateRequest = new Models.Requests.UserProfileDataUpdateRequest()
                {
                    NickName = Nickname,
                    Email = Email,
                    LastName = LastName,
                    FirstName = FirstName,
                    Birthday = FormattedBirthdayString,
                    MobileNumber = MobileNumber,
                    TigerCity = TigerCity
                };

                var obs = profileApiService.UpdateProfileProperties(propertiesUpdateRequest);

                obs.SubscribeOnce(test =>
                {
                    UpdateFinished = true;
                    profileApiService.GetUserInfo(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce(notUsed => { });
                });

                return obs;
            });

        }

    }
}

