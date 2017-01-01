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

namespace TigerApp.iOS.Pages.MissionList
{
    [Register ("MissionTableViewCell")]
    partial class MissionTableViewCell
    {
        [Outlet]
        UIKit.UIImageView imageView { get; set; }


        [Outlet]
        UIKit.UILabel missionDescriptionLabel { get; set; }


        [Outlet]
        UIKit.UILabel pointsLabel { get; set; }

        [Outlet]
        UIKit.UIImageView checkmarkImageView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (checkmarkImageView != null) {
                checkmarkImageView.Dispose ();
                checkmarkImageView = null;
            }

            if (imageView != null) {
                imageView.Dispose ();
                imageView = null;
            }

            if (missionDescriptionLabel != null) {
                missionDescriptionLabel.Dispose ();
                missionDescriptionLabel = null;
            }

            if (pointsLabel != null) {
                pointsLabel.Dispose ();
                pointsLabel = null;
            }
        }
    }
}