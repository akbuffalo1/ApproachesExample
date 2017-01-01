using AD.Plugins.Network.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using TigerApp.Shared.Models;
using System.Reactive.Disposables;

namespace TigerApp.Shared.Services.API
{
    [ApiResourcePath("state")]
    public interface IStateApiService : IBaseApiService
    {
        [ApiResourcePath("")]
        IObservable<UserState> GetUserState();
        int LastUserPoints { get; }
        int LastUserLevel { get; }
    }

    public class StateApiService : ApiServiceProvider, IStateApiService
    {
        private UserState _lastState;
        private int _lastUserLevel = 0;
        public int LastUserPoints{
            get {
                return _lastState != null ? _lastState.Current.Points : 0;
            }
        }

        public int LastUserLevel { 
            get {
                GetUserState().SubscribeOnce(_ => { });//Update user state
                return _lastState != null ? _lastUserLevel : 0;
            }
        }

        public IObservable<UserState> GetUserState()
        {
            //return this.CreateFileCacheableObservableRequest<UserState, object>(new object(), Priority.Internet, "user_state.dat", verb: Verbs.Get);
            var retObservable = System.Reactive.Linq.Observable.Create<UserState>(obs =>
            {
                var fullUri = this.GetFullUri("GetUserState", new object());

                this.Client.MakeFileCacheableRequest<UserState>(Priority.Internet, fullUri, "user_state.dat", (obj) =>
                {
                    obs.OnNext(obj);
                    obs.OnCompleted();
                    if (_updateNeeded(obj)) {
                        var lastSavedLevel = _lastUserLevel;
                        _lastState = obj;
                        Int32.TryParse(obj.Current.Level.Replace("level-", string.Empty), out _lastUserLevel);
                        if(lastSavedLevel != _lastUserLevel)
                            AD.Resolver.Resolve<IProfileApiService>().GetUserInfo().SubscribeOnce(_ => { });
                        AD.Resolver.Resolve<IMissionApiService>().GetMissions(Priority.Internet).SubscribeOnce((mis) => { });
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
                            AD.Resolver.Resolve<Shared.Services.API.IProfileApiService>().UpdateUserLoginStatus(false);
                        }
                        obs.OnError(ex);
                    }
                    catch (Exception propagatedEx)
                    {
                        obs.OnError(propagatedEx);
                    }
                });

                return Disposable.Empty;
            });

            return retObservable;
        }

        private bool _updateNeeded(UserState newState) {
            if (_lastState == null)
                return true;
            if (!_lastState.Current.Level.Equals(newState.Current.Level))
                return true;
            if (_lastState.Current.MissionsCount != newState.Current.MissionsCount)
                return true;
            if (_lastState.Current.CouponCount != newState.Current.CouponCount)
                return true;
            if (_lastState.Current.Points != newState.Current.Points)
                return true;
            return false;
        }
    }
}
