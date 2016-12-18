using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using WX.Utilities.WPFDesignerX.Windows;

namespace WX.Utilities.WPFDesignerX.TestApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is InvalidOperationException)
                e.Handled = true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected override void OnStartup(StartupEventArgs e)
        {
            /*
            CommonDataModule.UIAssembly = new Microsoft.FSharp.Core.FSharpOption<Assembly>(this.GetType().Assembly);
            CommonDataModule.IDProperty = new Microsoft.FSharp.Core.FSharpOption<DependencyProperty>(Extension.IDProperty);
            var v = new WX.Utilities.WPFDesignerX.BusinessEditor.MainWindow();
            var vm = new FVM_MainWindow();
            vm.Initialize();
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.Show();
            */
            base.OnStartup(e);

        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
