#region using

using System;
using System.Text;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using TigerApp.Droid.Utils;

#endregion

namespace TigerApp.Droid.Tutorial.SlidingTutorial
{
    public sealed class TutorialFragment : Fragment
    {
        private const string KEY_CONTENT = "TutorialFragment:Content";

        public static TutorialFragment newInstance(int contentId)
        {
            TutorialFragment fragment = new TutorialFragment();

            fragment._content = contentId;

            return fragment;
        }

        public event Action OnFragmentNextClickEvent;

        private int _content = 0;
        private ImageView _imageView;
        private LinearLayout _layout;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if ((savedInstanceState != null) && savedInstanceState.ContainsKey(KEY_CONTENT))
                _content = savedInstanceState.GetInt(KEY_CONTENT);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _imageView = new ImageView(Activity);
            _imageView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent);


            _layout = new LinearLayout(Activity);
            _layout.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent);
            _layout.SetGravity(GravityFlags.Center);

            if (_content != 0)
            {
                _imageView.SetImageResource(_content);
                _imageView.SetScaleType(ImageView.ScaleType.FitXy);
                _layout.AddView(_imageView);
            }

            _layout.Click += (sender, args) => { OnFragmentNextClickEvent?.Invoke(); };
            return _layout;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            PlatformUtil.UnbindDrawables(_layout);

            _imageView = null;
            _layout = null;

            GC.Collect();
            Java.Lang.Runtime.GetRuntime().Gc();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt(KEY_CONTENT, _content);
        }
    }
}