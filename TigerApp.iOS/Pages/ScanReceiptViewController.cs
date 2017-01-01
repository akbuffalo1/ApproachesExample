using AVFoundation;
using Foundation;
using System;
using System.Threading.Tasks;
using TigerApp.iOS.Utils;
using TigerApp.Shared.ViewModels;
using UIKit;
using ReactiveUI;
using System.Reactive.Linq;
using TigerApp.Shared.Models;

namespace TigerApp.iOS.Pages
{
    [Register("ScanReceiptViewController")]
    public class ScanReceiptViewController : BaseReactiveViewController<IScanReceiptViewModel>
    {
        void BackButton_TouchUpInside(object sender, EventArgs e)
        {
            DismissViewController(true, null);
        }

        private AVCaptureSession _captureSession;
        private AVCaptureDeviceInput _captureDeviceInput;
        private AVCaptureStillImageOutput _stillImageOutput;
        private AVCaptureVideoPreviewLayer _videoPreviewLayer;

        private UIView _previewView;
        private ProgressView _progressView;

        public ScanReceiptViewController()
        {
            this.WhenActivated(dispose =>
            {
                dispose(ViewModel.WhenAnyValue(vm => vm.Result).Where((ScanReceiptResult res) => res != null).Subscribe((scanResult) =>
                {
                    if (!scanResult.Equals(ScanReceiptResult.Empty))
                        ShowResults(scanResult);
                    else {
                        ShowError();
                    }
                }));
                dispose(ViewModel.WhenAnyValue(vm => vm.MissionCompleted).Where(complete => complete).Subscribe((missionCompleted) =>
                {
                    DismissViewController(true, null);
                }));
            });
        }

        protected override async void LayoutViews()
        {
            View.BackgroundColor = UIColor.White;

            var backButton = UICommon.CreateButton("", "BackButton");

            var mainStack = UICommon.CreateStackView(alignment: UIStackViewAlignment.Center, spacing: 20);

            var logoImage = UIImage.FromBundle("FlyingTigerLogo");
            nfloat logoImageAspectratio = logoImage.Size.Height / logoImage.Size.Width;

            var logoImageView = new UIImageView();
            logoImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            logoImageView.Image = logoImage;
            logoImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            var topMessage = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(30), UIColor.Black, UITextAlignment.Center);
            topMessage.Text = "Fai la scansione del\ntuo scontrino";
            topMessage.ApplyTigerFontDefaultAttributes(Fonts.TigerBasic.WithSize(30));

            var bottomMessage = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(30), UIColor.Black, UITextAlignment.Center);
            bottomMessage.Text = "Inquadra il TOTALE";

            _previewView = new UIView();

            var captureCommand = UICommon.CreateButton("Fai lo scan");
            captureCommand.TouchUpInside += async delegate
            {
                captureCommand.Enabled = false;
                _progressView.Hidden = false;
                var videoConnection = _stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
                var sampleBuffer = await _stillImageOutput.CaptureStillImageTaskAsync(videoConnection);

                var jpegImageAsNsData = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer);

                var img = UIImage.LoadFromData(jpegImageAsNsData);

                var jpegAsByteArray = jpegImageAsNsData.ToArray();

                _progressView.PhotoView.Image = img;

