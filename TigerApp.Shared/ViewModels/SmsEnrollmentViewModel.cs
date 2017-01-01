using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using System.Reactive.Linq;
using TigerApp.Shared.Models.Responses;
using TigerApp.Shared.Models.Requests;
using TigerApp.Shared.Utils;
using AD;

namespace TigerApp.Shared.ViewModels
{
    public interface ISmsEnrollmentViewModel : IViewModelBase
    {
        string PhoneNumber { get; }
        string VerificationCode { get; set; }
        IReactiveCommand PerformSmsVerification { get; }
        bool SignedInWithSms { get; }
    }

    public class SmsEnrollmentViewModel : ReactiveViewModel, ISmsEnrollmentViewModel
    {
        private readonly IAuthApiService _authService;
        private readonly ITDesAuthStore _tigerToken;

        public SmsEnrollmentViewModel(ITDesAuthStore tigerToken, IAuthApiService authApiService)
        {
            _authService = authApiService;
            _tigerToken = tigerToken;

            PhoneNumber = _authService.SmsLoginPhoneNumber;

            PerformSmsVerification = ReactiveCommand.CreateAsyncObservable((arg) =>
            {
                return Observable.Start(PerformSmsVerificationRequest);
            });
        }

        public string PhoneNumber
        {
            get;
            protected set;
        }

        [Reactive]
        public string VerificationCode
        {
            get;
            set;
        }

        [Reactive]
        public bool SignedInWithSms
        {
            get;
            protected set;
        }

        public IReactiveCommand PerformSmsVerification
        {
            get;
            protected set;
        }

        private void PerformSmsVerificationRequest()
        {
            _authService.SmsLogin(new SmsLoginRequest
            {
                phone_number = PhoneNumber,
                verify_code = VerificationCode,
            })
            .SubscribeOnce((SmsLoginResponse data) =>
            {
                if (data?.Token != null)
                {
                    var authData = _tigerToken.GetAuthData();
                    authData.AuthProviderToken = data?.Token;
                    authData.AuthProvider = "sms";
                    _tigerToken.SetAuthData(authData);
                    SignedInWithSms = true;
                    AD.Resolver.Resolve<Services.API.IProfileApiService>().UpdateUserLoginStatus(true);
                }
                _authService.SmsLoginPhoneNumber = null;
            });
        }
    }
}