using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
namespace WX.Utilities.WPFDesignerX.BusinessEditor
{
  public class ConverterBoolToVisibility : IValueConverter
  {

    //Get
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is bool && (bool)value)
      {
        return Visibility.Visible;
      }
      return Visibility.Collapsed;
    }

    //Set
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
