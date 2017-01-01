using System;
using ReactiveUI;
using TigerApp.Shared.Models.Responses;

namespace TigerApp
{
    public interface IFacebookAuthService
    {
        string Token { get; }
        string UserId { get; }
        IObservable<bool> IsSignedIn { get; }
        IReactiveCommand<FacebookAuthResponse> SignIn { get; }
        IReactiveCommand<FacebookAuthResponse> SignOut { get; }
    }
}