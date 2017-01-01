using AD.iOS;
using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace TigerApp.iOS.Utils
{
    public static class UICommon
    {
        public static float ScaleFactor = DeviceHelper.IsIphone5() ? 1 : DeviceHelper.IsIphone6() ? 1.2f : 1.3f;

        public static UIStackView CreateStackView(UILayoutConstraintAxis axis = UILayoutConstraintAxis.Vertical,
            UIStackViewDistribution distribution = UIStackViewDistribution.Fill,
            UIStackViewAlignment alignment = UIStackViewAlignment.Fill, float spacing = 0)
        {
            return new UIStackView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Axis = axis,
                Distribution = distribution,
                Alignment = alignment,
                Spacing = spacing
            };
        }

        public static UILabel CreateLabel(UIFont font, UIColor textColor, UITextAlignment alignment = UITextAlignment.Left,
            UILineBreakMode lineBreakMode = UILineBreakMode.TailTruncation, int lines = 0)
        {
            return new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = font,
                TextColor = textColor,
                TextAlignment = alignment,
                LineBreakMode = lineBreakMode,
                Lines = lines
            };
        }

        public static UIButton CreateIconButton(UIImage image, EventHandler handler)
        {
            var iconButton = UIButton.FromType(UIButtonType.RoundedRect);
            iconButton.TranslatesAutoresizingMaskIntoConstraints = false;
            iconButton.TintColor = UIColor.Black;
            iconButton.SetImage(image, UIControlState.Normal);
            iconButton.ContentMode = UIViewContentMode.ScaleAspectFit;
            iconButton.TouchUpInside += handler;

            return iconButton;
        }

        public static UIButton CreateButton(string title, string imagePath = null)
        {
            var button = UIButton.FromType(UIButtonType.RoundedRect);
            button.TranslatesAutoresizingMaskIntoConstraints = false;
            button.SetTitle(title, UIControlState.Normal);
            button.TitleLabel.Font = Fonts.TigerBasic.WithSize(30);
            button.SetTitleColor(UIColor.White, UIControlState.Normal);
            button.SetBackgroundImage(UIImage.FromBundle(imagePath ?? "ButtonBackground"), UIControlState.Normal);
            button.ContentEdgeInsets = new UIEdgeInsets(10, 30, 10, 30);
            button.HeightAnchor.ConstraintEqualTo(56).Active = true;
            return button;
        }

        public static UITableView CreateTableView()
        {
            return new UITableView
            {
                BackgroundColor = UIColor.Clear,
                CellLayoutMarginsFollowReadableWidth = false,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Bounces = false,
                RowHeight = UITableView.AutomaticDimension,
                EstimatedRowHeight = 60,
                SeparatorStyle = UITableViewCellSeparatorStyle.None
            };
        }

        public static UIView CreateDivider(float thickness = 1f, bool horizontal = true, UIColor color = null)
        {
            var divider = new UIView();
            divider.TranslatesAutoresizingMaskIntoConstraints = false;
            divider.BackgroundColor = color ?? Colors.DividerDefault;
            if (horizontal)
            {
                divider.HeightAnchor.ConstraintEqualTo(thickness).Active = true;
            }
            else
            {
                divider.WidthAnchor.ConstraintEqualTo(thickness).Active = true;
            }
            return divider;
        }

        public static UIImageView CreateBackIcon()
        {
            var back = new UIImageView();
            back.Image = UIImage.FromBundle("BackButton");
            back.WidthAnchor.ConstraintEqualTo(41).Active = true;
            back.HeightAnchor.ConstraintEqualTo(27).Active = true;
            back.UserInteractionEnabled = true;
            return back;
        }

        public static void ApplyTigerFontDefaultAttributes(this UILabel label, UIFont font, float lineHeight = 25F, float lineSpacing = 0.15F, UITextAlignment textAlignment = UITextAlignment.Center)
        {
            var attrText = new NSMutableAttributedString(label.Text);
            var range = new NSRange(0, label.Text.Length);

            var paragraphStyle = new NSMutableParagraphStyle();
            paragraphStyle.Alignment = textAlignment;
            paragraphStyle.LineSpacing = lineSpacing;
            paragraphStyle.MinimumLineHeight = lineHeight;
            paragraphStyle.MaximumLineHeight = lineHeight;

            attrText.AddAttribute(UIStringAttributeKey.ParagraphStyle, paragraphStyle, range);
            attrText.AddAttribute(UIStringAttributeKey.Font, font, range);
            attrText.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.Black, range);

            label.Lines = 0;
            label.AttributedText = attrText;
        }

        public static UITextField CreateTextField(string placeholder = null, UITextAlignment alignment = UITextAlignment.Center,
            UIKeyboardType keyboardType = UIKeyboardType.Default, bool password = false)
        {
            var textField = new TextField();
            textField.TranslatesAutoresizingMaskIntoConstraints = false;
            textField.Font = Fonts.FrutigerMedium.Pt19;
            textField.TextColor = UIColor.Black;
            textField.TextAlignment = alignment;
            textField.KeyboardType = keyboardType;
            textField.SecureTextEntry = password;

            if (!string.IsNullOrEmpty(placeholder))
            {
                textField.AttributedPlaceholder = new NSAttributedString(placeholder, foregroundColor: Colors.Hex999999);
            }

            textField.HeightAnchor.ConstraintEqualTo(32).Active = true;

            return textField;
        }

        public class TextField : UITextField
        {
            public override void Draw(CGRect rect)
            {
                var startingPoint = new CGPoint(x: rect.GetMinX(), y: rect.GetMaxY());
                var endingPoint = new CGPoint(x: rect.GetMaxX(), y: rect.GetMaxY());

                var path = new UIBezierPath();

                path.MoveTo(startingPoint);
                path.AddLineTo(endingPoint);
                path.LineWidth = 2.0f;

                Colors.Hex999999.SetStroke();

                path.Stroke();
            }
        }
    }
}
