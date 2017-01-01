#region using

using System;
using System.Reactive.Linq;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using ReactiveUI;
using TigerApp.Droid.UI;
using TigerApp.Shared.ViewModels;

#endregion

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "ScanMissionActivity")]
    public class ScanMissionActivity : BaseReactiveActivity<IScanMissionViewModel>
    {
        public const string EurosKey = "euros";
        private TintableButton _btnScanComplete;
        private ImageView _backImg;
        private TextView _txtPuntiCount;
        private ImageButton _btnBack;
        
        public ScanMissionActivity()
        {
            this.WhenActivated(dispose =>
            {
                InitListeners(dispose);
            });
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_scan_mission);

            InitViews();

            var euros = Intent.GetIntExtra(EurosKey, 10);
            SetPuntiCount(euros);
        }

        private void InitViews()
        {
            _btnBack = FindViewById<ImageButton>(Resource.Id.btnActionBarBack);
            _btnScanComplete = FindViewById<TintableButton>(Resource.Id.btnScanMissionCompleteCommand);
            _backImg = FindViewById<ImageView>(Resource.Id.imgScanMission);
            _txtPuntiCount = FindViewById<TextView>(Resource.Id.txtPuntiCount);
        }

        private void InitListeners(Action<IDisposable> dispose)
        {
            dispose(Observable.FromEventPattern(
                subscribe => _btnBack.Click += subscribe,
                unSubscribe => _btnBack.Click -= unSubscribe)
                    .Subscribe(evArgs => Finish()));
            dispose(Observable.FromEventPattern(
                subscribe => _btnScanComplete.Click += subscribe,
                unSubscribe => _btnScanComplete.Click -= unSubscribe)
                    .Subscribe(evArgs => StartNewActivity(typeof(ScanReceiptActivity), TransitionWay.DU)));
        }

        private void SetPuntiCount(int count)
        {
            Color color = Resources.GetColor(Resource.Color.scan_mission_1);
            int points = 10;
            switch (count)
            {
                case 10:
                    {
                        color = Resources.GetColor(Resource.Color.scan_mission_1);
                        points = 10;
                    }
                    break;
                case 20:
                    {
                        color = Resources.GetColor(Resource.Color.scan_mission_2);
                        points = 22;
                    }
                    break;
                case 30:
                    {
                        color = Resources.GetColor(Resource.Color.scan_mission_3);
                        points = 35;
                    }
                    break;
            }

            var messageTextView = FindViewById<TextView>(Resource.Id.txtScanMissionMessage);
            messageTextView.Text = messageTextView.Text.Replace("#EUROS!#",count.ToString());
            _txtPuntiCount.Text = points.ToString();
            _backImg.SetColorFilter(color, PorterDuff.Mode.Multiply);
        }
    }
}