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

namespace TigerApp.iOS.Views
{
    [Register ("CustomTextField")]
    partial class CustomTextField
    {
        [Outlet]
        UIKit.UIImageView Icon { get; set; }

        [Outlet]
        UIKit.UITextField textField { get; set; }

        [Outlet]
        UIKit.UIView View { get; set; }

        [Outlet]
        UIKit.UIView Divider { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Divider != null) {
                Divider.Dispose ();
                Divider = null;
            }

            if (Icon != null) {
                Icon.Dispose ();
                Icon = null;
            }

            if (textField != null) {
                textField.Dispose ();
                textField = null;
            }

            if (View != null) {
                View.Dispose ();
                View = null;
            }
        }
    }
}