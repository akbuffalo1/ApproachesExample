using System;
using Android.Animation;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using TigerApp.Droid.Utils;
using UK.CO.Chrisjenx.Calligraphy;

namespace TigerApp.Droid.UI
{
    public class ExpProgressBar : View
    {
        private float _progress = 0;
        private float _strokeWidth;
        private float _backgroundStrokeWidth;
        private Color _color = Android.Graphics.Color.Black;
        private Color _backgroundColor = Android.Graphics.Color.Gray;

        private int _startAngle = -90;
        private RectF _rectF;
        private Paint _backgroundPaint;
        private Paint _foregroundPaint;

        private Paint _progressTextPaint;
        private float _progressTextX;
        private float _progressTextY;
        private float _progresTextSize;

        private Paint _progressTextPaintW;

        private Paint _hintTextPaint;
        private float _hintTextX;
        private float _hintTextY;
        private float _hintTextSize;

        private Paint _linePaint;
        private Path _linePath;
        private Typeface _progressFace;
        private Typeface _hintFace;
        private float _textShift;
        private int _padding;
        private float _currentValue = 2350;
        private string _hintText = "liv.2";
        private float _minValue = 2000;
        private float _maxValue = 2400;

        public ExpProgressBar(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(context, attrs);
        }

        private void Init(Context context, IAttributeSet attrs)
        {
            _strokeWidth = Resources.GetDimension(Resource.Dimension.default_stroke_width);
            _backgroundStrokeWidth = Resources.GetDimension(Resource.Dimension.default_background_stroke_width);

            _rectF = new RectF();
            TypedArray typedArray = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.ExpProgressBar, 0, 0);
            //Reading values from the XML layout
            try
            {
                // Value
                _progress = typedArray.GetFloat(Resource.Styleable.ExpProgressBar_cpb_progress, _progress);
                // _strokeWidth
                _strokeWidth = typedArray.GetDimension(Resource.Styleable.ExpProgressBar_cpb_progressbar_width, _strokeWidth);
                _backgroundStrokeWidth = typedArray.GetDimension(Resource.Styleable.ExpProgressBar_cpb_background_progressbar_width, _backgroundStrokeWidth);
                // Color
                _color = typedArray.GetColor(Resource.Styleable.ExpProgressBar_cpb_progressbar_color, _color);
                _backgroundColor = typedArray.GetColor(Resource.Styleable.ExpProgressBar_cpb_background_progressbar_color, _backgroundColor);

                //text
                _progressFace =
                    TypefaceUtils.Load(context.Assets,
                        typedArray.GetString(Resource.Styleable.ExpProgressBar_cpb_text_progress_font));

                _hintFace =
                    TypefaceUtils.Load(context.Assets,
                        typedArray.GetString(Resource.Styleable.ExpProgressBar_cpb_text_hint_font));

                //TODO default values
                _progresTextSize = typedArray.GetDimension(Resource.Styleable.ExpProgressBar_cpb_text_progress_size, _strokeWidth);
                _hintTextSize = typedArray.GetDimension(Resource.Styleable.ExpProgressBar_cpb_text_hint_size, _strokeWidth);
                _textShift = typedArray.GetDimension(Resource.Styleable.ExpProgressBar_cpb_text_shift, _strokeWidth);
                _padding = (int)typedArray.GetDimension(Resource.Styleable.ExpProgressBar_cpb_padding, _strokeWidth);

            }
            finally
            {
                typedArray.Recycle();
            }

            // Init Background
            _backgroundPaint = new Paint(PaintFlags.AntiAlias);
            _backgroundPaint.Color = _backgroundColor;
            _backgroundPaint.SetStyle(Paint.Style.Stroke);
            _backgroundPaint.StrokeWidth = _backgroundStrokeWidth;

            // Init Foreground
            _foregroundPaint = new Paint(PaintFlags.AntiAlias);
            _foregroundPaint.Color = _color;
            _foregroundPaint.SetStyle(Paint.Style.Stroke);
            _foregroundPaint.StrokeWidth = _strokeWidth;

            //progress
            _progressTextPaint = new Paint();
            _progressTextPaint.Color = Color.Black;
            _progressTextPaint.SetTypeface(_progressFace);
            _progressTextPaint.AntiAlias = true;
            _progressTextPaint.TextSize = _progresTextSize;

            _progressTextPaintW = new Paint();
            _progressTextPaintW.Color = Color.White;
            _progressTextPaintW.SetTypeface(_progressFace);
            _progressTextPaintW.AntiAlias = true;
            _progressTextPaintW.TextSize = _progresTextSize;

            //init hint text
            _hintTextPaint = new Paint();
            _hintTextPaint.Color = Color.Black;
            _hintTextPaint.SetTypeface(_hintFace);
            _hintTextPaint.AntiAlias = true;
            _hintTextPaint.TextSize = _hintTextSize;

            //init dashed line paint
            _linePaint = new Paint();
            _linePaint.Color = Color.Black;
            _linePaint.SetStyle(Paint.Style.Stroke);
            _linePaint.StrokeWidth = ScreenUtils.Dp2Px(context, 2);
            _linePaint.SetPathEffect(new DashPathEffect(new float[] { ScreenUtils.Dp2Px(context, 4), ScreenUtils.Dp2Px(context, 2) }, 0));
            _linePath = new Path();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            var centerX = _rectF.Width() * 0.5f;
            canvas.DrawOval(_rectF, _backgroundPaint);
            float angle = 360 * _progress / 100;
            canvas.DrawArc(_rectF, _startAngle, angle, false, _foregroundPaint);

