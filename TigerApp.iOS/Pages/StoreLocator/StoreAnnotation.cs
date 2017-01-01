using CoreLocation;
using MapKit;
using TigerApp.Shared.Models;

namespace TigerApp.iOS.Pages.StoreLocator
{
    public class StoreAnnotation : MKPointAnnotation
    {
        public string StoreId { get; private set; }

        public StoreAnnotation(Store tigerStore)
        {
            Title = tigerStore.Name;
            Subtitle = tigerStore.Address;
            StoreId = tigerStore.Id;

            var location = new CLLocationCoordinate2D(tigerStore.Location.Latitude.Value, tigerStore.Location.Longitude.Value);
            SetCoordinate(location);
        }
    }
}

