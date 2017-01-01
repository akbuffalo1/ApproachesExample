#region using

using System;
using Android.Content;
using Android.Util;
using Android.Views;

#endregion

namespace TigerApp.Droid.UI
{
    internal class OnSwipeGestureListener : Java.Lang.Object, View.IOnTouchListener
    {
        //values in Dp
        private const int SWIPE_DISTANCE_THRESHOLD = 100;
        private const int SWIPE_VELOCITY_THRESHOLD = 100;

        public static int Dp2Px(Context context, int dip)
        {
            DisplayMetrics displayMetrics = context.Resources.DisplayMetrics;
            return (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, dip, displayMetrics);
        }

        private readonly GestureDetector _gestureDetector;

        public OnSwipeGestureListener(Context context)
        {
            _gestureDetector = new GestureDetector(
                context,
                new GestureListener(
                    this,
                    Dp2Px(context, SWIPE_DISTANCE_THRESHOLD),
                    Dp2Px(context, SWIPE_VELOCITY_THRESHOLD)
                )
            );
        }

        public event Action OnSwipeRight;
        public event Action OnSwipeLeft;
        public event Action OnSwipeUp;
        public event Action OnSwipeDown;
        public event Action OnClick;
        public event Action OnDoubleClick;
        public event Action OnLongClick;

        public bool OnTouch(View v, MotionEvent e)
        {
            return _gestureDetector.OnTouchEvent(e);
        }

        private class GestureListener : GestureDetector.SimpleOnGestureListener
        {
            private int _swipeDistanceThreshold;
            private int _swipeVelocityThreshold;

            private readonly OnSwipeGestureListener _listener;

            public GestureListener(OnSwipeGestureListener listener, int swipeDistanceThreshold,
                int swipeVelocityThreshold)
            {
                _listener = listener;
                _swipeDistanceThreshold = swipeDistanceThreshold;
                _swipeVelocityThreshold = swipeVelocityThreshold;
            }

            public override bool OnDown(MotionEvent e)
            {
                return true;
            }

            public override bool OnSingleTapUp(MotionEvent e)
            {
                _listener.OnClick?.Invoke();
                return base.OnSingleTapUp(e);
            }

            public override bool OnDoubleTap(MotionEvent e)
            {
                _listener.OnDoubleClick?.Invoke();
                return base.OnDoubleTap(e);
            }

            public override void OnLongPress(MotionEvent e)
            {
                _listener.OnLongClick?.Invoke();
                base.OnLongPress(e);
            }

            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                bool result = false;

                //try\catch used because e1 sometimes can be null
                //http://stackoverflow.com/questions/4151385/android-simpleongesturelistener-onfling-getting-a-null-motionevent
                try
                {
                    float diffY = e2.GetY() - e1.GetY();
                    float diffX = e2.GetX() - e1.GetX();
                    if (Math.Abs(diffX) > Math.Abs(diffY))
                    {
                        if (Math.Abs(diffX) > _swipeDistanceThreshold && Math.Abs(velocityX) > _swipeVelocityThreshold)
                        {
                            if (diffX > 0)
                                _listener.OnSwipeRight?.Invoke();
                            else
                                _listener.OnSwipeLeft?.Invoke();

                            result = true;
                        }
                    }
                    else
                    {
                        if (Math.Abs(diffY) > _swipeDistanceThreshold && Math.Abs(velocityY) > _swipeVelocityThreshold)
                        {
                            if (diffY > 0)
                                _listener.OnSwipeDown?.Invoke();
                            else
                                _listener.OnSwipeUp?.Invoke();

                            result = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    
                }

                return result;
            }
        }
    }
}