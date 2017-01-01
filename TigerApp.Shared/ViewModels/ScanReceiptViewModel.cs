using System;
using System.Text;
using System.Text.RegularExpressions;
using TigerApp.Shared.Services;
using TigerApp.Shared.Models;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TigerApp.Shared.Services.API;
using System.Threading;
using TigerApp.Shared.Models.TrackedActions;

namespace TigerApp.Shared.ViewModels
{
    public interface IScanReceiptViewModel : IViewModelBase
    {
        void GetReceiptData(byte[] imageBytes, bool onMainThread = false);
        ScanReceiptResult Result { get; }
        bool MissionCompleted { get; }
        void CompleteScanMission(ScanReceiptResult result, Action onReceiptAlreadyScanned);
        void SendWrongScan(ScanReceiptResult result, string encodedImage);
    }

    public class ScanReceiptViewModel : ReactiveViewModel, IScanReceiptViewModel
    {
        [Reactive]
        public bool MissionCompleted
        {
            get;
            protected set;
        }

        [Reactive]
        public ScanReceiptResult Result
        {
            get;
            protected set;
        } //= ScanReceiptResult.Empty;

        public void GetReceiptData(byte[] imageBytes,bool onMainThread = false)
        {
            if (onMainThread)
                Result = _extractResult(imageBytes);
            else
                new Thread(new ThreadStart(() => {
                    Result = _extractResult(imageBytes);
                })).Start();
        }

