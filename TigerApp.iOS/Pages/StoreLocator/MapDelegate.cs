using AD;
using MapKit;
using System;

namespace TigerApp.iOS.Pages.StoreLocator
{
    public class MapDelegate : MKMapViewDelegate
    {
        public event EventHandler<ValueEventArgs<string>> OnStoreAnnotationSelected;
        public event EventHandler<ValueEventArgs<string>> OnStoreAnnotationDeselected;
        public event EventHandler OnAnnotationViewsAdded;
        public event EventHandler OnMapRendered;
        public event EventHandler<ValueEventArgs<MKUserLocation>> OnUserLocationUpdated;
        public event EventHandler OnAcquiredUserLocation;

        public override void DidStopLocatingUser(MKMapView mapView)
        {
            OnAcquiredUserLocation?.Invoke(mapView, EventArgs.Empty);
        }

        public override void DidUpdateUserLocation(MKMapView mapView, MKUserLocation userLocation)
        {
            OnUserLocationUpdated?.Invoke(mapView, new ValueEventArgs<MKUserLocation>(userLocation));
        }

        public override void DidSelectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            if (view is StoreAnnotationView)
            {
                var annotation = view.Annotation as StoreAnnotation;
                OnStoreAnnotationSelected?.Invoke(mapView, new ValueEventArgs<string>(annotation.StoreId));
            }
        }

        public override void DidDeselectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            if (view is StoreAnnotationView)
            {
                var annotation = view.Annotation as StoreAnnotation;
                OnStoreAnnotationDeselected?.Invoke(mapView, new ValueEventArgs<string>(annotation.StoreId));
            }
        }

        public override void DidAddAnnotationViews(MKMapView mapView, MKAnnotationView[] views)
        {
            OnAnnotationViewsAdded?.Invoke(mapView, EventArgs.Empty);
        }

        public override void DidFinishRenderingMap(MKMapView mapView, bool fullyRendered)
        {
            OnMapRendered?.Invoke(mapView, EventArgs.Empty);
        }

        public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            if (annotation is StoreAnnotation)
            {
                var annView = mapView.DequeueReusableAnnotation(StoreAnnotationView.StoreAnnotationReuseIdentifer) ?? new StoreAnnotationView(annotation as StoreAnnotation);
                annView.Annotation = annotation;
                return annView;
            }
            else
            {
                return null;
            }
        }
    }
}