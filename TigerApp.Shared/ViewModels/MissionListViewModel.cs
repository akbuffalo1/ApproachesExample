using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using TigerApp.Shared.Models;
using TigerApp.Shared.Services.API;
using System.Linq;

namespace TigerApp.Shared.ViewModels
{
    public interface IMissionListViewModel : IViewModelBase
    {
        List<Objective> Missions { get; }
        void GetMissions();
        void SendInviteFBActionToServer();
    }

    public class MissionListViewModel : ReactiveViewModel, IMissionListViewModel
    {
        [Reactive]
        public List<Objective> Missions { get; protected set; }

        private readonly IMissionApiService _missionApi;

        public MissionListViewModel(IMissionApiService missionApi)
        {
            _missionApi = missionApi;
        }

        public void GetMissions()
        {
            _missionApi.GetMissions().SubscribeOnce((objectives) =>
                {
                    Missions = objectives.OrderBy(m => m.Order).ToList();
                    _missionApi.GetMissions(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce(updatedObjectives =>{
                        Missions = updatedObjectives.OrderBy(m => m.Order).ToList();                                            
                });});
        }

        public void SendInviteFBActionToServer() { 
            AD.Resolver.Resolve<ITrackedActionsApiService>().PushAction(new Shared.Models.TrackedActions.InviteFriendsTrackedAction(), null).SubscribeOnce(_ => { });
        }
    }
}