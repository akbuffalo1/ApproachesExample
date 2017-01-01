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
    [Register ("SmsEnrollmentViewController")]
    partial class SmsEnrollmentViewController
    {
        [Outlet]
        TigerApp.iOS.Views.CustomTextField verificationCodeCustomTextField { get; set; }


        [Outlet]
        UIKit.UIButton verifyCodeButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (verificationCodeCustomTextField != null) {
                verificationCodeCustomTextField.Dispose ();
                verificationCodeCustomTextField = null;
            }

            if (verifyCodeButton != null) {
                verifyCodeButton.Dispose ();
                verifyCodeButton = null;
            }
        }
    }
}