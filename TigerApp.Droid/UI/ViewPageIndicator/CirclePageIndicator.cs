#region using

using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using R = TigerApp.Droid.Resource;

#endregion

namespace TigerApp.Droid.UI.ViewPageIndicator
{
    /**
      * Draws circles (one for each view). The current view position is filled and
      * others are only stroked.
      */

    public class CirclePageIndicator : View, IPageIndicator
    {
        private static int INVALID_POINTER = -1;

        private float mRadius;
        private Paint mPaintPageFill = new Paint(PaintFlags.AntiAlias);
        private Paint mPaintStroke = new Paint(PaintFlags.AntiAlias);
        private Paint mPaintFill = new Paint(PaintFlags.AntiAlias);
        private ViewPager mViewPager;
        private ViewPager.IOnPageChangeListener mListener;
        private int mCurrentPage;
        private int mSnapPage;
        private float mPageOffset;
        private int mScrollState;
        private Android.Widget.Orientation mOrientation;
        private bool mCentered;
        private bool mSnap;

        private int mTouchSlop;
        private float mLastMotionX = -1;
        private int mActivePointerId = INVALID_POINTER;
        private bool mIsDragging;


        public CirclePageIndicator(Context context)
            : this(context, null)
        {
        }

        public CirclePageIndicator(Context context, IAttributeSet attrs)
            : this(context, attrs, R.Attribute.vpiCirclePageIndicatorStyle)
        {
        }

        public CirclePageIndicator(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            if (IsInEditMode) return;

            //Load defaults from resources
            Resources res = Resources;
            Color defaultPageColor = res.GetColor(R.Color.default_circle_indicator_page_color);
            Color defaultFillColor = res.GetColor(R.Color.default_circle_indicator_fill_color);
            int defaultOrientation = res.GetInteger(R.Integer.default_circle_indicator_orientation);
            Color defaultStrokeColor = res.GetColor(R.Color.default_circle_indicator_stroke_color);
            float defaultStrokeWidth = res.GetDimension(R.Dimension.default_circle_indicator_stroke_width);
            float defaultRadius = res.GetDimension(R.Dimension.default_circle_indicator_radius);
            bool defaultCentered = res.GetBoolean(R.Boolean.default_circle_indicator_centered);
            bool defaultSnap = res.GetBoolean(R.Boolean.default_circle_indicator_snap);

            //Retrieve styles attributes
            TypedArray a = context.ObtainStyledAttributes(attrs, R.Styleable.CirclePageIndicator, defStyle, 0);

            mCentered = a.GetBoolean(R.Styleable.CirclePageIndicator_centered, defaultCentered);
            mOrientation =
                (Android.Widget.Orientation)
                a.GetInt(R.Styleable.CirclePageIndicator_android_orientation, defaultOrientation);
            mPaintPageFill.SetStyle(Paint.Style.Fill);
            mPaintPageFill.Color = a.GetColor(R.Styleable.CirclePageIndicator_pageColor, defaultPageColor);
            mPaintStroke.SetStyle(Paint.Style.Stroke);
            mPaintStroke.Color = a.GetColor(R.Styleable.CirclePageIndicator_strokeColor, defaultStrokeColor);
            mPaintStroke.StrokeWidth = a.GetDimension(R.Styleable.CirclePageIndicator_strokeWidth, defaultStrokeWidth);
            mPaintFill.SetStyle(Paint.Style.Fill);
            mPaintFill.Color = a.GetColor(R.Styleable.CirclePageIndicator_fillColor, defaultFillColor);
            mRadius = a.GetDimension(R.Styleable.CirclePageIndicator_radius, defaultRadius);
            mSnap = a.GetBoolean(R.Styleable.CirclePageIndicator_snap, defaultSnap);

            Drawable background = a.GetDrawable(R.Styleable.CirclePageIndicator_android_background);
            if (background != null)
                SetBackgroundDrawable(background);

            a.Recycle();

            ViewConfiguration configuration = ViewConfiguration.Get(context);
            mTouchSlop = ViewConfigurationCompat.GetScaledPagingTouchSlop(configuration);
        }


        public void setCentered(bool centered)
        {
            mCentered = centered;
            Invalidate();
        }

        public bool isCentered()
        {
            return mCentered;
        }

