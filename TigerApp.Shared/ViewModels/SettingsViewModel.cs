using System;
using System.Collections.Generic;
using System.Text;

namespace TigerApp.Shared.ViewModels
{
    public interface ISettingsViewModel : IViewModelBase
    {

    }

    public class SettingsViewModel : ReactiveViewModel, ISettingsViewModel
    {
    }
}
