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

namespace TigerApp.iOS.Pages.Missions.TrotterMission
{
    [Register ("TrotterStoreCollectionViewCell")]
    partial class TrotterStoreCollectionViewCell
    {
        [Outlet]
        UIKit.UILabel cityLabel { get; set; }


        [Outlet]
        UIKit.UIView dividerView { get; set; }


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

            if (dividerView != null) {
                dividerView.Dispose ();
                dividerView = null;
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