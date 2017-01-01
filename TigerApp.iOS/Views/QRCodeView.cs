using System;
using TigerApp.Shared;
using UIKit;

namespace TigerApp.iOS.Views
{
    public class QRCodeView : UIImageView
    {
        protected ZXing.Mobile.BarcodeWriter Writer;
        protected string QRUrl = "";
        protected int QRWidth;
        protected int QRHeight;

        public QRCodeView()
        {
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

        protected string _url;
        public string Url 
        {
            get {
                return _url;
            }
            set {
                Image = Writer.Write(value);
                _url = value;
            }
        }
    }
}
