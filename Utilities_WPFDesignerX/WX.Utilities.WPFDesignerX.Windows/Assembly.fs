namespace WX.Utilities.WPFDesignerX.Windows
open System.Windows.Markup
open System.Runtime.InteropServices

(*
[<assembly: XmlnsPrefix("http://schemas.WX.com/xaml", "tfx")>]
[<assembly: XmlnsDefinition("http://schemas.WX.com/xaml", "WX.Utilities.WPFDesignerX.Windows")>]
*)

[<assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "WX.Utilities.WPFDesignerX.Windows")>]
[<assembly: XmlnsCompatibleWithAttribute("http://schemas.microsoft.com/winfx/2007/xaml/presentation", "WX.Utilities.WPFDesignerX.Windows")>]
[<assembly: XmlnsCompatibleWithAttribute("http://schemas.microsoft.com/winfx/2008/xaml/presentation", "WX.Utilities.WPFDesignerX.Windows")>]
do()

//=================================
(*
http://www.codeproject.com/Articles/111911/A-Guide-to-Cleaner-XAML-with-Custom-Namespaces-and
*)