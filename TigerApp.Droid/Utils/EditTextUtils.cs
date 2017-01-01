using System;
using Android.Widget;

namespace TigerApp.Droid.Utils
{
    public static class EditTextUtils
    {
        public static bool IsEmpty(this EditText self)
        {
            return self.Text.Matches("");
        }
    }
}

