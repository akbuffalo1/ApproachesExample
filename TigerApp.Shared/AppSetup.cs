using AD;

using Splat;

using ReactiveUI;
using System.Reactive.Linq;
using TigerApp.Shared.ViewModels;
using System.Reflection;
using AD.TypeExtensions;
using TigerApp.Shared.Utils;

namespace TigerApp
{
    public class AppSetup : ApplicationSetupBase, IEnableLogger
    {
        // ReactiveUI specific configuration
        private void ConfigureLogging()
        {
#if DEBUG
            LogHost.Default.Level = LogLevel.Debug;
#endif
        }

        public override void Setup(IDependencyContainer ioc)
        {
            /** 
                Register ViewModels so that they get injected into viewcontrollers automatically 
            **/
            Assembly.GetExecutingAssembly().CreatableTypes()
                .Inherits<ReactiveViewModel>()
                .AsInterfaces()
                .RegisterAsMultiInstance();

            /**
                UserError is ReactiveUI specific class
                that serves as a shared buffer of all exceptions (except native)
                thrown from within ReactiveObject/s. 
                
                Here, we're registering a handler for such exceptions, and use
                AD's IDialogProvider plugin to display errors as they come.
            **/
            UserError.RegisterHandler(err =>
            {
                var message = "";
                if (AD.Resolver.Resolve<INetworkReachability>().IsConnected)
                    message = err.ErrorMessage;//"Si è verificato un errore imprevisto sui nostri server!";
                else
                    message = "Non è stato possibile raggiungere i nostri server! Verfica la tua connessione internet";
                Resolver.Resolve<IDialogProvider>().DisplayError(message);
                return Observable.Return(RecoveryOptionResult.CancelOperation);
            });

            base.Setup(ioc);
        }
    }
}