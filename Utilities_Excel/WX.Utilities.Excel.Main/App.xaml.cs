using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WX.Utilities.Excel.Main
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
            base.OnStartup(e);

            var mainView = Application.LoadComponent(new Uri("/WX.Utilities.Excel.View;component/MainWindow.xaml", UriKind.Relative));
            if (mainView == null) return;
            var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "WX.Utilities.Excel.AppLogic");
            if (asm==null) return; 
            var mainViewModelType = asm.GetTypes().FirstOrDefault(a => a.Name == "FVM_MainWindow");
            if (mainViewModelType == null) return;
            var mainViewModel = Activator.CreateInstance(mainViewModelType);
            if (mainViewModel == null) return;
            if (mainView is Window)
            {
                ((Window)mainView).DataContext = mainViewModel;
                ((Window)mainView).Show();
            }

            /*正确，但需要额外引用依赖
            var mainView = new MainWindow();
            var mainViewModel = new FVM_MainWindow();
            mainView.DataContext = mainViewModel;
            mainView.Show();
            */
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
