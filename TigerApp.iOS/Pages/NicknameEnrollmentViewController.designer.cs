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
    [Register ("NicknameEnrollmentViewController")]
    partial class NicknameEnrollmentViewController
    {
        [Outlet]
        UIKit.UIButton registerButton { get; set; }


        [Outlet]
        TigerApp.iOS.Views.CustomTextField txtFieldNickname { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView avatarImage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (avatarImage != null) {
                avatarImage.Dispose ();
                avatarImage = null;
            }

            if (registerButton != null) {
                registerButton.Dispose ();
                registerButton = null;
            }

            if (txtFieldNickname != null) {
                txtFieldNickname.Dispose ();
                txtFieldNickname = null;
            }
        }
    }
}