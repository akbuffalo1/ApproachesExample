using System;
using Android.App;
using Android.OS;
using Android.Widget;
using ReactiveUI;
using TigerApp.Shared.ViewModels;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "@string/app_name")]
    public class ChangeNumberActivity : BaseReactiveActivity<IChangeNumberViewModel>
    {
        LinearLayout llApplyNextContainer;
        ImageView btnBack;
        TextView tvNextButtonText;

        public ChangeNumberActivity()
        {
            this.WhenActivated((dis) => { 
                dis(this.BindCommand(ViewModel, vm => vm.PerformApplyPress, act => act.llApplyNextContainer));
            });
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_change_number);

            llApplyNextContainer = FindViewById<LinearLayout>(Resource.Id.llApplyNextContainer);
            tvNextButtonText = FindViewById<TextView>(Resource.Id.tvApplyNext);
            btnBack = FindViewById<ImageView>(Resource.Id.btnBack);

            tvNextButtonText.Text = GetString(Resource.String.change_number_fine);
            btnBack.Click += delegate { Finish(); };
        }
    }
}