
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TigerApp.Droid.UI;
using TigerApp.Shared.Services.API;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "CheckInSurveyActivity")]
    public class CheckInSurveyActivity : Activity
    {
        public const string StoreIdIntentKey = "StoreId";
        private string _surveyUrl;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var storeId = Intent.GetStringExtra(StoreIdIntentKey);
            AD.Resolver.Resolve<IStoreApiService>().GetStoreList().SubscribeOnce(stores =>
            {
                var lastCheckinStores = stores != null ? stores.Find(store => store.Slug.Equals(storeId)) : null;
                if (lastCheckinStores != null)
                {
                    _surveyUrl = string.Format(Shared.Constants.Forms.CheckinSurvey, lastCheckinStores.Address);
                    this.RunOnUiThread(OpenSurveyForm);
                }
                else {
                    AD.Resolver.Resolve<IStoreApiService>().GetStoreList(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce(updatedStores =>
                    {
                        lastCheckinStores = updatedStores.Find(store => store.Slug.Equals(storeId));
                        if (lastCheckinStores != null)
                        {
                            _surveyUrl = string.Format(Shared.Constants.Forms.CheckinSurvey, lastCheckinStores.Address);
                            this.RunOnUiThread(OpenSurveyForm);
                        }
                    });
                }
            });
        }

        void OpenSurveyForm()
        {
            var formsView = new GoogleFormsWebView(this, _surveyUrl);
            formsView.OnSubmit += (object sender, bool submitted) =>
            {
                if (submitted)
                {
                    Finish();
                }
            };

            SetContentView(formsView);

            formsView.OpenForm();
        }
   }
}
