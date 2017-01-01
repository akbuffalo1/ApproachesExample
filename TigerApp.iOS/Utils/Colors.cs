using System;
using UIKit;

namespace TigerApp.iOS.Utils
{
    public static class Colors
    {
        public static UIColor HexF6E277 { get { return ColorFromHexString("#F6E277"); } }
        public static UIColor HexF3F3F2 { get { return ColorFromHexString("#F3F3F2"); } }
        public static UIColor DividerDefault { get { return UIColor.FromRGB(224, 224, 224); } }
        public static UIColor Hex4D4D4D { get { return ColorFromHexString("#4D4D4D"); } }
        public static UIColor Hex999999 { get { return ColorFromHexString("#999999"); } }
        public static UIColor HexF8F7F5 { get { return ColorFromHexString("#F8F7F5"); } }
        public static UIColor HexEDED74 { get { return ColorFromHexString("#EDED74"); } }
        public static UIColor SemiTransparentBlack { get { return UIColor.FromRGBA(0, 0, 0, 0.5f); } }

        public static UIColor ColorFromHexString(string hexValue, float alpha = 1.0f)
        {
            var colorString = hexValue.Replace("#", "");
            if (alpha > 1.0f)
            {
                alpha = 1.0f;
            }
            else if (alpha < 0.0f)
            {
                alpha = 0.0f;
            }

            float red, green, blue;

            switch (colorString.Length)
            {
                case 3: // #RGB
                    {
                        red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f;
                        green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f;
                        blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f;
                        return UIColor.FromRGBA(red, green, blue, alpha);
                    }
                case 6: // #RRGGBB
                    {
                        red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
                        green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
                        blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                        return UIColor.FromRGBA(red, green, blue, alpha);
                    }

                default:
                    throw new ArgumentOutOfRangeException(string.Format("Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB", hexValue));

            }
        }
    }
}
