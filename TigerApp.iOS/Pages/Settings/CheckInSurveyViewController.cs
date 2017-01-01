using Foundation;
using System;
using System.Reactive.Linq;
using UIKit;
using TigerApp.Shared.Services.API;

namespace TigerApp.iOS.Pages
{
    [Register("CheckInSurveyViewController")]
    public class CheckInSurveyViewController : UIViewController, IUIWebViewDelegate
    {
        private UIWebView _webView;
        private string _serverUrl;
        private string ServerUrl { 
            set {
                _serverUrl = value;
                _webView.LoadRequest(NSUrlRequest.FromUrl(new NSUrl(value)));
            }
        }

        public CheckInSurveyViewController(string storeId)
        {
            AD.Resolver.Resolve<IStoreApiService>().GetStoreList().SubscribeOnce(stores =>
            {
                var lastCheckinStores = stores != null ? stores.Find(store => store.Slug.Equals(storeId)) : null;
                if (lastCheckinStores != null)
                {
                    ServerUrl = string.Format(Shared.Constants.Forms.CheckinSurvey, lastCheckinStores.Address);
                }
                else {
                    AD.Resolver.Resolve<IStoreApiService>().GetStoreList(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce(updatedStores =>
                    {
                        lastCheckinStores = updatedStores.Find(store => store.Slug.Equals(storeId));
                        if (lastCheckinStores != null)
                        {
                            ServerUrl = string.Format(Shared.Constants.Forms.CheckinSurvey, lastCheckinStores.Address);
                        }
                    });
                }
            });
        }

        public override bool PrefersStatusBarHidden() => true;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _webView = new UIWebView();
            _webView.TranslatesAutoresizingMaskIntoConstraints = false;
            _webView.ScalesPageToFit = true;
            _webView.Delegate = this;

            View.Add(_webView);

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(_webView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(_webView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(_webView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(_webView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0)
            });
        }

        [Export("webViewDidFinishLoad:")]
        public void LoadingFinished(UIWebView webView)
        {
            if (webView.Request.Url.LastPathComponent == "formResponse" && webView.Request.HttpMethod == "POST")
            {
                Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe((obj) => { InvokeOnMainThread(() => { DismissViewController(true, null); }); });
            }
        }
    }
}