                ViewModel.GetReceiptData(jpegAsByteArray,true);
                _progressView.Hidden = true;
                captureCommand.Enabled = true;
            };

            mainStack.AddArrangedSubview(logoImageView);
            mainStack.AddArrangedSubview(topMessage);
            mainStack.AddArrangedSubview(new UIView());
            mainStack.AddArrangedSubview(bottomMessage);
            mainStack.AddArrangedSubview(captureCommand);

            _progressView = new ProgressView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Hidden = true
            };

            View.AddSubview(_previewView);
            View.AddSubview(mainStack);
            View.AddSubview(_progressView);

            View.Add(backButton);

            backButton.TouchUpInside += BackButton_TouchUpInside;

            backButton.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 8).Active = true;
            backButton.TopAnchor.ConstraintEqualTo(logoImageView.TopAnchor).Active = true;
            backButton.WidthAnchor.ConstraintEqualTo(41).Active = true;
            backButton.HeightAnchor.ConstraintEqualTo(27).Active = true;

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),

                NSLayoutConstraint.Create(logoImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 0.5f, 0),
                NSLayoutConstraint.Create(logoImageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Top, 1, 30),
                NSLayoutConstraint.Create(logoImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, logoImageView, NSLayoutAttribute.Width, logoImageAspectratio, 0),

                NSLayoutConstraint.Create(captureCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 1, -20),
                NSLayoutConstraint.Create(captureCommand, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Bottom, 1, -10),

                NSLayoutConstraint.Create(_previewView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(_previewView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(_previewView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(_previewView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),

                NSLayoutConstraint.Create(_progressView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(_progressView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(_progressView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(_progressView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0)
            });

            await AuthorizeCameraUse();
            SetupLiveCameraStream();
        }

        async Task AuthorizeCameraUse()
        {
            var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);

            if (authorizationStatus != AVAuthorizationStatus.Authorized)
            {
                await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
            }
        }

        public void SetupLiveCameraStream()
        {
            _captureSession = new AVCaptureSession();

            var viewLayer = _previewView.Layer;
            _videoPreviewLayer = new AVCaptureVideoPreviewLayer(_captureSession)
            {
                Frame = this.View.Frame
            };
            _previewView.Layer.AddSublayer(_videoPreviewLayer);

            var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
            ConfigureCameraForDevice(captureDevice);

            if (captureDevice != null)
            {
                _captureDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice);
                _captureSession.AddInput(_captureDeviceInput);

                var dictionary = new NSMutableDictionary();
                dictionary[AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG);
                _stillImageOutput = new AVCaptureStillImageOutput()
                {
                    OutputSettings = new NSDictionary()
                };

                _captureSession.AddOutput(_stillImageOutput);
                _captureSession.StartRunning();
            }
        }

        void ConfigureCameraForDevice(AVCaptureDevice device)
        {
            var error = new NSError();

            if (device == null)
                return;

            if (device.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
            {
                device.LockForConfiguration(out error);
                device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
                device.UnlockForConfiguration();
            }
            else if (device.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
            {
                device.LockForConfiguration(out error);
                device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
                device.UnlockForConfiguration();
            }
            else if (device.IsWhiteBalanceModeSupported(AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance))
            {
                device.LockForConfiguration(out error);
                device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
                device.UnlockForConfiguration();
            }
        }

        public void ShowResults(ScanReceiptResult scanResult) {
            var alert = new UIAlertView();
            alert.Title = "Verifica i dati della scansione";
            alert.Message = string.Format("ID Transazione : {0}\nTotale : {1}",scanResult.ReceiptId,scanResult.Amount);
            alert.AddButton("Conferma");
            alert.AddButton("Ripeti la scansione");
            alert.Clicked += (sender, buttonArgs) => {
                if (buttonArgs.ButtonIndex == 0)
                {
                    ViewModel.CompleteScanMission(scanResult);
                    //TODO bind with ViewModel.MissionCompleted
                    //DismissViewController(true, null);
                }
            };
            alert.DismissWithClickedButtonIndex(1,true);
            alert.Show();
        }

        private bool _sendFeedback = false;
        public void ShowError() { 
            var alert = new UIAlertView();
            alert.Title = "Scansione Fallita!";
            alert.Message = Shared.Constants.Strings.ScanReceiptErrorMessage;
            alert.AddButton("Ripeti scansione");
            alert.AddButton("Invia un feedback");
            alert.Clicked += (sender, buttonArgs) =>
            {
                if (buttonArgs.ButtonIndex == 1)
                {
                    _sendFeedback = true;
                }
            };
            alert.DismissWithClickedButtonIndex(0, true);
            alert.DismissWithClickedButtonIndex(1, true);
            alert.Dismissed += (s, e) =>
            {
                if (_sendFeedback) { 
                    var feedbackManager = HockeyApp.iOS.BITHockeyManager.SharedHockeyManager.FeedbackManager;
                    feedbackManager.ShowFeedbackComposeViewWithGeneratedScreenshot();
                    _sendFeedback = false;
                }
            };
            alert.Show();
        }

        public class ProgressView : UIView
        {
            private UIActivityIndicatorView _spinner;

            private UIImageView _photoView;
            public UIImageView PhotoView { get { return _photoView; } }

            public ProgressView()
            {
                var overlay = new UIView
                {
                    BackgroundColor = Colors.SemiTransparentBlack,
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                _spinner = new UIActivityIndicatorView
                {
                    ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge,
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                _photoView = new UIImageView { TranslatesAutoresizingMaskIntoConstraints = false };

                AddSubview(_photoView);
                AddSubview(overlay);
                AddSubview(_spinner);

                AddConstraints(new NSLayoutConstraint[]
                {
                    NSLayoutConstraint.Create(_photoView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1, 0),
                    NSLayoutConstraint.Create(_photoView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, this, NSLayoutAttribute.Leading, 1, 0),
                    NSLayoutConstraint.Create(_photoView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this, NSLayoutAttribute.Width, 1, 0),
                    NSLayoutConstraint.Create(_photoView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, this, NSLayoutAttribute.Height, 1, 0),

                    NSLayoutConstraint.Create(overlay, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1, 0),
                    NSLayoutConstraint.Create(overlay, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, this, NSLayoutAttribute.Leading, 1, 0),
                    NSLayoutConstraint.Create(overlay, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this, NSLayoutAttribute.Width, 1, 0),
                    NSLayoutConstraint.Create(overlay, NSLayoutAttribute.Height, NSLayoutRelation.Equal, this, NSLayoutAttribute.Height, 1, 0),

                    NSLayoutConstraint.Create(_spinner, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterX, 1, 0),
                    NSLayoutConstraint.Create(_spinner, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1, 0)
                });
            }

            public override bool Hidden
            {
                get
                {
                    return base.Hidden;
                }

                set
                {
                    base.Hidden = value;
                    if (value)
                    {
                        _spinner.StopAnimating();
                    }
                    else
                    {
                        _spinner.StartAnimating();
                    }
                }
            }
        }
    }
}