            CalculateTextParameters(centerX + _backgroundStrokeWidth);

            canvas.DrawText(_currentValue.ToString(), _progressTextX-ScreenUtils.Dp2Px(Context, 2), _progressTextY+ ScreenUtils.Dp2Px(Context, 2), _progressTextPaintW);
            canvas.DrawText(_currentValue.ToString(), _progressTextX, _progressTextY, _progressTextPaint);
            canvas.DrawText(_hintText, _hintTextX, _hintTextY, _hintTextPaint);

            _linePath.MoveTo(centerX + _backgroundStrokeWidth - _linePaint.StrokeWidth * 0.5f, 0);
            _linePath.LineTo(centerX + _backgroundStrokeWidth, _padding + _backgroundStrokeWidth + _padding);
            canvas.DrawPath(_linePath, _linePaint);
        }

        private void CalculateTextParameters(float radius)
        {
            //TODO fit text size to component width
            _progressTextX = radius - _progressTextPaint.MeasureText(_currentValue.ToString()) / 2f + _padding;
            _progressTextY = radius + _textShift + _padding;//+ (float)((double)_progressTextPaint.TextSize);

            _hintTextX = radius - _hintTextPaint.MeasureText(_hintText) / 2f + _padding;
            _hintTextY = radius + (float)_hintTextPaint.TextSize + _textShift + _padding;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int height = GetDefaultSize(SuggestedMinimumHeight, heightMeasureSpec);

            int width = GetDefaultSize(SuggestedMinimumWidth, widthMeasureSpec);

            int min = Math.Min(width + _padding, height + _padding);
            SetMeasuredDimension(min, min);
            float highStroke = (_strokeWidth > _backgroundStrokeWidth) ? _strokeWidth : _backgroundStrokeWidth;
            _rectF.Set(_padding + highStroke / 2, _padding + highStroke / 2, min - _padding - highStroke / 2, min - _padding - highStroke / 2);
        }

        public virtual float Progress
        {
            get { return _progress; }
            set
            {
                this._progress = (value <= 100) ? value : 100;
                Invalidate();
            }
        }

        public float CurrentValue
        {
            get { return _currentValue; }
            set
            {
                _currentValue = value;
                Progress = CalculatePercentage(_currentValue);
            }
        }

        private float CalculatePercentage(float currentValue)
        {
            float num1 = MaxValue - MinValue;
            float num2 = MaxValue - currentValue;
            if (num1 == 0.0)
                return 0.0f;
            return (float)((1.0 - (double)num2 / (double)num1) * 100.0);
        }

        public float MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                Invalidate();
            }
        }

        public float MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                Invalidate();
            }
        }

        public string HintText
        {
            get { return _hintText; }
            set
            {
                _hintText = value;
                Invalidate();
            }
        }

        public float ProgresTextSize
        {
            get { return _progresTextSize; }
            set
            {
                _progresTextSize = value;
                _progressTextPaint.TextSize = _progresTextSize;
                _progressTextPaintW.TextSize = _progresTextSize;
                Invalidate();
            }
        }

        public float TextShift
        {
            get { return _textShift; }
            set { _textShift = value; Invalidate(); }
        }

        public virtual float ProgressBarWidth
        {
            get { return _strokeWidth; }
            set
            {
                this._strokeWidth = value;
                _foregroundPaint.StrokeWidth = value;
                RequestLayout(); //Because it should recalculate its bounds
                Invalidate();
            }
        }


        public virtual float BackgroundProgressBarWidth
        {
            get { return _backgroundStrokeWidth; }
            set
            {
                this._backgroundStrokeWidth = value;
                _backgroundPaint.StrokeWidth = value;
                RequestLayout(); //Because it should recalculate its bounds
                Invalidate();
            }
        }


        public virtual Color Color
        {
            get { return _color; }
            set
            {
                this._color = value;
                _foregroundPaint.Color = value;
                Invalidate();
                RequestLayout();
            }
        }


        public virtual Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                this._backgroundColor = value;
                _backgroundPaint.Color = value;
                Invalidate();
                RequestLayout();
            }
        }

        public virtual float ProgressWithAnimation
        {
            set
            {
                SetProgressWithAnimation(value, 1500);
            }
        }

        public virtual void SetProgressWithAnimation(float progress, int duration)
        {
            ValueAnimator objectAnimator = ObjectAnimator.OfInt((int)_minValue, (int)progress);
            objectAnimator.SetDuration(duration);
            objectAnimator.SetInterpolator(new DecelerateInterpolator());
            objectAnimator.Update += (sender, args) =>
            {
                CurrentValue = (int)args.Animation.AnimatedValue;
            };
            objectAnimator.Start();
        }

        public virtual void UpdateProgressWithAnimation(float progress, int duration)
        {
            ValueAnimator objectAnimator = ObjectAnimator.OfInt((int)_currentValue, (int)progress);
            objectAnimator.SetDuration(duration);
            objectAnimator.SetInterpolator(new DecelerateInterpolator());
            objectAnimator.Update += (sender, args) =>
            {
                CurrentValue = (int)args.Animation.AnimatedValue;
            };
            objectAnimator.Start();
        }
    }
}