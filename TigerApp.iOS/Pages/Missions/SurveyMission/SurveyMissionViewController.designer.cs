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

namespace TigerApp.iOS.Pages.Missions.SurveyMission
{
    [Register ("SurveyMissionViewController")]
    partial class SurveyMissionViewController
    {
        [Outlet]
        UIKit.UIButton completeMissionButton { get; set; }


        [Action ("backButton:")]
        partial void backButton (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (completeMissionButton != null) {
                completeMissionButton.Dispose ();
                completeMissionButton = null;
            }
        }
    }
}