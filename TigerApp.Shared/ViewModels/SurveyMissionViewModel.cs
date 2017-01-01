using System;
using ReactiveUI.Fody.Helpers;
using TigerApp.Shared.Models;
using TigerApp.Shared.Services.API;

namespace TigerApp.Shared.ViewModels
{
    public interface ISurveyMissionViewModel : IViewModelBase
    {
        string SurveyUrl { get; }
        void SendCompleteSurveyActionToServer();
        void GetSurveyUrl();
    }

    public class SurveyMissionViewModel : ReactiveViewModel,ISurveyMissionViewModel
    {
        private string _surveySlug;
        [Reactive]
        public string SurveyUrl {
            get; /*{
                var lastCheckin = AD.Resolver.Resolve<IStoreCheckInApiService>().LastCheckin;
                string address = string.Empty;
                if(lastCheckin != null)
                    address = string.Format("{0} , {1}",lastCheckin.Address,lastCheckin.Address);
                return string.Format(Constants.Forms.CheckinSurvey, address);
            }*/
            protected set;
        }

        public void SendCompleteSurveyActionToServer() {
            AD.Resolver.Resolve<ITrackedActionsApiService>().PushAction(new Models.TrackedActions.CompleteSurveyTrackedAction(_surveySlug),null).SubscribeOnce(_ => { });
        }

        public void GetSurveyUrl() {
            AD.Resolver.Resolve<ISurveyApiService>().GetSurveys().SubscribeOnce(surveys => {
                _surveySlug = surveys[0].Slug;
                SurveyUrl = surveys[0].Url;
            });
        }
    }
}
