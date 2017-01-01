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

namespace TigerApp.iOS.Pages
{
    [Register ("EnableGeolocationViewController")]
    partial class EnableGeolocationViewController
    {
        [Outlet]
        UIKit.UIButton authorizeRegionButton { get; set; }


        [Outlet]
        UIKit.UIImageView mapImageView { get; set; }


        [Action ("gotoListOfStoresPage:")]
        partial void gotoListOfStoresPage (Foundation.NSObject sender);


        [Action ("OnAuthorizeRegionClicked:")]
        partial void OnAuthorizeRegionClicked (Foundation.NSObject sender);


        [Action ("OnInAnotherRegionClicked:")]
        partial void OnInAnotherRegionClicked (Foundation.NSObject sender);


        [Action ("OnListOfStoresClicked:")]
        partial void OnListOfStoresClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (authorizeRegionButton != null) {
                authorizeRegionButton.Dispose ();
                authorizeRegionButton = null;
            }

            if (mapImageView != null) {
                mapImageView.Dispose ();
                mapImageView = null;
            }
        }
    }
}