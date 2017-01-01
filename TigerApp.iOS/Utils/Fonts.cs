using UIKit;

namespace TigerApp.iOS.Utils
{
    public static class Fonts
    {
        public static TigerFont TigerBasic { get { return new TigerFont("tiger-basic-Regular"); } }
        public static TigerFont TigerCandy { get { return new TigerFont("Tiger-Candy_201509"); } }
        public static TigerFont FrutigerRegular { get { return new TigerFont("FrutigerNextLTW1G-RegularCn"); } }
        public static TigerFont FrutigerMedium { get { return new TigerFont("FrutigerNextLTW1G-MediumCn"); } }

        public class TigerFont
        {
            private string _fontName;
            private float _size;

            public TigerFont(string fontName)
            {
                _fontName = fontName;
            }

            public string FontName { get { return _fontName; } }

            private UIFont GetFont => UIFont.FromName(FontName, _size);

            public UIFont Pt8
            {
                get
                {
                    _size = 8f;
                    return GetFont;
                }
            }

            public UIFont Pt9
            {
                get
                {
                    _size = 9f;
                    return GetFont;
                }
            }

            public UIFont Pt10
            {
                get
                {
                    _size = 10f;
                    return GetFont;
                }
            }

            public UIFont Pt11
            {
                get
                {
                    _size = 11f;
                    return GetFont;
                }
            }

            public UIFont Pt12
            {
                get
                {
                    _size = 12f;
                    return GetFont;
                }
            }

            public UIFont Pt13
            {
                get
                {
                    _size = 13f;
                    return GetFont;
                }
            }

            public UIFont Pt14
            {
                get
                {
                    _size = 14f;
                    return GetFont;
                }
            }

            public UIFont Pt15
            {
                get
                {
                    _size = 15f;
                    return GetFont;
                }
            }

            public UIFont Pt16
            {
                get
                {
                    _size = 16f;
                    return GetFont;
                }
            }

            public UIFont Pt17
            {
                get
                {
                    _size = 17f;
                    return GetFont;
                }
            }

            public UIFont Pt18
            {
                get
                {
                    _size = 18f;
                    return GetFont;
                }
            }

            public UIFont Pt19
            {
                get
                {
                    _size = 19f;
                    return GetFont;
                }
            }

            public UIFont Pt20
            {
                get
                {
                    _size = 20f;
                    return GetFont;
                }
            }

            public UIFont Pt21
            {
                get
                {
                    _size = 21f;
                    return GetFont;
                }
            }

            public UIFont Pt22
            {
                get
                {
                    _size = 22f;
                    return GetFont;
                }
            }

            public UIFont Pt23
            {
                get
                {
                    _size = 23f;
                    return GetFont;
                }
            }

            public UIFont Pt24
            {
                get
                {
                    _size = 24f;
                    return GetFont;
                }
            }

            public UIFont Pt25
            {
                get
                {
                    _size = 25f;
                    return GetFont;
                }
            }

            public UIFont WithSize(float size)
            {
                _size = size;
                return GetFont;
            }
        }
    }
}
