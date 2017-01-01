using Foundation;
using System.ComponentModel;
using TigerApp.iOS.Utils;
using UIKit;

namespace TigerApp.iOS.Views
{
    [Register("ActivityIndicatorView"), DesignTimeVisible(true)]
    public class ActivityIndicatorView : UIView
    {
        private UIActivityIndicatorView _spinner;

        public ActivityIndicatorView()
        {
            TranslatesAutoresizingMaskIntoConstraints = false;

            var overlay = new UIView
            {
                BackgroundColor = Colors.SemiTransparentBlack,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            _spinner = new UIActivityIndicatorView
            {
                ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge,
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            AddSubview(overlay);
            AddSubview(_spinner);

            AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(overlay, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(overlay, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, this, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(overlay, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(overlay, NSLayoutAttribute.Height, NSLayoutRelation.Equal, this, NSLayoutAttribute.Height, 1, 0),

                NSLayoutConstraint.Create(_spinner, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(_spinner, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1, 0)
            });

            _spinner.StartAnimating();
        }

        public override bool Hidden
        {
            get { return base.Hidden; }
            set
            {
                base.Hidden = value;
                if (value)
                {
                    _spinner.StopAnimating();
                }
                else
                {
                    _spinner.StartAnimating();
                }
            }
        }
    }
}
