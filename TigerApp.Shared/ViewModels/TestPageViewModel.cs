using System;
using System.Collections.Generic;
using System.Text;

namespace TigerApp.Shared.ViewModels
{
    /* 
     * This is Model for TestActivity only for development and testing purposes 
     */
    public interface ITestPageViewModel : IViewModelBase
    {

    }

    public class TestPageViewModel : ReactiveViewModel, ITestPageViewModel
    {

    }
}
