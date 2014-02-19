using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WX.Utilities.Excel.View
{
    public partial class UC_ExcelDataExport : UserControl
    {
        public UC_ExcelDataExport()
        {
            InitializeComponent();
        }

        private void exportBtn_Click(object sender, RoutedEventArgs e)
        {
            exportMenu.IsOpen = true;
            exportMenu.DataContext = root.DataContext;
            BindingOperations.SetBinding(exportToExcel, MenuItem.CommandProperty, new Binding("CMD_ExportToExcel"));
            BindingOperations.SetBinding(exportToXml, MenuItem.CommandProperty, new Binding("CMD_ExportToXml"));
        }
    }
}
