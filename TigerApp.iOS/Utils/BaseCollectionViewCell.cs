using System;
using CoreGraphics;
using UIKit;

namespace TigerApp.iOS.Utils
{
    public abstract class BaseCollectionViewCell<TInput> : UICollectionViewCell
    {
        public BaseCollectionViewCell(CGRect frame) : base(frame)
        {
        }

        protected BaseCollectionViewCell(IntPtr handle) : base(handle)
        {
        }

        public BaseCollectionViewCell() { }
        public abstract void Bind(TInput datum);
    }
}