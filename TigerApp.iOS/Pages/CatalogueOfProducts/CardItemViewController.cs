using Facebook.ShareKit;
using System;
using System.Threading.Tasks;
using TigerApp.Shared.Models;
using UIKit;
using Foundation;

namespace TigerApp.iOS.Pages.CatalogueOfProducts
{
    public partial class CardItemViewController : UIViewController
    {
        void TutorialButton_TouchUpInside(object sender, EventArgs e)
        {
            PresentViewController(new Tutorial.ProductCatalogue.ProductCatalogueTutorial1ViewController(), true, null);
        }

        private Product _product;

        public event EventHandler LikeDislikeTapped;
        public bool isLikeButtonClicked;
        public UIButton BackButton => backButton;

        public void ShowPuntoImage()
        {
            PuntoImage.Hidden = false;
        }

        public Product Product { get { return _product; } }

        public CardItemViewController(Product product) : base("CardItemViewController", null)
        {
            _product = product;
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            tutorialButton.TouchUpInside += TutorialButton_TouchUpInside;

            LikeButton.TouchUpInside += LikeButton_TouchUpInside;

            LikeButton.SetBackgroundImage(UIImage.FromBundle("catalogue_02_button"), UIControlState.Normal);
            LikeButton.SetBackgroundImage(UIImage.FromBundle("catalogue_06_button"), UIControlState.Selected);
            DislikeButton.TouchUpInside += DislikeButton_TouchUpInside;
            DislikeButton.SetBackgroundImage(UIImage.FromBundle("catalogue_01_button"), UIControlState.Normal);
            DislikeButton.SetBackgroundImage(UIImage.FromBundle("catalogue_07_button"), UIControlState.Selected);

            facebookShare.TouchUpInside += delegate
            {
                ShareLinkContent shareContent = new ShareLinkContent();
                shareContent.SetContentUrl(new Foundation.NSUrl(_product.SiteLink));
                ShareDialog.Show(this, shareContent, new ProductSharingDelegate(_product.Id.ToString()));
            };

            ProductTitleLabel.Text = _product.Name;
            ProductPriceLabel.Text = _product.Price.ToString() + " €";

            try
            {
                ImageUrl.Image = await LoadImage(_product.Image);
            }
            catch
            {
                var dialogProvider = AD.Resolver.Resolve<AD.IDialogProvider>();
                dialogProvider.DisplayError("Error while downloading image");
            }

            PuntoImage.Hidden = true;
        }

        async void LikeButton_TouchUpInside(object sender, EventArgs e)
        {
            isLikeButtonClicked = true;
            LikeProduct();
            await Task.Delay(200);
            LikeDislikeTapped?.Invoke(this, EventArgs.Empty);
        }

        public void LikeProduct()
        {
            LikeButton.Selected = true;
            DislikeButton.Selected = false;
        }

        async void DislikeButton_TouchUpInside(object sender, EventArgs e)
        {
            isLikeButtonClicked = false;
            DislikeProduct();
            await Task.Delay(200);
            LikeDislikeTapped?.Invoke(this, EventArgs.Empty);
        }

        public void DislikeProduct()
        {
            DislikeButton.Selected = true;
            LikeButton.Selected = false;
        }

        public void RestartProduct()
        {
            DislikeButton.Selected = false;
            LikeButton.Selected = false;

            PuntoImage.Hidden = true;
        }

        async Task<UIImage> LoadImage(string imageUrl)
        {
            var httpClient = new System.Net.Http.HttpClient();
            Task<byte[]> contentsTask = httpClient.GetByteArrayAsync(AD.Resolver.Resolve<AD.IHttpServerConfig>().BaseAddress + imageUrl);
            var contents = await contentsTask;
            return UIImage.LoadFromData(Foundation.NSData.FromArray(contents));
        }

        public class ProductSharingDelegate : NSObject, ISharingDelegate
        {
            private string _productId;

            public ProductSharingDelegate(string productId)
            {
                _productId = productId;
            }

            public void DidCancel(ISharing sharer)
            {
            }

            public void DidComplete(ISharing sharer, NSDictionary results)
            {
                AD.Resolver.Resolve<Shared.Services.API.ITrackedActionsApiService>().PushAction(new Shared.Models.TrackedActions.ShareFBTrackedAction(_productId), null)
                  .Subscribe(_ => { });
            }

            public void DidFail(ISharing sharer, NSError error)
            {
                Console.WriteLine(string.Format("[ ProductSharingDelegate ] : {0}", error.LocalizedDescription));
            }
        }
    }
}


