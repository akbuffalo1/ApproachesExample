using Foundation;
using System;
using UIKit;

namespace TigerApp.iOS.Views
{
    public partial class CustomTextField : UIView
    {
        private const string Name = nameof(CustomTextField);
        public static readonly UINib Nib = UINib.FromName(Name, NSBundle.MainBundle);
        public static readonly NSString Key = new NSString(Name);

        public override bool CanBecomeFirstResponder => true;
        public override bool CanResignFirstResponder => true;

        public UITextField TextField => textField;

        public override bool BecomeFirstResponder()
        {
            textField.BecomeFirstResponder();
            return base.BecomeFirstResponder();
        }

        public override bool ResignFirstResponder()
        {
            textField.ResignFirstResponder();
            return base.ResignFirstResponder();
        }

        public void SetKeyboard(UIKeyboardType keyboardType)
        {
            textField.KeyboardType = keyboardType;
        }

        public CustomTextField(IntPtr handle) : base(handle) { Initialize(); }

        public CustomTextField() { Initialize(); }

        public CustomTextField(CoreGraphics.CGRect frame) : base(frame) { Initialize(); }

        public CustomTextField(NSCoder coder) : base(coder) { Initialize(); }


        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public void HideIcon()
        {
            Icon.Hidden = true;
        }

        public void ShowIcon()
        {
            Icon.Hidden = false;
        }

        public void SetIcon(string imagePath)
        {
            Icon.Image = UIImage.FromBundle(imagePath);
        }

        public void HideDivider()
        {
            Divider.Hidden = true;
        }

        private void Initialize()
        {
            View = Nib.Instantiate(this, null)[0] as UIView;
            View.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            View.Frame = Bounds;

            AddSubview(View);
            SetNeedsDisplay();
        }

        [Export("placeholderKey")]
        public string PlaceholderKey
        {
            set
            {
                textField.Placeholder = value;
            }
        }

        [Export("hideIconKey")]
        public bool HideIconKey
        {
            set
            {
                Icon.Hidden = value;
            }
        }
    }
}

