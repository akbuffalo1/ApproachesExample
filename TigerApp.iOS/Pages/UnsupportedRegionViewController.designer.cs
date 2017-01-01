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
    [Register ("UnsupportedRegionViewController")]
    partial class UnsupportedRegionViewController
    {
        [Outlet]
        UIKit.UILabel bottomLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CloseButton { get; set; }


        [Action ("onBackButtonClick:")]
        partial void onBackButtonClick (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (bottomLabel != null) {
                bottomLabel.Dispose ();
                bottomLabel = null;
            }

            if (CloseButton != null) {
                CloseButton.Dispose ();
                CloseButton = null;
            }
        }
    }
}