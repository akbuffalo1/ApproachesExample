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

namespace TigerApp.iOS.Pages.EditProfile
{
    [Register ("EditProfileViewController")]
    partial class EditProfileViewController
    {
        [Outlet]
        UIKit.UITextField cognomeField { get; set; }


        [Outlet]
        UIKit.UITextField compleanoField { get; set; }


        [Outlet]
        UIKit.UIButton confirmButton { get; set; }


        [Outlet]
        UIKit.UITextField emailField { get; set; }


        [Outlet]
        UIKit.UITextField nicknameField { get; set; }


        [Outlet]
        UIKit.UITextField nomeField { get; set; }


        [Outlet]
        UIKit.UITextField telephoneField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TigerApp.iOS.Pages.EditProfile.TigerCityPicker cityField { get; set; }


        [Action ("OnBackButtonClick:")]
        partial void OnBackButtonClick (Foundation.NSObject sender);


        [Action ("OnConfirmButtonClick:")]
        partial void OnConfirmButtonClick (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (cityField != null) {
                cityField.Dispose ();
                cityField = null;
            }

            if (cognomeField != null) {
                cognomeField.Dispose ();
                cognomeField = null;
            }

            if (compleanoField != null) {
                compleanoField.Dispose ();
                compleanoField = null;
            }

            if (confirmButton != null) {
                confirmButton.Dispose ();
                confirmButton = null;
            }

            if (emailField != null) {
                emailField.Dispose ();
                emailField = null;
            }

            if (nicknameField != null) {
                nicknameField.Dispose ();
                nicknameField = null;
            }

            if (nomeField != null) {
                nomeField.Dispose ();
                nomeField = null;
            }

            if (telephoneField != null) {
                telephoneField.Dispose ();
                telephoneField = null;
            }
        }
    }
}