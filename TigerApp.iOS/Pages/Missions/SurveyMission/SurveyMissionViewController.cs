using System;
using System.Reactive.Linq;
using ReactiveUI;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages.Missions.SurveyMission
{
    public partial class SurveyMissionViewController : BaseReactiveViewController<ISurveyMissionViewModel>
    {
        private bool _surveyOpened;
        void CompleteMissionButton_TouchUpInside(object sender, EventArgs e)
        {
            ViewModel.GetSurveyUrl();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            completeMissionButton.TouchUpInside += CompleteMissionButton_TouchUpInside;
        }

        private void _openWebView() {
            if (_surveyOpened)
                return;
            else
                _surveyOpened = true;
            PresentViewController(new SurveyMissionGoogleFormViewController(ViewModel.SurveyUrl, () =>
            {
                ViewModel.SendCompleteSurveyActionToServer();
                DismissViewController(true, null);
            }), true, null);
        }

        partial void backButton(Foundation.NSObject sender)
        {
            DismissViewController(true, null);
        }

        public SurveyMissionViewController()
        {
            this.WhenActivated(dispose =>
            {
                dispose(ViewModel.WhenAnyValue(vm => vm.SurveyUrl).Where(surveyUrl => !string.IsNullOrEmpty(surveyUrl)).Subscribe(surveyUrl => { _openWebView(); }));
            });
        }
    }
}

