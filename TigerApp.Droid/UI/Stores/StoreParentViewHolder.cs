#region using

using System;
using Android.Views;
using Android.Widget;
using TigerApp.Droid.UI.ExpandableRecyclerView.ViewHolders;

#endregion

namespace TigerApp.Droid.UI.Stores
{
    public class StoreParentViewHolder : ParentViewHolder
    {
        private const float InitialPosition = 0.0f;
        private const float RotatedPosition = 90f;
        private const float InitialAlpha = 0.0f;
        private const float TargetAlpha = 1.0f;
        public ImageView arrowIcon;
        public TextView txtCaption;

        public StoreParentViewHolder(View itemView) : base(itemView)
        {
            txtCaption = itemView.FindViewById<TextView>(Resource.Id.txtCaption);
            arrowIcon = itemView.FindViewById<ImageView>(Resource.Id.icExpand);
            arrowIcon.Rotation = InitialPosition;
            arrowIcon.Alpha = InitialAlpha;
        }

        public override bool Expanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;

                if (_isExpanded)
                {
                    arrowIcon.Rotation = RotatedPosition;
                    arrowIcon.Alpha = TargetAlpha;
                }
                else
                {
                    arrowIcon.Rotation = InitialPosition;
                    arrowIcon.Alpha = InitialAlpha;
                }
            }
        }
    }
}