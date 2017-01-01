using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using TigerApp.Droid.UI.SwipeCards;
using TigerApp.Droid.Utils;
using TigerApp.Shared.Models;

namespace TigerApp.Droid.UI
{
    interface ICardActionDispatcher
    {
        event Action<ProductVote, Product> OnCardActionEvent;
        event Action<ProductVote, Product> OnCardSwipeActionEvent;
    }

    public class ProductCard : CardView, ICardActionDispatcher
    {
        public Product Product;
        public bool Voted { get; private set; }

        private const int PuntoDXinDp = 50;
        private int _puntoDXinPX;

        private ImageButton _btnDislike;
        private ImageButton _btnLike;
        private ImageView _imgProduct;
        private ImageView _imgPunto;
        private Animation _puntoApearence;
        private ImageView _btnLikeOver;
        private ImageView _btnDislikeOver;
        private ImageButton _btnBack;
        private float _progressToDiscard;
        private ImageButton _btnShare;


        public ProductCard(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ProductCard(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init(context);
        }

        public ProductCard(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(context);
        }

        public ProductCard(Context context) : base(context)
        {
            Init(context);
        }

        public event Action<ProductVote, Product> OnCardActionEvent;
        public event Action<ProductVote, Product> OnCardSwipeActionEvent;

        private void Init(Context context)
        {
            _puntoApearence = AnimationUtils.LoadAnimation(context, Resource.Animation.punto_apearence);
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();

            _imgPunto = FindViewById<ImageView>(Resource.Id.imgPunto);
            _imgProduct = FindViewById<ImageView>(Resource.Id.imgProduct);
            _btnLike = FindViewById<ImageButton>(Resource.Id.btnLike);
            _btnDislike = FindViewById<ImageButton>(Resource.Id.btnDislike);
            _btnLikeOver = FindViewById<ImageView>(Resource.Id.btnLikeOver);
            _btnDislikeOver = FindViewById<ImageView>(Resource.Id.btnDislikeOver);
            _btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
            _btnShare = FindViewById<ImageButton>(Resource.Id.btnShare);

            _btnShare.Click += (sender, args) => { OnCardActionEvent?.Invoke(ProductVote.Share, Product); };
            _btnLike.Click += (sender, args) => { AddPunto(ProductVote.Like); };
            _btnDislike.Click += (sender, args) => { AddPunto(ProductVote.Dislike); };
            _btnBack.Click += (sender, args) =>
            {
                Voted = true;
                OnCardActionEvent?.Invoke(ProductVote.Ignore, Product);
            };

            _imgPunto.SetX(_imgProduct.Width * 0.5f - _imgPunto.Width * 0.5f);
            _imgPunto.SetY(_imgProduct.Height * 0.5f + _imgProduct.GetY());

            _puntoDXinPX = ScreenUtils.Dp2Px(Context, PuntoDXinDp);
            Invalidate();
        }

        private void AddPunto(ProductVote action)
        {
            _btnLike.Clickable = false;
            _btnDislike.Clickable = false;

            if (AD.Resolver.Resolve<Shared.Services.API.IProfileApiService>().IsLoggedIn) {
                _imgPunto.SetX(_imgProduct.Width * 0.5f);
                _imgPunto.SetY(_imgProduct.Height * 0.5f + _imgProduct.GetY());
                _imgPunto.Visibility = ViewStates.Visible;
                _imgPunto.Alpha = 1;
                _imgPunto.StartAnimation(_puntoApearence);
            }

            if (action == ProductVote.Like)
            {
                _btnLikeOver.Visibility = ViewStates.Visible;
                _btnLikeOver.Alpha = 1;
            }
            else
            {
                _btnDislikeOver.Visibility = ViewStates.Visible;
                _btnDislikeOver.Alpha = 1;
            }
            Voted = true;
            OnCardActionEvent?.Invoke(action, Product);

            Invalidate();
        }

        public float ProgressToDiscad
        {
            get { return _progressToDiscard; }
            set
            {
                _progressToDiscard = value;
                var alpha = Math.Min(1, Math.Abs(_progressToDiscard));
                if (_progressToDiscard < 0)
                {
                    _btnDislikeOver.Alpha = alpha;
                    _btnLikeOver.Alpha = 0;
                    _btnLikeOver.Visibility = ViewStates.Gone;
                    _btnDislikeOver.Visibility = ViewStates.Visible;
                    _imgPunto.SetX(_imgProduct.Width * 0.5f - _imgPunto.Width * 0.5f + alpha * _puntoDXinPX);
                }
                else if (_progressToDiscard > 0)
                {
                    _btnDislikeOver.Alpha = 0;
                    _btnLikeOver.Alpha = alpha;
                    _btnLikeOver.Visibility = ViewStates.Visible;
                    _btnDislikeOver.Visibility = ViewStates.Gone;
                    _imgPunto.SetX(_imgProduct.Width * 0.5f - _imgPunto.Width * 0.5f - alpha * _puntoDXinPX);
                }
                else
                {
                    _imgPunto.Alpha = 0;
                    _btnLikeOver.Alpha = 0;
                    _btnDislikeOver.Alpha = 0;
                    _btnLikeOver.Visibility = ViewStates.Gone;
                    _btnDislikeOver.Visibility = ViewStates.Gone;
                }

                if (_progressToDiscard != 0 && AD.Resolver.Resolve<Shared.Services.API.IProfileApiService>().IsLoggedIn)
                {
                    _imgPunto.SetY(_imgProduct.Height * 0.5f + _imgProduct.GetY());
                    _imgPunto.Visibility = ViewStates.Visible;
                    _imgPunto.Alpha = alpha;
                }

                Invalidate();
            }
        }


        internal class CardSwipeListener : CardStack.ICardEventListener
        {
            private readonly int _discardDistancePx;
            private readonly CardStack _cardStack;

            public CardSwipeListener(int discardDistancePx, CardStack cardStack)
            {
                _discardDistancePx = discardDistancePx;
                _cardStack = cardStack;
            }

            public bool SwipeEnd(int section, float x1, float y1, float x2, float y2)
            {
                //var distance = CardUtils.Distance(x1, y1, x2, y2);
                //Discard card only if user moves card to RIGHT/LEFT
                var discard = Math.Abs(x2 - x1) > _discardDistancePx;
                var cardView = _cardStack.TopView as ProductCard;
                if (!discard) cardView.ProgressToDiscad = 0;
                if (discard)
                {
                    if (x2 < x1)
                        cardView.OnCardSwipeActionEvent?.Invoke(ProductVote.Dislike, cardView.Product);
                    else
                        cardView.OnCardSwipeActionEvent?.Invoke(ProductVote.Like, cardView.Product);
                }

                return discard;
            }

            public bool SwipeStart(int section, float x1, float y1, float x2, float y2)
            {
                var cardView = _cardStack.TopView as ProductCard;
                return false;
            }

            public bool SwipeContinue(int section, float x1, float y1, float x2, float y2)
            {
                var cardView = _cardStack.TopView as ProductCard;
                cardView.ProgressToDiscad = (x2 - x1) / _discardDistancePx;

                return false;
            }

            public void Discarded(int mIndex, int direction)
            {

            }

            public void TopCardTapped()
            {

            }
        }
    }
}