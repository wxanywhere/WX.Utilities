namespace WX.Utilities.WPFDesignerX.Windows.Design
open System
open System.Windows.Media
open Microsoft.Windows.Design.Metadata
open WX.Utilities.WPFDesignerX.Windows

[<AutoOpen>]
module TypeExtension=
  type Extension with
    static member TypeId=new TypeIdentifier("WX.Utilities.WPFDesignerX.Windows.Extension")
    static member IDPropertyIdentifier= new PropertyIdentifier(Extension.TypeId, "ID")
    static member IsUITypeRootPropertyIdentifier= new PropertyIdentifier(Extension.TypeId, "IsUITypeRoot")

  type Color with
    static member GetInvertColor(color:Color)=
      Color.FromArgb(color.A,~~~color.R,~~~color.G,~~~color.B)

//===========================
(*
//正确参考，但xmlNamespace局限较大
static member TypeId=new TypeIdentifier("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Extension")
*)