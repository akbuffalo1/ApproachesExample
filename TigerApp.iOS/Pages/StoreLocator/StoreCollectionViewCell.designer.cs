// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace TigerApp.iOS.Pages.StoreLocator
{
    [Register ("StoreCollectionViewCell")]
    partial class StoreCollectionViewCell
    {
        [Outlet]
        UIKit.UILabel cityLabel { get; set; }

        [Outlet]
        UIKit.UILabel distanceLabel { get; set; }

        [Outlet]
        UIKit.UIImageView pinImage { get; set; }

        [Outlet]
        UIKit.UILabel streetLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (cityLabel != null) {
                cityLabel.Dispose ();
                cityLabel = null;
            }

            if (distanceLabel != null) {
                distanceLabel.Dispose ();
                distanceLabel = null;
            }

            if (pinImage != null) {
                pinImage.Dispose ();
                pinImage = null;
            }

            if (streetLabel != null) {
                streetLabel.Dispose ();
                streetLabel = null;
            }
        }
    }
}