        public void setPageColor(Color pageColor)
        {
            mPaintPageFill.Color = pageColor;
            Invalidate();
        }

        public Color getPageColor()
        {
            return mPaintPageFill.Color;
        }

        public void setFillColor(Color fillColor)
        {
            mPaintFill.Color = fillColor;
            Invalidate();
        }

        public Color getFillColor()
        {
            return mPaintFill.Color;
        }

        public void setOrientation(Android.Widget.Orientation orientation)
        {
            switch (orientation)
            {
                case Android.Widget.Orientation.Horizontal:
                case Android.Widget.Orientation.Vertical:
                    mOrientation = orientation;
                    RequestLayout();
                    break;

                default:
                    throw new Java.Lang.IllegalArgumentException("Orientation must be either HORIZONTAL or VERTICAL.");
            }
        }

        public Android.Widget.Orientation getOrientation()
        {
            return mOrientation;
        }

        public void setStrokeColor(Color strokeColor)
        {
            mPaintStroke.Color = strokeColor;
            Invalidate();
        }

        public int getStrokeColor()
        {
            return mPaintStroke.Color;
        }

        public void setStrokeWidth(float strokeWidth)
        {
            mPaintStroke.StrokeWidth = strokeWidth;
            Invalidate();
        }

        public float getStrokeWidth()
        {
            return mPaintStroke.StrokeWidth;
        }

        public void setRadius(float radius)
        {
            mRadius = radius;
            Invalidate();
        }

        public float getRadius()
        {
            return mRadius;
        }

        public void setSnap(bool snap)
        {
            mSnap = snap;
            Invalidate();
        }

        public bool isSnap()
        {
            return mSnap;
        }

        //@Override
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (mViewPager == null)
                return;
            int count = mViewPager.Adapter.Count;
            if (count == 0)
                return;

            if (mCurrentPage >= count)
            {
                setCurrentItem(count - 1);
                return;
            }

            int longSize;
            int longPaddingBefore;
            int longPaddingAfter;
            int shortPaddingBefore;
            if (mOrientation == Android.Widget.Orientation.Horizontal)
            {
                longSize = Width;
                longPaddingBefore = PaddingLeft;
                longPaddingAfter = PaddingRight;
                shortPaddingBefore = PaddingTop;
            }
            else
            {
                longSize = Height;
                longPaddingBefore = PaddingTop;
                longPaddingAfter = PaddingBottom;
                shortPaddingBefore = PaddingLeft;
            }

            float threeRadius = mRadius * 3;
            float shortOffset = shortPaddingBefore + mRadius;
            float longOffset = longPaddingBefore + mRadius;
            if (mCentered)
                longOffset += ((longSize - longPaddingBefore - longPaddingAfter) / 2.0f) -
                              ((count * threeRadius) / 2.0f);

            float dX;
            float dY;

            float pageFillRadius = mRadius;
            if (mPaintStroke.StrokeWidth > 0)
                pageFillRadius -= mPaintStroke.StrokeWidth / 2.0f;

            //Draw stroked circles
            for (int iLoop = 0; iLoop < count; iLoop++)
            {
                float drawLong = longOffset + (iLoop * threeRadius);
                if (mOrientation == Android.Widget.Orientation.Horizontal)
                {
                    dX = drawLong;
                    dY = shortOffset;
                }
                else
                {
                    dX = shortOffset;
                    dY = drawLong;
                }
                // Only paint fill if not completely transparent
                if (mPaintPageFill.Alpha > 0)
                    canvas.DrawCircle(dX, dY, pageFillRadius, mPaintPageFill);

                // Only paint stroke if a stroke width was non-zero
                if (pageFillRadius != mRadius)
                    canvas.DrawCircle(dX, dY, mRadius, mPaintStroke);
            }

            //Draw the filled circle according to the current scroll
            float cx = (mSnap ? mSnapPage : mCurrentPage) * threeRadius;
            if (!mSnap)
                cx += mPageOffset * threeRadius;
            if (mOrientation == Android.Widget.Orientation.Horizontal)
            {
                dX = longOffset + cx;
                dY = shortOffset;
            }
            else
            {
                dX = shortOffset;
                dY = longOffset + cx;
            }
            canvas.DrawCircle(dX, dY, mRadius, mPaintFill);
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            if (base.OnTouchEvent(ev))
                return true;
            if ((mViewPager == null) || (mViewPager.Adapter.Count == 0))
                return false;

