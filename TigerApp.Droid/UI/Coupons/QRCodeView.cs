using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Widget;
using TigerApp.Shared;

namespace TigerApp.Droid.UI.Coupons
{
    public class QRCodeView : ImageView
    {
        protected ZXing.Mobile.BarcodeWriter Writer;
        protected string QRUrl = "";
        protected int QRWidth;
        protected int QRHeight;

        public QRCodeView(Context ctx):base(ctx)
        {
            _init();
        }

        public QRCodeView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _init();
        }

        public QRCodeView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs,defStyleAttr,defStyleRes)
        {
            _init();
        }

        public QRCodeView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            _init();
        }

        protected void _init() {
            QRWidth = Constants.QRCodeSetting.QRDefaultWidth;
            QRHeight = Constants.QRCodeSetting.QRDefaultHeight;
            Writer = new ZXing.Mobile.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = QRWidth,
                    Height = QRHeight
                }
            };
        }

        public override void SetImageURI(Android.Net.Uri uri)
        {
            QRUrl = uri.ToString();
            SetImageBitmap(Writer.Write(QRUrl));
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            QRWidth = MeasureSpec.GetSize(widthMeasureSpec);
            QRHeight = MeasureSpec.GetSize(heightMeasureSpec);
            RefreshQR();
        }

        protected void RefreshQR() {
            Writer.Options = new ZXing.Common.EncodingOptions
            {
                Width = QRWidth,
                Height = QRHeight
            };
            SetImageBitmap(Writer.Write(QRUrl));
        }
    }
}
