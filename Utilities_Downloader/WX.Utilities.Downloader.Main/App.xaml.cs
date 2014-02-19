using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WX.Utilities.Downloader.View;
using WX.Utilities.Downloader.ViewModel;

namespace WX.Utilities.Downloader.Main
{
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
            base.OnStartup(e);

            var mainView = new MainWindow();
            var mainViewModel = new FVM_MainWindow();
            mainView.DataContext = mainViewModel;
            mainView.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