            MotionEventActions action = ev.Action & MotionEventActions.Mask;
            switch (action)
            {
                case MotionEventActions.Down:
                    mActivePointerId = MotionEventCompat.GetPointerId(ev, 0);
                    mLastMotionX = ev.GetX();
                    break;

                case MotionEventActions.Move:
                {
                    int activePointerIndex = MotionEventCompat.FindPointerIndex(ev, mActivePointerId);
                    float x = MotionEventCompat.GetX(ev, activePointerIndex);
                    float deltaX = x - mLastMotionX;

                    if (!mIsDragging)
                        if (Math.Abs(deltaX) > mTouchSlop)
                            mIsDragging = true;

                    if (mIsDragging)
                    {
                        mLastMotionX = x;
                        if (mViewPager.IsFakeDragging || mViewPager.BeginFakeDrag())
                            mViewPager.FakeDragBy(deltaX);
                    }

                    break;
                }

                case MotionEventActions.Cancel:
                case MotionEventActions.Up:
                    if (!mIsDragging)
                    {
                        int count = mViewPager.Adapter.Count;
                        int width = Width;
                        float halfWidth = width / 2f;
                        float sixthWidth = width / 6f;

                        if ((mCurrentPage > 0) && (ev.GetX() < halfWidth - sixthWidth))
                        {
                            if (action != MotionEventActions.Cancel)
                                mViewPager.SetCurrentItem(mCurrentPage - 1, true);
                            return true;
                        }
                        else if ((mCurrentPage < count - 1) && (ev.GetX() > halfWidth + sixthWidth))
                        {
                            if (action != MotionEventActions.Cancel)
                                mViewPager.SetCurrentItem(mCurrentPage + 1, true);
                            return true;
                        }
                    }

                    mIsDragging = false;
                    mActivePointerId = INVALID_POINTER;
                    if (mViewPager.IsFakeDragging) mViewPager.EndFakeDrag();
                    break;

                case MotionEventActions.PointerDown:
                {
                    int index = MotionEventCompat.GetActionIndex(ev);
                    mLastMotionX = MotionEventCompat.GetX(ev, index);
                    mActivePointerId = MotionEventCompat.GetPointerId(ev, index);
                    break;
                }

                case MotionEventActions.PointerUp:
                    int pointerIndex = MotionEventCompat.GetActionIndex(ev);
                    int pointerId = MotionEventCompat.GetPointerId(ev, pointerIndex);
                    if (pointerId == mActivePointerId)
                    {
                        int newPointerIndex = pointerIndex == 0 ? 1 : 0;
                        mActivePointerId = MotionEventCompat.GetPointerId(ev, newPointerIndex);
                    }
                    mLastMotionX = MotionEventCompat.GetX(ev, MotionEventCompat.FindPointerIndex(ev, mActivePointerId));
                    break;
            }

            return true;
        }

        //@Override
        public void setViewPager(ViewPager view)
        {
            if (mViewPager == view)
                return;
            if (mViewPager != null)
                mViewPager.SetOnPageChangeListener(null);
            if (view.Adapter == null)
                throw new Java.Lang.IllegalStateException("ViewPager does not have adapter instance.");
            mViewPager = view;
            mViewPager.SetOnPageChangeListener(this);
            Invalidate();
        }

        //@Override
        public void setViewPager(ViewPager view, int initialPosition)
        {
            setViewPager(view);
            setCurrentItem(initialPosition);
        }

        //@Override
        public void setCurrentItem(int item)
        {
            if (mViewPager == null)
                throw new Java.Lang.IllegalStateException("ViewPager has not been bound.");
            mViewPager.SetCurrentItem(item, true);
            mCurrentPage = item;
            Invalidate();
        }

        //@Override
        public void notifyDataSetChanged()
        {
            Invalidate();
        }

        //@Override
        public void OnPageScrollStateChanged(int state)
        {
            mScrollState = state;

            if (mListener != null)
                mListener.OnPageScrollStateChanged(state);
        }

        //@Override
        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            mCurrentPage = position;
            mPageOffset = positionOffset;
            Invalidate();

