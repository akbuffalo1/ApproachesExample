#region using

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AD;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using ReactiveUI;
using TigerApp.Droid.Services.Platform;
using TigerApp.Droid.Tutorial.SlidingTutorial;
using TigerApp.Droid.UI;
using TigerApp.Droid.UI.SwipeCards;
using TigerApp.Droid.Utils;
using TigerApp.Shared;
using TigerApp.Shared.Models;
using TigerApp.Shared.Services.API;
using TigerApp.Shared.ViewModels;
using Xamarin.Facebook;
using Xamarin.Facebook.AppEvents;
using Xamarin.Facebook.Share;
using Xamarin.Facebook.Share.Model;
using Xamarin.Facebook.Share.Widget;

#endregion

namespace TigerApp.Droid.Pages
{
    [Activity]
    public class ProductsCatalogueActivity : BaseReactiveActivity<IProductsCatalogueViewModel>,
        ViewPager.IOnPageChangeListener
    {
        static string TAG = nameof(ProductsCatalogueActivity);

        private CardStack _cardStack;
        private Dialog _dialog;
        private List<Product> _products;
        private ProgressDialog _progressDialog;
        private ProductCardsAdapter _cardAdapter;
        private FacebookCallback<SharerResult> _shareCallback;
        private ShareDialog _shareDialog;
        private RelativeLayout _layoutAllVoted;
        private ImageButton _btnBack;
        private int _votedProducts = 0;
        private string _productSharedId;
        private ViewPager _pager;
        private ImageButton _btnInfo;
        private TestFragmentAdapter _tutorialAdapter;

        private ICallbackManager callbackManager { get; set; }

        public ProductsCatalogueActivity()
        {
            this.WhenActivated(dispose =>
            {
                dispose(ViewModel.WhenAnyValue(vm => vm.Products).Subscribe(products =>
                {
                    if (products != null)
                    {
                        _products = products;
                        _cardAdapter.Clear();
                        _cardAdapter.AddAll(_products);
                        _products.ForEach(product => Logger.Debug("Product", product.Image));
                        if (_products.Count == 0)
                        {
                            _layoutAllVoted.Visibility = ViewStates.Visible;
                            _cardStack.Visibility = ViewStates.Gone;
                        }
                    }
                }));

                dispose(ViewModel.WhenAnyValue(vm => vm.IsLoading).Subscribe(isLoading =>
                {
                    if (!isLoading)
                        _progressDialog?.Dismiss();
                    else
                        _progressDialog = this.ShowTransparentProgress();
                }));
                ShowTutorial();
                ViewModel.GetProductList();
            });
        }


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_products_catalogue);

            _cardStack = FindViewById<CardStack>(Resource.Id.container);
            _cardStack.ContentResource = Resource.Layout.product_card;
            _cardStack.CardEventListener = new ProductCard.CardSwipeListener(ScreenUtils.Dp2Px(this, 100), _cardStack);

            _cardAdapter = new ProductCardsAdapter(ApplicationContext, Resource.Layout.product_card);
            _cardAdapter.OnCardActionEvent += OnCardCardAction;
            _cardAdapter.OnCardSwipeActionEvent += OnCardSwipeActionEvent;
            _cardStack.Adapter = _cardAdapter;

            _layoutAllVoted = FindViewById<RelativeLayout>(Resource.Id.layoutAllVoted);
            _btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
            _btnBack.Click += (sender, args) => Finish();
            OnSwipeLeft += Finish;

            _pager = FindViewById<ViewPager>(Resource.Id.pager);
            _pager.Visibility = ViewStates.Gone;
            _pager.SetOnPageChangeListener(this);

            _btnInfo = FindViewById<ImageButton>(Resource.Id.btnInfo);
            _btnInfo.Click += (sender, args) => { ShowSlideTutorial(); };

            #region Popup

            _cardStack.OnCardSwiped += delegate { };

            #endregion

            FacebookSdk.SdkInitialize(ApplicationContext);
            AppEventsLogger.ActivateApp(Application);

            callbackManager = AD.Resolver.Resolve<ICallbackManager>();
            if (callbackManager == null)
            {
                var ioc = Resolver.Resolve<IDependencyContainer>();
                callbackManager = CallbackManagerFactory.Create();
                ioc.Register(callbackManager);
            }

            _shareCallback = new FacebookCallback<SharerResult>
            {
                HandleSuccess = shareResult =>
                {
                    AD.Resolver.Resolve<ITrackedActionsApiService>()
                        .PushAction(new Shared.Models.TrackedActions.ShareFBTrackedAction(_productSharedId), null)
                        .SubscribeOnce(_ => { });
                    DialogUtil.ShowAlert(this, Resources.GetString(Resource.String.msg_facebook_successfully_posted));
                },
                HandleCancel = () => { Logger.Debug(TAG, "HelloFacebook: Canceled"); },
                HandleError =
                    shareError =>
                    {
                        DialogUtil.ShowAlert(this, Resources.GetString(Resource.String.msg_facebook_post_error));
                    }
            };

