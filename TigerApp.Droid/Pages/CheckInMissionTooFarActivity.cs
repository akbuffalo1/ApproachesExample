using Android.App;
using Android.OS;
using Android.Text;
using Android.Text.Style;
using Android.Widget;
using TigerApp.Shared;
using TigerApp.Shared.ViewModels;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "CheckInMissionTooFarActivity")]
    public class CheckInMissionTooFarActivity : BaseReactiveActivity<ICheckInMissionTooFarViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_check_in_mission_complete);

            string message = Constants.Strings.CheckInMissionPageCompleteMessage.Replace("\n\n\n", "\n\n");
            SpannableString formattedMessage = new SpannableString(message);
            formattedMessage.SetSpan(new RelativeSizeSpan(1.3f), 0, 4, SpanTypes.ExclusiveExclusive);
            FindViewById<TextView>(Resource.Id.txtCheckInMissionCompleteMessage).TextFormatted = formattedMessage;
        }
    }
}