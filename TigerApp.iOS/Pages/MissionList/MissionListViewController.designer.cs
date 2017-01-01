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
    [Register ("MissionListViewController")]
    partial class MissionListViewController
    {
        [Outlet]
        UIKit.UIButton expHomeButton { get; set; }


        [Outlet]
        UIKit.UIView missionViewHolder { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (expHomeButton != null) {
                expHomeButton.Dispose ();
                expHomeButton = null;
            }

            if (missionViewHolder != null) {
                missionViewHolder.Dispose ();
                missionViewHolder = null;
            }
        }
    }
}