using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TigerApp.Droid.Utils;

namespace TigerApp.Droid.Tutorial
{
    public class ImageTutorialController
    {
        public static void ShowTutorial(Context context, List<ImageTutorialModel> tutorialImages)
        {
            new TutorialController(context, tutorialImages).Start();
        }

        private class TutorialController
        {
            private readonly Context _context;
            private readonly List<ImageTutorialModel> _tutorialImages;
            private int _index;
            private Dialog _dialog;
            private ImageView _imageContainer;
            private FrameLayout _dialogLayout;
            private ImageTutorialModel _currentModel;

            public TutorialController(Context context, List<ImageTutorialModel> tutorialImages)
            {
                _context = context;
                _tutorialImages = tutorialImages;
            }

            public void Start()
            {
                _dialog = new Dialog(_context, Resource.Style.ImageTutorialStyle);
                _dialog.SetContentView(Resource.Layout.dialog_tutorial_layout);

                _currentModel = _tutorialImages[0];
                _imageContainer = _dialog.FindViewById<ImageView>(Resource.Id.imageContainer);
                _imageContainer.SetImageResource(_currentModel.ImageResourceId);
                _dialogLayout = _dialog.FindViewById<FrameLayout>(Resource.Id.dialogLayout);
                _dialogLayout.Click += OnDialodClick;

                _dialogLayout.LayoutChange += (sender, args) =>
                {
                    float x;
                    if(_currentModel.IsFlipped)
                        _imageContainer.SetY(_currentModel.AnchorView.GetY() + _imageContainer.Height + ScreenUtils.Dp2Px(_context, _currentModel.yShiftDp));
                    else
                        _imageContainer.SetY(_currentModel.AnchorView.GetY() - ScreenUtils.Dp2Px(_context, _currentModel.yShiftDp));

                    if (_currentModel.Gravity == GravityFlags.Start)
                    {
                        var cx = _currentModel.AnchorView.Left - _imageContainer.Width +
                                 _currentModel.AnchorView.Width;
                        x = Math.Max(cx, 0);
                    }
                    else if (_currentModel.Gravity == GravityFlags.End)
                    {
                        x = Math.Min(_currentModel.AnchorView.Left, _dialogLayout.Width - _imageContainer.Width);
                    }
                    else
                    {
                        throw new Exception("unsupported gravity");
                    }

                    _imageContainer.SetX(x);

                };

                _dialog.Show();
            }

            private void OnDialodClick(object sender, EventArgs e)
            {
                _index++;

                if (_index == _tutorialImages.Count)
                {
                    _dialog.Dismiss();
                    return;
                }

                _currentModel = _tutorialImages[_index];
                _imageContainer = _dialog.FindViewById<ImageView>(Resource.Id.imageContainer);
                _imageContainer.SetImageResource(_currentModel.ImageResourceId);
                _imageContainer.Invalidate();
                _dialogLayout.Invalidate();
            }
        }
    }
}