using Android.App;
using Android.OS;
using TigerApp.Droid.UI;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "@string/app_name")]
    public class SettingsFeedbackActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var formsView = new GoogleFormsWebView(this, Shared.Constants.Forms.FeedbackUrl);
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