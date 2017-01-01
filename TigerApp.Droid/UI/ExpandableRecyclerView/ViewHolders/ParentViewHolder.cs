using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using TigerApp.Droid.UI.ExpandableRecyclerView.ClickListeners;

namespace TigerApp.Droid.UI.ExpandableRecyclerView.ViewHolders
{
    public class ParentViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
    {
        protected bool _isExpanded;

        public ParentViewHolder(View itemView) : base(itemView)
        {
            _isExpanded = false;
        }
       
        public IParentItemClickListener ParentItemClickListener { get; set; }
      

        public virtual bool Expanded
        {
            get { return _isExpanded; }

            set
            {
                _isExpanded = value;
            }
        }

        public virtual void OnClick(View v)
        {
            if (ParentItemClickListener != null)
            {
                Expanded = !_isExpanded;
                ParentItemClickListener.OnParentItemClickListener(LayoutPosition);
            }
        }

      /*  public void SetCustomClickableViewOnly(int clickableViewId)
        {
            _clickableView = ItemView.FindViewById(clickableViewId);
            ItemView.SetOnClickListener(null);
            _clickableView.SetOnClickListener(this);

            if (_rotationEnabled)
            {
                _clickableView.Rotation = _rotation;
            }
        }*/

       /* public void SetCustomClickableViewAndItem(int clickableViewId)
        {
            _clickableView = ItemView.FindViewById(clickableViewId);
            ItemView.SetOnClickListener(this);
            _clickableView.SetOnClickListener(this);
            if (_rotationEnabled)
            {
                _clickableView.Rotation = _rotation;
            }
        }*/

       /* public void CancelAnimation()
        {
            _rotationEnabled = false;
            if (_rotationEnabled)
            {
                _clickableView.Rotation = _rotation;
            }
        }*/

        public void SetMainItemClickToExpand()
        {
            ItemView.SetOnClickListener(this);
        }
    }
}