using System;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

using AD;
using ReactiveUI;
using System.Net;
using AD.Plugins.Network.Rest;

namespace TigerApp
{
    public interface IApiServiceProvider
    {
        IApiClient Client { get; }
        string BaseServiceUri { get; }
        string ResourceUriForMethod(string methodName);
    }

    public class ApiServiceProvider : IApiServiceProvider
    {
        public IApiClient Client
        {
            get;
            protected set;
        }

        public string BaseServiceUri
        {
            get;
            protected set;
        }

        private readonly Dictionary<string, string> ResourceUriForMethodName = new Dictionary<string, string>();

        public string ResourceUriForMethod(string methodName)
        {
            try
            {
                return ResourceUriForMethodName[methodName];
            }
            catch
            {
                throw new ArgumentException($"{nameof(ApiResourcePathAttribute)} is missing on {methodName}");
            }
        }

        private void SetMethodUriMapFromAttributes()
        {
            var type = GetType();
            var methods = type.GetInterfaceMethods();

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<ApiResourcePathAttribute>();
                if (attr == null)
                    continue;
                ResourceUriForMethodName.Add(method.Name, attr.URI);
            }
        }

        private void SetBaseUriFromAttributes()
        {
            var type = GetType();
            var hieararchy = type.GetInterfaceHierarchy()
                .Select(i => i.GetCustomAttribute<ApiResourcePathAttribute>())
                .Where(i => i != null);

            var apiUri = "";
            foreach (var i in hieararchy)
            {
                if(!string.IsNullOrEmpty(i.URI))
                    apiUri = $"{apiUri}/{i.URI}";
            }
            var baseUri = AD.Resolver.Resolve<Shared.Services.API.IApiVersionConfig>().GetBaseApiUrlFromUrl(apiUri);
            BaseServiceUri = $"{baseUri}{apiUri}";
        }

        public ApiServiceProvider(IApiClient client = null)
        {
            Client = client ?? Resolver.Resolve<IApiClient>();
            SetMethodUriMapFromAttributes();
            SetBaseUriFromAttributes();
        }
    }

    public static class ApiServiceProviderEx
    {
        private readonly static Regex UriParamsRegex =
            new Regex("{([^}]*)}", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public static IObservable<TResponse> CreateFileCacheableObservableRequest<TResponse, TRequest>
        (this IApiServiceProvider self, TRequest req, Priority priority, string slug, Action<IObserver<TResponse>, Exception> catchStatusError = null, string verb = Verbs.Post, [CallerMemberName] string callerName = "")
        where TRequest : class
        {
            var retObservable = Observable.Create<TResponse>(obs =>
            {
                var fullUri = self.GetFullUri(callerName, req);

                self.Client.MakeFileCacheableRequest<TResponse>(priority, fullUri, slug, (obj) =>
                {
                    obs.OnNext(obj);
                    obs.OnCompleted();
                }, (ex) =>
                {
                    try
                    {
                        if (catchStatusError != null)
                        {
                            catchStatusError(obs, ex);
                        }
                        else {
                            obs.OnError(ex);
                        }
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

        public static IObservable<TResponse> CreateCacheableObservableRequest<TResponse, TRequest>
        (this IApiServiceProvider self, TRequest req, Priority priority, string slug, Action<string, TResponse> storeData, Func<string, TResponse> retrieveData, string verb = Verbs.Post, Action<IObserver<TResponse>, Exception> catchStatusError = null, [CallerMemberName] string callerName = "")
        where TRequest : class
        {
            var retObservable = Observable.Create<TResponse>(obs =>
            {
                var fullUri = self.GetFullUri(callerName, req);

                self.Client.MakeCacheableRequest<TResponse>(priority, fullUri, slug, (obj) =>
                {
                    obs.OnNext(obj);
                    obs.OnCompleted();
                }, (ex) =>
                {
                    try
                    {
                        if (catchStatusError != null)
                        {
                            catchStatusError(obs, ex);
                        }
                        else {
                            obs.OnError(ex);
                        }
                    }
                    catch (Exception propagatedEx)
                    {
                        obs.OnError(propagatedEx);
                    }
                }, storeData, retrieveData);

                return Disposable.Empty;
            });

            retObservable = retObservable.Catch(err =>
            {
                UserError.Throw(new UserError(err.Message));
            });

            return retObservable;
        }

        public static IObservable<TResponse> CreateFileCacheableObservableRequestWithoutCatch<TResponse, TRequest>
        (this IApiServiceProvider self, TRequest req, Priority priority, string slug, Action<IObserver<TResponse>, Exception> catchStatusError = null, string verb = Verbs.Post, [CallerMemberName] string callerName = "")
        where TRequest : class
        {
            var retObservable = CreateFileCacheableObservableRequest<TResponse, TRequest>(self, req, priority, slug, catchStatusError, verb, callerName);
                
            retObservable = retObservable.Catch(err =>
            {
                UserError.Throw(new UserError(err.Message));
            });

            return retObservable;
        }

        public static IObservable<TResponse> CreateObservableRequestWithoutCatch<TResponse, TRequest>
        (this IApiServiceProvider self, TRequest req, Action<IObserver<TResponse>, Exception> catchStatusError = null, string verb = Verbs.Post, [CallerMemberName] string callerName = "")
        where TRequest : class
        {
            var retObservable = Observable.Create<TResponse>(obs =>
            {
                var fullUri = self.GetFullUri(callerName, req);

                self.Client.MakeRequestFor<TResponse, TRequest>(fullUri, req, (obj) =>
                {
                    obs.OnNext(obj.Result);
                    obs.OnCompleted();
                }, (ex) =>
                {
                    try
                    {
                        if (catchStatusError != null)
                        {
                            catchStatusError(obs, ex);
                        }
                        else {
                            obs.OnError(ex);
                        }
                    }
                    catch (Exception propagatedEx)
                    {
                        obs.OnError(propagatedEx);
                    }
                }, verb);

                return Disposable.Empty;
            });

            return retObservable;
        }

        public static IObservable<TResponse> CreateObservableRequest<TResponse, TRequest>
        (this IApiServiceProvider self, TRequest req, Action<IObserver<TResponse>, Exception> catchStatusError = null, string verb = Verbs.Post, [CallerMemberName] string callerName = "")
        where TRequest : class
        {
            var retObservable = CreateObservableRequestWithoutCatch(self, req, catchStatusError, verb, callerName);

            retObservable = retObservable.Catch(err =>
            {
                UserError.Throw(new UserError(err.Message));
            });

            return retObservable;
        }

        public static string GetFullUri<TRequest>(this IApiServiceProvider self, string methodName, TRequest req)
        where TRequest : class
        {
            var methodUri = self.ResourceUriForMethod(methodName);
            var reqType = req.GetType();

            var calculatedUri = UriParamsRegex.Replace(methodUri, (match) =>
            {
                if (match.Groups.Count < 2)
                {
                    throw new FormatException($"{methodUri} URL is malformed!");
                }

                var matchedVal = match.Groups[1].Value;

                try
                {
                    var prop = reqType.GetProperty(matchedVal, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    var propVal = prop.GetValue(req);
                    return propVal.ToString();
                }
                catch
                {
                    var errorMsg = $"{matchedVal} property is missing on class {reqType.Name}";
                    throw new UriFormatException(errorMsg);
                }
            });

            return $"{self.BaseServiceUri}/{calculatedUri}";
        }
    }
}

