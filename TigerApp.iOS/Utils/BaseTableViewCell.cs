using System;
using CoreGraphics;
using UIKit;

namespace TigerApp.iOS.Utils
{
    public abstract class BaseTableViewCell<TInput> : UITableViewCell
    {
        public BaseTableViewCell(CGRect frame) : base(frame)
        {
        }

        protected BaseTableViewCell(IntPtr handle) : base(handle)
        {
        }

        public abstract void Bind(TInput datum);
    }
}