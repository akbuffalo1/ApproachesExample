using System;
using System.Collections.Generic;

namespace TigerApp.Shared.Services.API
{
    public interface IApiVersionConfig
    {
        void Update();
        string AppVersion { get; }
        int AppVersionCode { get; }
        string GetBaseApiUrlFromUrl(string path);
    }

    public class ApiVersion
    { 
        public string Slug { get; set; }
        public string Version { get; set; }
    }

    public abstract class BaseApiVersionConfig : IApiVersionConfig
    {
        protected string _lastUpdateAppVersion;
        private List<ApiVersion> _apiVersions;

        public BaseApiVersionConfig()
        {
            Update();
        }

        public string GetBaseApiUrlFromUrl(string path) {
            var apiVersion = _getApiVersionFromSlugAndAppVersion(path);
            return string.Format("/api/{0}", apiVersion);
        }

        public void Update()
        {
            _lastUpdateAppVersion = AppVersion;
            _updateApiVersion();
        }

        public virtual string  AppVersion {
            get;
            protected set;
        }

        public virtual int AppVersionCode
        {
            get;
            protected set;
        }

        protected string _getApiVersionFromSlugAndAppVersion(string slug) {
            if (!_lastUpdateAppVersion.Equals(AppVersion))
                Update();
            foreach (var api in _apiVersions) {
                if (slug.StartsWith(api.Slug, StringComparison.CurrentCulture))
                    return api.Version;
            }
            return "v1";
        }

        protected void _updateApiVersion() {
            _apiVersions = new List<ApiVersion>() {
                new ApiVersion(){
                    Slug = "/auth/fb",
                    Version = "v1"
                },
                new ApiVersion(){
                    Slug = "/auth/sms",
                    Version = "v1"
                },
                new ApiVersion(){
                    Slug = "/avatar",
                    Version = "v1"
                },
                new ApiVersion(){
                    Slug = "/coupon",
                    Version = "v1"
                },
                new ApiVersion(){
                    Slug = "/disposablecoupons",
                    Version = "v1"
                },
                new ApiVersion(){
                    Slug = "/mission",
                    Version = "v1"
                },
                new ApiVersion(){
                    Slug = "/device",
                    Version = "v1"
                },
                new ApiVersion(){
                    Slug = "/product",
                    Version = "v1"
                },
                new ApiVersion(){
                    Slug = "/me",
                    Version = "v1"
                },
                new ApiVersion(){
                    Slug = "/badges",
                    Version = "v1"
                },
                new ApiVersion(){
                    Slug = "/receipt",
                    Version = "v1"
                },
                new ApiVersion(){
                    Slug = "/state",
                    Version = "v1"
                },
                new ApiVersion(){
                    Slug = "/store",
                    Version = "v1"
                },
                new ApiVersion(){
                    Slug = "/checkin",
                    Version = "v1"
                },
                new ApiVersion(){
                    Slug = "/action/push/",
                    Version = "v1"
                },
            };
        }

    }
}
