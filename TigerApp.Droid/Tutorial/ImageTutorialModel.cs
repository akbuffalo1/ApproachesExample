using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TigerApp.Droid.Tutorial
{
    public class ImageTutorialModel
    {
        public int ImageResourceId;
        public View AnchorView;
        public GravityFlags Gravity;
        public int yShiftDp = 0;
        public bool IsFlipped = false; 
    }
}