#region using

using System;
using System.Reactive.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Widget;
using ReactiveUI;
using TigerApp.Droid.UI;
using TigerApp.Shared;
using TigerApp.Shared.ViewModels;

#endregion

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "SurveyMissionActivity")]
    public class SurveyMissionActivity : BaseReactiveActivity<ISurveyMissionViewModel>
    {
        private TintableButton _btnMissionComplete;
        private TextView _txtPuntiCount;
        private ImageButton _btnBack;

        public SurveyMissionActivity()
        {
            this.WhenActivated(dispose =>
            {
                InitListeners(dispose);
                dispose(ViewModel.WhenAnyValue(vm => vm.SurveyUrl).Where(surveyUrl => !string.IsNullOrEmpty(surveyUrl)).Subscribe(surveyUrl => { _openWebView();}));
            });
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_survey_mission);

            InitViews();
        }

        private void InitViews()
        {
            _btnBack = FindViewById<ImageButton>(Resource.Id.btnActionBarBack);
            _btnMissionComplete = FindViewById<TintableButton>(Resource.Id.btnMissionCompleteCommand);
            _txtPuntiCount = FindViewById<TextView>(Resource.Id.txtPuntiCount);
        }

        private void InitListeners(Action<IDisposable> dispose)
        {
            dispose(Observable.FromEventPattern(
                    subscribe => _btnBack.Click += subscribe,
                    unSubscribe => _btnBack.Click -= unSubscribe)
                .Subscribe(evArgs => Finish()));
            dispose(Observable.FromEventPattern(
                    subscribe => _btnMissionComplete.Click += subscribe,
                    unSubscribe => _btnMissionComplete.Click -= unSubscribe)
                .Subscribe(evArgs =>
                {
                    ViewModel.GetSurveyUrl();
                }));
        }

        private void _openWebView() 
        {
            var formsView = new GoogleFormsWebView(this,ViewModel.SurveyUrl);
            formsView.OnSubmit += (object sender, bool submitted) => {
                if (submitted) { 
                    ViewModel.SendCompleteSurveyActionToServer();
                    Finish();
                }
            };
            SetContentView(formsView);
            //FindViewById<RelativeLayout>(Resource.Id.surveyMissionContainer).AddView(formsView);
            formsView.OpenForm();
        }
    }
}