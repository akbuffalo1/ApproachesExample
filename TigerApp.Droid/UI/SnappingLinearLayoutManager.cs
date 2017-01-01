#region using

using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;

#endregion

namespace TigerApp.Droid.UI
{
    public class SnappingLinearLayoutManager : LinearLayoutManager
    {
        public SnappingLinearLayoutManager(Context context, int orientation, bool reverseLayout)
            : base(context, orientation, reverseLayout)
        {
        }

        public override void SmoothScrollToPosition(RecyclerView recyclerView, RecyclerView.State state,
            int position)
        {
            LinearSmoothScroller smoothScroller = new TopSnappedSmoothScroller(recyclerView.Context, this);
            smoothScroller.TargetPosition = position;
            StartSmoothScroll(smoothScroller);
        }

        private class TopSnappedSmoothScroller : LinearSmoothScroller
        {
            private readonly SnappingLinearLayoutManager _manager;

            public TopSnappedSmoothScroller(Context context, SnappingLinearLayoutManager manager) : base(context)
            {
                _manager = manager;
            }

            public override PointF ComputeScrollVectorForPosition(int targetPosition)
            {
                return _manager.ComputeScrollVectorForPosition(targetPosition);
            }

            protected override int VerticalSnapPreference
            {
                get { return SnapToStart; }
            }
        }
    }
}