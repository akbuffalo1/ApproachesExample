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

namespace TigerApp.iOS.Pages.Missions.CitiesCheckin
{
    [Register ("CitiesCheckInViewController")]
    partial class TwoCitiesCheckInViewController
    {
        [Outlet]
        UIKit.UIButton backButton { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint citiesStackHeightConstraint { get; set; }


        [Outlet]
        UIKit.UIStackView citiesStackView { get; set; }


        [Outlet]
        UIKit.UILabel missionDescriptionLabel { get; set; }


        [Outlet]
        UIKit.UILabel titleLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (backButton != null) {
                backButton.Dispose ();
                backButton = null;
            }

            if (citiesStackHeightConstraint != null) {
                citiesStackHeightConstraint.Dispose ();
                citiesStackHeightConstraint = null;
            }

            if (citiesStackView != null) {
                citiesStackView.Dispose ();
                citiesStackView = null;
            }

            if (missionDescriptionLabel != null) {
                missionDescriptionLabel.Dispose ();
                missionDescriptionLabel = null;
            }

            if (titleLabel != null) {
                titleLabel.Dispose ();
                titleLabel = null;
            }
        }
    }
}