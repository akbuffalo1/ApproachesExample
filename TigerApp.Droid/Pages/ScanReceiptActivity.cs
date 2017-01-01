#region using

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using HockeyApp.Android;
using ReactiveUI;
using TigerApp.Droid.UI.DeviceCamera;
using TigerApp.Droid.Utils;
using TigerApp.Shared.Models;
using TigerApp.Shared.ViewModels;

#endregion

namespace TigerApp.Droid.Pages
{
    [Activity]
    public class ScanReceiptActivity : BaseReactiveActivity<IScanReceiptViewModel>
    {
        const string TAG = nameof(ScanReceiptActivity);
        const string ImagesFolderName = "TigerReceipts";
        private Button _btnScan;
        private ICameraPreview _cameraPreviewContainer;
        private ICameraDevice _cameraDevice;
        private Dialog _progressDialog;
        private Bitmap _lastScanImage;

        public ScanReceiptActivity()
        {
            this.WhenActivated(dispose => {
                dispose(ViewModel.WhenAnyValue(vm => vm.Result).Where((ScanReceiptResult res)=>res!=null).Subscribe((scanResult) =>
                {
                    this.RunOnUiThread(() => { 
                        if (!scanResult.Equals(ScanReceiptResult.Empty))
                            ShowResults(scanResult);
                        else {
                            ShowError();
                        }
                    });
                }));
                dispose(ViewModel.WhenAnyValue(vm => vm.MissionCompleted).Where((bool isCompleted) => isCompleted ).Subscribe((isCompleted) =>
                    {
                        Finish();
                    }));
            });
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_scan_receipt);

            _btnScan = FindViewById<Button>(Resource.Id.btnScan);
            _cameraPreviewContainer = (ICameraPreview)FindViewById<View>(Resource.Id.cameraPreview);

            _btnScan.Click += TakePhoto;

            if (Build.VERSION.SdkInt > BuildVersionCodes.LollipopMr1)
                _cameraDevice = new Camera2Device(this, _cameraPreviewContainer, false);
            else
                _cameraDevice = new CameraDeviceDeprecated(this, _cameraPreviewContainer, false);

            _cameraDevice.OnPictureTakenEvent += OnPictureTaken;

            FeedbackManager.Register(this);
        }

        private void OnPictureTaken(byte[] data)
        {
            if (data != null)
            {
                _lastScanImage = BitmapFactory.DecodeByteArray(data, 0, data.Length);
                ViewModel.GetReceiptData(data);

            }
            else
            {
                Toast.MakeText(this, "Something went wrong while taking this photo", ToastLength.Long).Show();
            }

        }

        protected override void OnResume()
        {
            base.OnResume();

            _cameraDevice.StartCamera();
        }

        protected override void OnPause()
        {
            base.OnPause();

            _cameraDevice.StopCamera();
        }

        private void ShowResults(ScanReceiptResult result)
        {
            AlertDialog alertDialog = null;
            var editableView = LayoutInflater.Inflate(Resource.Layout.dialog_scan_receipt, null);
            var amountEditText = editableView.FindViewById<EditText>(Resource.Id.amountEditText);
            var transactionIdEditText = editableView.FindViewById<EditText>(Resource.Id.transactionEditText);
            amountEditText.Text = result.Amount.ToString();
            transactionIdEditText.Text = result.ReceiptId.ToString();
            #if DEBUG
            amountEditText.Enabled = true;
            transactionIdEditText.Enabled = true;
            #endif
            var alert = new AlertDialog.Builder(this);
            alert.SetTitle("Verifica i dati della scansione");
            alert.SetView(editableView);
            alert.SetPositiveButton("Conferma", (sender, e) => { 
                if(alertDialog != null)
                    alertDialog.Dismiss();
                float amount = 0;
                long transID = 0;
                if (long.TryParse(transactionIdEditText.Text, out transID) && float.TryParse(amountEditText.Text, out amount)){
                    ViewModel.CompleteScanMission(new ScanReceiptResult()
                    {
                        ReceiptId = transID,
                        Amount = amount
                    }, () => { 
                        var errorAlert = new AlertDialog.Builder(this);
                        errorAlert.SetMessage("Questo scontrino è già stato usato!");
                        errorAlert.SetPositiveButton("OK", (s, args) =>
                        {
                            Finish();                        
                        });
                        errorAlert.Show();
                    });
                    //TODO bind with ViewModel.MissionCompleted
                    //Finish();
                }
            });
            alert.SetNegativeButton("Ripeti Scansione", (sender, e) => { 
                if (alertDialog != null)
                    alertDialog.Dismiss();
                float amount = 0;
                long transID = 0;
                //TODO remove
                if (long.TryParse(transactionIdEditText.Text, out transID) && float.TryParse(amountEditText.Text, out amount))
                {
                    _sendWrongScan(amount, transID);
                }
                //
                _btnScan.Enabled = true;
                _progressDialog.Dismiss();
                _cameraDevice.StartCamera();
            });
            alertDialog = alert.Show();
        }

        private void _sendWrongScan(float amount, long transID)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                Matrix rotationMatrix = new Matrix();
                rotationMatrix.PostRotate(90);
                var lastRotatedScan = Bitmap.CreateBitmap(_lastScanImage, 0, 0, _lastScanImage.Width, _lastScanImage.Height, rotationMatrix, true);
                lastRotatedScan.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);

                var bytes = stream.ToArray();
                var encodedImage = Convert.ToBase64String(bytes);
                ViewModel.SendWrongScan(new ScanReceiptResult()
                {
                    ReceiptId = transID,
                    Amount = amount
                }, encodedImage);
            }
        }

        private void ShowError()
        {
            AlertDialog alertDialog = null;
            var alert = new AlertDialog.Builder(this);
            alert.SetTitle("Scansione Fallita!");
            alert.SetMessage(Shared.Constants.Strings.ScanReceiptErrorMessage);
            alert.SetPositiveButton("Ripeti Scansione", (sender, e) =>
            {
                if (alertDialog != null)
                    alertDialog.Dismiss();
                _btnScan.Enabled = true;
                _progressDialog.Dismiss();
                _cameraDevice.StartCamera();
                _sendWrongScan(-1, -1);
            });
            alert.SetNegativeButton("Segnala problema", (sender, e) =>
            {
                if (alertDialog != null)
                    alertDialog.Dismiss();
                _btnScan.Enabled = true;
                _progressDialog.Dismiss();
                _cameraDevice.StartCamera();
                StartActivity(typeof(SettingsFeedbackActivity));
                Finish();
            });
            alertDialog = alert.Show();
        }

        private void TakePhoto(object sender, EventArgs e)
        {
            _progressDialog = this.ShowProgress(true);
            _btnScan.Enabled = false;
            //_cameraDevice.AutoFocus();
            _cameraDevice.TakePicture();
        }
    }
}