            if (mListener != null)
                mListener.OnPageScrolled(position, positionOffset, positionOffsetPixels);
        }

        //@Override
        public void OnPageSelected(int position)
        {
            if (mSnap || mScrollState == ViewPager.ScrollStateIdle)
            {
                mCurrentPage = position;
                mSnapPage = position;
                Invalidate();
            }

            if (mListener != null)
                mListener.OnPageSelected(position);
        }

        //@Override
        public void setOnPageChangeListener(ViewPager.IOnPageChangeListener listener)
        {
            mListener = listener;
        }

        /*
         * (non-Javadoc)
         *
         * @see android.view.View#onMeasure(int, int)
         */
        //@Override
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (mOrientation == Android.Widget.Orientation.Horizontal)
                SetMeasuredDimension(measureLong(widthMeasureSpec), measureShort(heightMeasureSpec));
            else
                SetMeasuredDimension(measureShort(widthMeasureSpec), measureLong(heightMeasureSpec));
        }

        /**
         * Determines the width of this view
         *
         * @param measureSpec
         *            A measureSpec packed into an int
         * @return The width of the view, honoring constraints from measureSpec
         */

        private int measureLong(int measureSpec)
        {
            int result;
            MeasureSpecMode specMode = MeasureSpec.GetMode(measureSpec);
            int specSize = MeasureSpec.GetSize(measureSpec);

            if ((specMode == MeasureSpecMode.Exactly) || (mViewPager == null))
            {
                //We were told how big to be
                result = specSize;
            }
            else
            {
                //Calculate the width according the views count
                int count = mViewPager.Adapter.Count;
                result = (int) (PaddingLeft + PaddingRight
                                + (count * 2 * mRadius) + (count - 1) * mRadius + 1);
                //Respect AT_MOST value if that was what is called for by measureSpec
                if (specMode == MeasureSpecMode.AtMost)
                    result = Math.Min(result, specSize);
            }
            return result;
        }

        /**
         * Determines the height of this view
         *
         * @param measureSpec
         *            A measureSpec packed into an int
         * @return The height of the view, honoring constraints from measureSpec
         */

        private int measureShort(int measureSpec)
        {
            int result;

            MeasureSpecMode specMode = MeasureSpec.GetMode(measureSpec);
            int specSize = MeasureSpec.GetSize(measureSpec);

            if (specMode == MeasureSpecMode.Exactly)
            {
                //We were told how big to be
                result = specSize;
            }
            else
            {
                //Measure the height
                result = (int) (2 * mRadius + PaddingTop + PaddingBottom + 1);
                //Respect AT_MOST value if that was what is called for by measureSpec
                if (specMode == MeasureSpecMode.AtMost)
                    result = Math.Min(result, specSize);
            }
            return result;
        }

        //@Override
        protected override void OnRestoreInstanceState(IParcelable state)
        {
            SavedState savedState = (SavedState) state;
            base.OnRestoreInstanceState(savedState.SuperState);
            mCurrentPage = savedState.currentPage;
            mSnapPage = savedState.currentPage;
            RequestLayout();
        }

        //@Override
        protected override IParcelable OnSaveInstanceState()
        {
            IParcelable superState = base.OnSaveInstanceState();
            SavedState savedState = new SavedState(superState);
            savedState.currentPage = mCurrentPage;
            return savedState;
        }

        class SavedState : BaseSavedState
        {
            public int currentPage;

            public SavedState(IParcelable superState)
                : base(superState)
            {
            }

            private SavedState(Parcel pin)
                : base(pin)
            {
                currentPage = pin.ReadInt();
            }

            //@Override
            public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
            {
                base.WriteToParcel(dest, flags);
                dest.WriteInt(currentPage);
            }

            class PCreator : Java.Lang.Object, IParcelableCreator
            {
                public Java.Lang.Object CreateFromParcel(Parcel source)
                {
                    return new SavedState(source);
                }

                public Java.Lang.Object[] NewArray(int size)
                {
                    return new SavedState[size];
                }
            }

            public IParcelableCreator CREATOR = new PCreator();

            //@SuppressWarnings("UnusedDeclaration")
            //public Parcelable.Creator<SavedState> CREATOR = new Parcelable.Creator<SavedState>() {
            //    @Override
            //    public SavedState createFromParcel(Parcel in) {
            //        return new SavedState(in);
            //    }

            //    @Override
            //    public SavedState[] newArray(int size) {
            //        return new SavedState[size];
            //    }
            //};
        }
    }
}