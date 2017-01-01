using System;
using System.Collections.Generic;
using System.Text;

namespace TigerApp.Shared.ViewModels
{
    public interface IScanMissionViewModel : IViewModelBase
    {
    }

    public class ScanMissionViewModel : ReactiveViewModel, IScanMissionViewModel
    {
    }
}
