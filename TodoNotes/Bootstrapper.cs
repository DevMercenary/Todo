using Prism.DryIoc;
using Prism.Ioc;

using System.Windows;
using HandyControl.Tools;
using Prism.Modularity;
using TodoNotes.Views;
using TodoNotes.Views.Window;

namespace TodoNotes
{
    public class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {

            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return base.CreateModuleCatalog();

        }
    }
}
