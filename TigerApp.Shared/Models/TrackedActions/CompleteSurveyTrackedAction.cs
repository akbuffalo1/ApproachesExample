using System;
using Newtonsoft.Json;

namespace TigerApp.Shared.Models.TrackedActions
{
    public class CompleteSurveyTrackedActionPayload : TrackedActionPayload
    {
        [JsonProperty("survey_id")]
        public string SurveyId;
    } 

    public class CompleteSurveyTrackedAction : TrackedAction
    {
        public CompleteSurveyTrackedAction(string surveyId):base("user-complete-survey", new CompleteSurveyTrackedActionPayload() { SurveyId = surveyId })
        {
        }
    }
}
