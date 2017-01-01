using CoreAnimation;
using CoreGraphics;
using Foundation;
using ReactiveUI;
using Softweb.Xamarin.Controls.iOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using TigerApp.iOS.Utils;
using TigerApp.Shared;
using TigerApp.Shared.Models;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages.CatalogueOfProducts
{
    public partial class CatalogueOfProductsViewController : BaseReactiveViewController<IProductsCatalogueViewModel>, ICardViewDataSource
    {
        private List<Product> _products;
        private int _index;
        private nfloat _xValue;

        private Dictionary<int, UIView> _viewMap = new Dictionary<int, UIView>();
        private Dictionary<int, CardItemViewController> _controllerMap = new Dictionary<int, CardItemViewController>();

        private UIView _noProductsView;

        private void CardItem_LikeDislikeTapped(object sender, EventArgs e)
        {
            var cardItem = sender as CardItemViewController;

            //ViewModel.VoteProduct(cardItem.Product.Id, cardItem.isLikeButtonClicked ? ProductVote.Like : ProductVote.Dislike);

            if (cardItem.isLikeButtonClicked)
            {
                CardView.SwipeTopCardToRight();
            }
            else
            {
                CardView.SwipeTopCardToLeft();
            }
        }

        private void BackButton_TouchUpInside(object sender, EventArgs e)
        {
            AD.Resolver.Resolve<IProductApiService>().GetProductList(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce((_) => { });
            DismissViewController(true, null);
        }

        public CardView CardView { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;

            _noProductsView = CreateNoProductsView();

            CardView = new CardView();

            //Wire up events
            CardView.DidSwipeLeft += OnSwipeLeft;
            CardView.DidSwipeRight += OnSwipeRight;
            CardView.DidCancelSwipe += OnSwipeCancelled;
            CardView.DidStartSwipingCardAtLocation += OnSwipeStarted;
            CardView.SwipingCardAtLocation += OnSwiping;
            CardView.DidEndSwipingCard += OnSwipeEnded;
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            CardView.Center = new CGPoint(View.Center.X, View.Center.Y);
            CardView.Bounds = new CGRect(0f, 0f, View.Bounds.Width, View.Bounds.Height);
            View.AddSubview(CardView);
        }

        private void OnTutorialIscrivimiClicked()
        {
            // NOTE: Goes to HomeViewController
            DismissViewController(true, null);
        }

        public UIView NextCardForCardView(CardView cardView)
        {
            if (_index == _products.Count)
            {
                return null;
            }

            var card = new UIView
            {
                BackgroundColor = UIColor.White,
                Frame = CardView.Bounds
            };

            _viewMap.Add(_index, card);

            var cardItem = new CardItemViewController(_products[_index]);
            cardItem.View.Frame = CardView.Bounds;
            cardItem.View.Layer.EdgeAntialiasingMask = CAEdgeAntialiasingMask.All;
            cardItem.View.Layer.ShadowColor = UIColor.Black.CGColor;
            cardItem.View.Layer.ShadowOffset = new CGSize(0f, 1.5f);
            cardItem.View.Layer.ShouldRasterize = true;
            cardItem.View.Layer.ShadowOpacity = 0.33f;
            cardItem.View.Layer.ShadowRadius = 4.0f;
            cardItem.View.Layer.CornerRadius = 6.0f;
            cardItem.BackButton.TouchUpInside += BackButton_TouchUpInside;

            _controllerMap.Add(_index, cardItem);

            card.AddSubview(cardItem.View);

            //Rasterize card for more efficient animation
            card.Layer.ShouldRasterize = true;

            cardItem.LikeDislikeTapped += CardItem_LikeDislikeTapped;

            _index++;
            return card;
        }

        private UIView CreateNoProductsView()
        {
            var mainView = new UIView() { BackgroundColor = UIColor.White, Hidden = true };
            mainView.TranslatesAutoresizingMaskIntoConstraints = false;

            var mainStack = UICommon.CreateStackView();
            var imageStack = UICommon.CreateStackView(alignment: UIStackViewAlignment.Center);

            var topStripe = new UIView { BackgroundColor = Colors.ColorFromHexString("#FFCD8F") };

            var logoImage = UIImage.FromBundle("FlyingTigerLogo");
            nfloat logoImageAspectRatio = logoImage.Size.Height / logoImage.Size.Width;

            var logoImageView = new UIImageView();
            logoImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            logoImageView.Image = logoImage;
            logoImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            var flagsImage = UIImage.FromBundle("no_products_flags");
            nfloat flagsImageAspectRatio = flagsImage.Size.Height / flagsImage.Size.Width;

            var flagsImageView = new UIImageView();
            flagsImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            flagsImageView.Image = flagsImage;
            flagsImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            var textImage = UIImage.FromBundle("no_products_text");
            nfloat textImageAspectRatio = textImage.Size.Height / textImage.Size.Width;

            var textImageView = new UIImageView();
            textImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            textImageView.Image = textImage;
            textImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            var message = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(37), UIColor.Black);
            message.AttributedText = new NSAttributedString(Constants.Strings.ProductCatalogueNoProductsMessage,
                new UIStringAttributes { ParagraphStyle = new NSMutableParagraphStyle { LineSpacing = 1f, LineHeightMultiple = 0.7f, Alignment = UITextAlignment.Center } });

            var bottomToolbar = new UIToolbar { TintColor = UIColor.Black, BarTintColor = Colors.ColorFromHexString("#FFCD8F"), Translucent = false };
            var bottomRightButton = new UIBarButtonItem(UIImage.FromBundle("catalogue_05_button"), UIBarButtonItemStyle.Plain, (s, e) => { DismissViewController(true, null); });
            bottomToolbar.Items = new UIBarButtonItem[]
            {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                bottomRightButton
            };

            var arrowImage = UIImage.FromBundle("no_products_arrow");
            nfloat arrowImageAspectRatio = textImage.Size.Height / textImage.Size.Width;

            var arrowImageView = new UIImageView();
            arrowImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            arrowImageView.Image = arrowImage;
            arrowImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            mainStack.AddArrangedSubview(topStripe);
            mainStack.AddArrangedSubview(textImageView);
            mainStack.AddArrangedSubview(message);
            mainStack.AddArrangedSubview(bottomToolbar);

            imageStack.AddArrangedSubview(logoImageView);
            imageStack.AddArrangedSubview(flagsImageView);

            mainView.AddSubview(mainStack);
            mainView.AddSubview(imageStack);
            mainView.AddSubview(arrowImageView);

            View.AddSubview(mainView);

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(mainView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(mainView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(mainView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(mainView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),
            });

            mainView.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.Height, 1, 0),

                NSLayoutConstraint.Create(imageStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(imageStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(imageStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.Width, 1, 0),

                NSLayoutConstraint.Create(topStripe, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(topStripe, NSLayoutAttribute.Height, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Height, 0.22f, 0),

                NSLayoutConstraint.Create(logoImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, imageStack, NSLayoutAttribute.Width, 0.55f, 0),
                NSLayoutConstraint.Create(logoImageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, imageStack, NSLayoutAttribute.Top, 1, 30),
                NSLayoutConstraint.Create(logoImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, logoImageView, NSLayoutAttribute.Width, logoImageAspectRatio, 0),

                NSLayoutConstraint.Create(flagsImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, imageStack, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(flagsImageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, logoImageView, NSLayoutAttribute.Bottom, 1, 0),
                NSLayoutConstraint.Create(flagsImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, flagsImageView, NSLayoutAttribute.Width, flagsImageAspectRatio, 0),

                NSLayoutConstraint.Create(textImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 1, -40),
                NSLayoutConstraint.Create(textImageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, topStripe, NSLayoutAttribute.Bottom, 1, 20),
                NSLayoutConstraint.Create(textImageView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Leading, 1, 20),
                NSLayoutConstraint.Create(textImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, textImageView, NSLayoutAttribute.Width, textImageAspectRatio, 0),

                NSLayoutConstraint.Create(bottomToolbar, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(bottomToolbar, NSLayoutAttribute.Height, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Height, 0, 64),

                NSLayoutConstraint.Create(arrowImageView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.Leading, 1, 40),
                NSLayoutConstraint.Create(arrowImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.Width, 0.30f, 0),
                NSLayoutConstraint.Create(arrowImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, arrowImageView, NSLayoutAttribute.Width, arrowImageAspectRatio, 0),
                NSLayoutConstraint.Create(arrowImageView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.Bottom, 1, -40)
            });

            return mainView;
        }

        void ShowNoProductsViewIfNeeded(int index)
        {
            if (index + 1 == _products.Count)
            {
                UIView.Animate(1, () =>
                {
                    CardView.Hidden = true;
                    _noProductsView.Hidden = false;
                });
            }
        }

        void OnSwipeLeft(object sender, SwipeEventArgs e)
        {
            int index = _viewMap.FirstOrDefault(v => v.Value == e.View).Key;
            if (ViewModel.ShowPunto)
            {
                _controllerMap[index].ShowPuntoImage();
            }
            ViewModel.VoteProduct(_products[index].Id, ProductVote.Dislike);
            ShowNoProductsViewIfNeeded(index);
        }

        void OnSwipeRight(object sender, SwipeEventArgs e)
        {
            int index = _viewMap.FirstOrDefault(v => v.Value == e.View).Key;
            if (ViewModel.ShowPunto)
            {
                _controllerMap[index].ShowPuntoImage();
            }
            ViewModel.VoteProduct(_products[index].Id, ProductVote.Like);
            ShowNoProductsViewIfNeeded(index);
        }

        void OnSwipeCancelled(object sender, SwipeEventArgs e)
        {
            _controllerMap[_viewMap.FirstOrDefault(v => v.Value == e.View).Key].RestartProduct();
        }

        void OnSwipeStarted(object sender, SwipingStartedEventArgs e)
        {
            _xValue = e.Location.X;
        }

        void OnSwiping(object sender, SwipingEventArgs e)
        {
            if (_xValue + 20 < e.Location.X)
            {
                _controllerMap[_viewMap.FirstOrDefault(v => v.Value == e.View).Key].LikeProduct();
                _xValue = e.Location.X;
            }
            else if (_xValue - 20 > e.Location.X)
            {
                _controllerMap[_viewMap.FirstOrDefault(v => v.Value == e.View).Key].DislikeProduct();
                _xValue = e.Location.X;
            }
        }

        void OnSwipeEnded(object sender, SwipingEndedEventArgs e)
        {

        }

        public CatalogueOfProductsViewController()
        {
            TransitioningDelegate = TransitionManager.Left;

            this.WhenActivated(dis =>
            {
                dis(ViewModel.WhenAnyValue(vm => vm.Products).Subscribe(products =>
                {
                    if (products != null)
                    {
                        if (products.Count == 0)
                        {
                            UIView.Animate(0.3, () =>
                            {
                                CardView.Hidden = true;
                                _noProductsView.Hidden = false;
                            });
                        }
                        else
                        {
                            _products = products;
                            _index = 0;
                            _viewMap.Clear();
                            _controllerMap.Clear();
                            CardView.DataSource = this;
                        }
                    }
                    else {
                        _noProductsView.Hidden = false;
                    }
                }));

                dis(ViewModel.WhenAnyValue(vm => vm.ShowPopup).Subscribe(show =>
                {
                    if (show)
                    {
                        InvokeOnMainThread(() =>
                        {
                            PresentViewController(new Tutorial.ProductCatalogTutorialViewController(onTutorialIscrivimiClicked: OnTutorialIscrivimiClicked, onDismiss: () =>
                            {
                                ViewModel.UpdateShowPopup();
                            }), true, null);
                        });
                    }
                    /*else if (ViewModel.ShowTutorial)
                    {
                        InvokeOnMainThread(() =>
                        {
                            PresentViewController(new Tutorial.ProductCatalogue.ProductCatalogueTutorial1ViewController(), true, null);
                        });
                    }*/
                }));

                ViewModel.GetProductList();
            });
        }
    }
}