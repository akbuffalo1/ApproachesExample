using System;
using Android.App;
using Android.OS;
using Android.Widget;
using ReactiveUI;
using TigerApp.Shared.ViewModels;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "@string/app_name")]
    public class ChangeNumberInstructionActivity : BaseReactiveActivity<IChangeNumberInstructionViewModel>
    {
        LinearLayout llApplyNextContainer;
        ImageView btnBack;
        TextView tvNextButtonText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_change_number_instruction);

            llApplyNextContainer = FindViewById<LinearLayout>(Resource.Id.llApplyNextContainer);
            tvNextButtonText = FindViewById<TextView>(Resource.Id.tvApplyNext);
            btnBack = FindViewById<ImageView>(Resource.Id.btnBack);

            tvNextButtonText.Text = GetString(Resource.String.change_number_avanti);
            llApplyNextContainer.Click += delegate { StartActivity(typeof(ChangeNumberActivity)); };
            btnBack.Click += delegate { Finish(); };
        }
    }
}