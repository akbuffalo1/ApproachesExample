#region using

using System;
using Android.Support.V4.App;
using Java.Lang;

#endregion

namespace TigerApp.Droid.Tutorial.SlidingTutorial
{
    public class TestFragmentAdapter : FragmentPagerAdapter
    {
        public event Action OnFragmentNextClickEvent;

        protected static int[] CONTENT =
        {
            Resource.Drawable.tutorial_vote_sharet,
            Resource.Drawable.tutorial_swipe_right,
            Resource.Drawable.tutorial_swipe_left,
            0
        };

        private int mCount = CONTENT.Length;

        public TestFragmentAdapter(FragmentManager fm)
            : base(fm)
        {
        }

        public override Fragment GetItem(int position)
        {
            var fragment = TutorialFragment.newInstance(CONTENT[position % CONTENT.Length]);
            fragment.OnFragmentNextClickEvent += OnFragmentNextClick;
            return fragment;
        }

        private void OnFragmentNextClick()
        {
            OnFragmentNextClickEvent?.Invoke();
        }

        public override int Count
        {
            get { return mCount; }
        }

        public int getCount()
        {
            return mCount;
        }

        public void setCount(int count)
        {
            if (count > 0 && count <= 10)
            {
                mCount = count;
                NotifyDataSetChanged();
            }
        }
    }
}