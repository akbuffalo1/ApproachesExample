using System;
using System.Collections.Generic;
using TigerApp.Shared.Models.TrackedActions;

namespace TigerApp.Shared.Services.Platform
{
    public interface IGeofenceService
    {
        void RegisterPlace(Place place);
        bool StartMonitoring(Action<Place> onEnter=null, Action<Place> onExit=null);
        bool StopMonitoring();
        void ClearAll();
        void SaveAll();
    }
}
