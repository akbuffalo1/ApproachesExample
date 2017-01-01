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

namespace TigerApp.iOS.Tutorial
{
    [Register ("TutorialBubble")]
    partial class TutorialBubble
    {
        [Outlet]
        UIKit.UILabel textLabel { get; set; }

        [Outlet]
        UIKit.UIView View { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (textLabel != null) {
                textLabel.Dispose ();
                textLabel = null;
            }

            if (View != null) {
                View.Dispose ();
                View = null;
            }
        }
    }
}