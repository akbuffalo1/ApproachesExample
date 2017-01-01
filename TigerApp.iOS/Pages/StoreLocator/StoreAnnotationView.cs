using System;
using CoreGraphics;
using CoreLocation;
using MapKit;
using TigerApp.Shared.Models;
using UIKit;

namespace TigerApp.iOS.Pages.StoreLocator
{
    public class StoreAnnotationView : MKAnnotationView
    {
        public static string StoreAnnotationReuseIdentifer = nameof(StoreAnnotationView);
        public override string ReuseIdentifier => StoreAnnotationReuseIdentifer;

        public static UIImage ImageForSelectedState => UIImage.FromBundle("store_loc_04").Scale(new CGSize(23, 35));
        public static UIImage ImageForNormalState => UIImage.FromBundle("store_loc_03").Scale(new CGSize(23, 35));

        public StoreAnnotationView(StoreAnnotation storeAnnotation) : base(storeAnnotation, StoreAnnotationReuseIdentifer)
        {
            //CanShowCallout = true;
            Image = ImageForNormalState;
            Enabled = true;
        }

        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);

            if (selected)
            {
                Image = ImageForSelectedState;
            }
            else
            {
                Image = ImageForNormalState;
            }
        }
    }
}