using System;
using System.Reactive.Linq;
using Foundation;
using UIKit;

namespace TigerApp.iOS.Pages.Missions.SurveyMission
{
    public partial class SurveyMissionGoogleFormViewController : UIViewController, IUIWebViewDelegate
    {
        private readonly string FormUrl;
        private readonly Action OnFormSubmitted;

        public SurveyMissionGoogleFormViewController(string url, Action onSubmit) : base("SurveyMissionGoogleFormViewController", null)
        {
            FormUrl = url;
            OnFormSubmitted = onSubmit;
        }

        public override bool PrefersStatusBarHidden() => true;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            webView.ScalesPageToFit = true;
            webView.Delegate = this;
            webView.LoadRequest(NSUrlRequest.FromUrl(new NSUrl(FormUrl)));
        }

        [Export("webViewDidFinishLoad:")]
        public void LoadingFinished(UIWebView webView)
        {
            if (webView.Request.Url.LastPathComponent == "formResponse" && webView.Request.HttpMethod == "POST")
            {
                Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe((obj) =>
                {
                    InvokeOnMainThread(() =>
                    {
                        DismissViewController(true, () =>
                        {
                            OnFormSubmitted?.Invoke();
                        });
                    });
                });
            }
        }
    }
}

