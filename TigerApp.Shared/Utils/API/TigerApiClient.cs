using System;
using AD;
using AD.Plugins.Network.Rest;
using AD.Plugins.TripleDesAuthToken;

namespace TigerApp.Shared.Utils.API
{
    public class TigerApiClient : ApiClient
    {
        private readonly ITigerToken _tigerToken;

        public TigerApiClient(
            ILogger logger,
            IHttpServerConfig config,
            IJsonConverter serializer,
            IJsonRestClient restClient,
            INetworkReachability networkReachability,
            IFileStore fileStore,
            ITigerToken tigerToken
        ) : base(logger, config, serializer, restClient, networkReachability, fileStore)
        {
            _tigerToken = tigerToken;
        }

        public override RestRequest CreateRequest(string path)
        {
            var request = base.CreateRequest(path);
            request.Headers["x-auth-token"] = AuthToken;
            return request;
        }

        public override RestRequest CreateRequest<T>(string path, T body)
        {
            var request = base.CreateRequest<T>(path, body);
            request.Headers["x-auth-token"] = AuthToken;
            return request;
        }

        public IAbortable MakePatchRequestFor<TResponse, TRequest>(string path, TRequest requestBody, Action<DecodedRestResponse<TResponse>> successAction, Action<Exception> errorAction) where TRequest : class
        {
            var request = CreateRequest<TRequest>(ServerConfig.BaseAddress + path, requestBody);
            request.Verb = "PATCH";
            return RestClient.MakeRequestFor<TResponse>(request, successAction, errorAction);
        }

        private string AuthToken => _tigerToken.TokenString;
    }
}

