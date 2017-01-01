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
using TigerApp.Shared.ViewModels;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "@string/app_name")]
    public class UnsupportedRegionActivity : BaseReactiveActivity<IUnsupportedRegionViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_unsupported_region);
        }
    }
}