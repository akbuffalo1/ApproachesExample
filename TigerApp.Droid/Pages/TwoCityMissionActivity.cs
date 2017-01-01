
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ReactiveUI;
using TigerApp.Shared.ViewModels;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "TwoCityMissionActivity")]
    public class TwoCityMissionActivity : BaseReactiveActivity<ITwoCitiesCheckInViewModel>
    {
        LinearLayout _llBottomListContainer;
        ImageView _btnBack;

        public TwoCityMissionActivity() { 
            this.WhenActivated(dis =>
            {
                dis(ViewModel.WhenAnyValue(vm => vm.Cities).Where(cities => cities != null).Subscribe(UpdateImages));
                ViewModel.GetCheckInCities();

            });
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_two_cities_mission);

            _llBottomListContainer = FindViewById<LinearLayout>(Resource.Id.llBottomListContainer);
            _btnBack = FindViewById<ImageView>(Resource.Id.btnBack);
            _btnBack.Click += (sender, e) => Finish();
        }

        void UpdateImages(IList<string> dataList)
        {
            var dataCount = dataList.Count();
            for (var counter = 0; counter < _llBottomListContainer.ChildCount; counter++)
            {
                LinearLayout itemContainer = (LinearLayout)_llBottomListContainer.GetChildAt(counter);
                var image = ((ImageView)itemContainer.GetChildAt(0));
                var label = ((TextView)itemContainer.GetChildAt(1));
                if (counter < dataCount)
                {
                    image.SetImageResource(Resource.Drawable.missione_tiger_checkin_2citta_icon_on);
                    label.Text = dataList[counter];
                    label.Visibility = ViewStates.Visible;
                }
                else
                {
                    image.SetImageResource(Resource.Drawable.missione_tiger_checkin_2citta_icon_off);
                    label.Visibility = ViewStates.Gone;
                }
            }
        }
    }
}