        private ScanReceiptResult _extractResult(byte [] imageBytes)
        {
            if (imageBytes == null)
                return ScanReceiptResult.Empty;
            var message = AD.Resolver.Resolve<IGoogleApiCloudVisionService>().TextDetection(imageBytes);
            if (message != null)
            {

                String TotaleValue = "";
                String OrginalMsg = message;
                int TransactionIndex = OrginalMsg.IndexOf("TRANSAZIONE");
                String tempMsg = "";
                String TransactionValue = "";

                try
                {
                    
                    TransactionIndex = OrginalMsg.IndexOf("#");
                    TransactionValue = OrginalMsg.Substring(TransactionIndex + 1, 6);

                    if (_isDigitsOnly(TransactionValue))
                    {
                        TransactionValue = TransactionValue.Replace("(\\r|\\n)", "");
                        TransactionValue = TransactionValue.Replace(System.Environment.NewLine, "");
                    }
                    else
                        return ScanReceiptResult.Empty;

                    if (TransactionValue.Contains(" ") || TransactionValue.Contains("\n"))
                    {
                        int OperatorIndex = OrginalMsg.IndexOf("OPERATORE");
                        TransactionIndex = OrginalMsg.IndexOf("TRANSAZIONE");
                        if (OperatorIndex > 0 && TransactionIndex > 0 && TransactionIndex < OperatorIndex)
                        {
                            TransactionValue = OrginalMsg.Substring(TransactionIndex + 13, OperatorIndex - TransactionIndex - 13);
                            //TransactionValue = TransactionValue.replaceAll("(\\r|\\n)", "");
                        }
                    }

                    if (TransactionValue.Contains("\n"))
                    {
                        return ScanReceiptResult.Empty;
                    }

                    int RegIndex = OrginalMsg.IndexOf("REG.");
                    if (RegIndex > 0)
                    {
                        OrginalMsg = OrginalMsg.Substring(0, RegIndex);
                    }

                    int Contante = OrginalMsg.IndexOf("CONTANTE");
                    int Resto = OrginalMsg.IndexOf("RESTO");

                    if (Resto > 0)
                    {
                        int lslash = OrginalMsg.LastIndexOf(",");
                        if (lslash > 0)
                            OrginalMsg = OrginalMsg.Substring(0, lslash);
                        lslash = OrginalMsg.LastIndexOf(",");
                        if (lslash > 0)
                            OrginalMsg = OrginalMsg.Substring(0, lslash);
                    }

                    Contante = OrginalMsg.IndexOf("CONTANTE");
                    Resto = OrginalMsg.IndexOf("RESTO");

                    if (Contante > 0)
                    {
                        if (Resto > 0)
                        {
                            String newTmp = OrginalMsg.Substring(Resto);
                            if (!newTmp.Contains(",")) // Rotated
                            {
                                OrginalMsg = OrginalMsg.Substring(0, Contante);
                                int lslash = OrginalMsg.LastIndexOf("\n");
                                if (lslash > 0)
                                    OrginalMsg = OrginalMsg.Substring(0, lslash);
                            }
                            else {
                                OrginalMsg = OrginalMsg.Substring(0, Contante);
                            }
                        }
                        else OrginalMsg = OrginalMsg.Substring(0, Contante);

                    }
                    int lastIndexOfVirgula = OrginalMsg.LastIndexOf(",");
                    if (lastIndexOfVirgula > 0)
                        tempMsg = OrginalMsg.Substring(0, lastIndexOfVirgula + 3);
                    int LastSlashN = tempMsg.LastIndexOf("\n");
                    if (LastSlashN > 0)
                        tempMsg = tempMsg.Substring(LastSlashN);
                    TotaleValue = tempMsg.Replace(System.Environment.NewLine, "");

                    if (TotaleValue == "")
                    {
                        int opt = message.IndexOf("OPERATORE");
                        int con = message.IndexOf("CONTANTE");
                        if (opt > 0 && con > 0)
                        {
                            TotaleValue = message.Substring(opt + 10, con - 1 - opt - 10);
                            int totV1 = TotaleValue.IndexOf(".00");
                            int totV2 = TotaleValue.IndexOf(",00");
                            if (totV1 > 0)
                            {
                                TotaleValue = TotaleValue.Substring(0, totV1 + 3);
                                int lsh = TotaleValue.LastIndexOf("\n");

                                if (lsh > 0)
                                    TotaleValue = TotaleValue.Substring(lsh + 1);
                                int sp = TotaleValue.LastIndexOf(" ");
                                if (sp > 0)
                                    TotaleValue = TotaleValue.Substring(sp + 1);
                                TotaleValue = TotaleValue.Replace(".", ",");
                            }
                            else if (totV2 > 0)
                            {
                                TotaleValue = TotaleValue.Substring(0, totV2 + 3);
                                int lsh = TotaleValue.LastIndexOf("\n");

                                if (lsh > 0)
                                    TotaleValue = TotaleValue.Substring(lsh + 1);
                                int sp = TotaleValue.LastIndexOf(" ");
                                if (sp > 0)
                                    TotaleValue = TotaleValue.Substring(sp + 1);
                            }
                        }
                    }

                    if (!Regex.IsMatch(TransactionValue, "[0-9,.]*") || !Regex.IsMatch(TotaleValue, "[0-9,.]*") || TotaleValue == "" || TotaleValue.Contains(" "))
                    {
                        return ScanReceiptResult.Empty;
                    }


                    TotaleValue = TotaleValue.Replace(System.Environment.NewLine, "");
                    int count = 0;
                    if (TotaleValue.Equals("1,00"))
                    {
                        int frstPrice = message.IndexOf(",");
                        if (frstPrice > 0)
                        {
                            String c1 = message.Substring(frstPrice - 1, 4);
                            if (c1 == "1,00")
                            {
                                String str = message;
                                String findStr = ",";
                                int lastIndex = 0;
                                count = 0;

                                while (lastIndex != -1)
                                {

                                    lastIndex = str.IndexOf(findStr, lastIndex);

                                    if (lastIndex != -1)
                                    {

                                        String c2 = str.Substring(lastIndex - 1, 4);
                                        if (c1 == "1,00")
                                        {
                                            count++;
                                            lastIndex += findStr.Length;
                                        }
                                    }
                                }

                            }
                            else { count = 7; }
                        }

                    }

                    if (count > 4)
                    {
                        TotaleValue = "7,00";
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return ScanReceiptResult.Empty;
                }

                TotaleValue = TotaleValue.Replace("(\\r|\\n)", "");
                TotaleValue = TotaleValue.Replace(System.Environment.NewLine, "");
                long receiptId;
                float amount;
                if(Int64.TryParse(TransactionValue, out receiptId) && float.TryParse(TotaleValue,out amount))
                    return new ScanReceiptResult() { 
                        ReceiptId = receiptId,
                        Amount = amount
                    };
            }

            return ScanReceiptResult.Empty;
        }

        private bool _isDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    if (c != '\n')
                        return false;
            }
            return true;
        }

        public void CompleteScanMission(ScanReceiptResult result, Action onReceiptAlreadyScanned) {
            var sendReceiptDataCommand = AD.Resolver.Resolve<IScanReceiptApiService>().SendReceiptData(result, onReceiptAlreadyScanned, AD.Resolver.Resolve<AD.ITDesAuthService>().Logout);
            sendReceiptDataCommand.SubscribeOnce(_ => { 
                MissionCompleted = true; 
            });
        }

        public void SendWrongScan(ScanReceiptResult result, string encodedImage) {
            AD.Resolver.Resolve<ITrackedActionsApiService>().PushAction(new FailScanReceiptTrackedAction(result.ReceiptId, result.Amount, encodedImage), null).SubscribeOnce(
                _ => 
            {
                var i = 9;
            });
        }
    }
}