            _shareDialog = new ShareDialog(this);
            _shareDialog.RegisterCallback(callbackManager, _shareCallback);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            callbackManager.OnActivityResult(requestCode, (int) resultCode, data);
        }

        private void OnCardCardAction(ProductVote action, Product product)
        {
            if (action == ProductVote.Ignore)
            {
                Resolver.Resolve<IProductApiService>()
                    .GetProductList(AD.Plugins.Network.Rest.Priority.Internet)
                    .SubscribeOnce(_ => { });
                Finish();
                return;
            }

            if (action == ProductVote.Share)
            {
                ShareProductOnFacebook(product);
                return;
            }

            VoteProduct(product.Id, action);
            ShowTutorial();
            Task.Delay(250).ContinueWith(o => { RunOnUiThread(() => _cardStack.DiscardTop((int) action, 400)); });
        }

        private void ShareProductOnFacebook(Product product)
        {
            if (!AD.Resolver.Resolve<Shared.Services.API.IProfileApiService>().IsLoggedIn)
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.msg_facebook_need_to_login), ToastLength.Long)
                    .Show();
                Finish();
                return;
            }

            var linkContent = new ShareLinkContent.Builder()
                .SetContentUrl(
                    Android.Net.Uri.Parse(product.SiteLink))
                .JavaCast<ShareLinkContent.Builder>()
                .Build();

            var canPresentShareDialog = ShareDialog.CanShow(Java.Lang.Class.FromType(typeof(ShareLinkContent)));

            _productSharedId = product.Id.ToString();
            if (canPresentShareDialog)
                _shareDialog.Show(linkContent);
            else
                DialogUtil.ShowAlert(this, "Cant share post");
        }

        private void OnCardSwipeActionEvent(ProductVote action, Product product)
        {
            if (action == ProductVote.Ignore)
            {
                Resolver.Resolve<IProductApiService>()
                    .GetProductList(AD.Plugins.Network.Rest.Priority.Internet)
                    .SubscribeOnce(_ => { });
                Finish();
                return;
            }

            VoteProduct(product.Id, action);
            ShowTutorial();
        }

        private void VoteProduct(long productId, ProductVote action)
        {
            ViewModel.VoteProduct(productId, action);
            _votedProducts++;
            if (_votedProducts == _products.Count)
            {
                _layoutAllVoted.Visibility = ViewStates.Visible;
                _cardStack.Visibility = ViewStates.Gone;
            }
        }


        private void ShowTutorial()
        {
            if (ViewModel.ShowPopup)
            {
                _dialog = new Dialog(this, Resource.Style.ImageTutorialStyle);
                _dialog.SetContentView(Resource.Layout.popup_product_catalogue);

                var popupButton1 = _dialog.FindViewById<ImageView>(Resource.Id.imgProductsCataloguePopup2);
                popupButton1.Click += delegate
                {
                    Resolver.Resolve<IFlagStoreService>().Set(Constants.Flags.PRODUCTS_CATALOGUE_POPUP_SHOWN);
                    ViewModel.UpdateShowPopup();
                    _dialog.Dismiss();
                };
                var popupButton2 = _dialog.FindViewById<ImageView>(Resource.Id.imgProductsCataloguePopup3);
                popupButton2.Click += delegate
                {
                    _dialog.Dismiss();
                    Finish();
                };

                _dialog.Show();

                _dialog.DismissEvent += (sender, args) =>
                {
                    var rootView = _dialog.FindViewById(Resource.Id.popupRootView);
                    PlatformUtil.UnbindDrawables(rootView);
                    GC.Collect();
                    Java.Lang.Runtime.GetRuntime().Gc();
                };
            }
            /*else { 
                if (ViewModel.ShowTutorial)
                {
                    ShowSlideTutorial();
                    AD.Resolver.Resolve<IFlagStoreService>().Set(Constants.Flags.PRODUCTS_CATALOGUE_TUTORIAL_SHOWN);
                }
            }*/
        }

        #region Slide tutorial

        private void ShowSlideTutorial()
        {
            _pager.Visibility = ViewStates.Visible;
            _tutorialAdapter = new TestFragmentAdapter(SupportFragmentManager);
            _pager.Adapter = _tutorialAdapter;

            _tutorialAdapter.OnFragmentNextClickEvent += OnNextTutorialSlide;
            _cardStack.Visibility = ViewStates.Gone;
            _layoutAllVoted.Visibility = ViewStates.Gone;
            _btnInfo.Visibility = ViewStates.Gone;

            _pager.SetCurrentItem(0, false);
        }

        private void HideSlideTutorial()
        {
            _pager.Visibility = ViewStates.Gone;
            _btnInfo.Visibility = ViewStates.Visible;
            if (_products.Count == 0)
            {
                _layoutAllVoted.Visibility = ViewStates.Visible;
                _cardStack.Visibility = ViewStates.Gone;
            }
            else
            {
                _layoutAllVoted.Visibility = ViewStates.Gone;
                _cardStack.Visibility = ViewStates.Visible;
            }

            //try to clear fragments an free memory
            var fragments = SupportFragmentManager.Fragments;
            if (fragments != null)
            {
                var ft = SupportFragmentManager.BeginTransaction();
                foreach (var f in fragments)
                {
                    if (f is TutorialFragment)
                    {
                        ft.Remove(f);
                    }
                }
                ft.CommitAllowingStateLoss();
            }

            GC.Collect();
            Java.Lang.Runtime.GetRuntime().Gc();
        }

        private void OnNextTutorialSlide()
        {
            _pager.SetCurrentItem(_pager.CurrentItem + 1, true);
        }

        #region ViewPager.IOnPageChangeListener

        public void OnPageScrollStateChanged(int state)
        {
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
        }

        public void OnPageSelected(int position)
        {
            if (position == _tutorialAdapter.Count-1)
                HideSlideTutorial();
        }

        #endregion

        #endregion
    }
}