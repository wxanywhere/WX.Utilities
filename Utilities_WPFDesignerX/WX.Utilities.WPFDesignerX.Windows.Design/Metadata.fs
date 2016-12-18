namespace WX.Utilities.WPFDesignerX.Windows.Design

open System
open System.IO
open System.Collections.Generic
open System.Text
open System.Xml.Linq
open System.ComponentModel
open System.Windows.Media
open System.Windows.Controls
open System.Windows
open Microsoft.Windows.Design.Features
open Microsoft.Windows.Design.Metadata
open Microsoft.Win32
open WX
open WX.Utilities.WPFDesignerX.Common
open WX.Utilities.WPFDesignerX.Windows

type internal Metadata()=
  interface IProvideAttributeTable with
    member this.AttributeTable
      with get():AttributeTable= 
        match new AttributeTableBuilder() with
        | x ->
            x.AddCustomAttributes(typeof<FrameworkElement>,new FeatureAttribute(typeof<AnnotationControllerAdornerProvider>))
            x.AddCustomAttributes(typeof<Window>,new FeatureAttribute(typeof<UITypeRootControllerAdornerProvider>))
            x.AddCustomAttributes(typeof<UserControl>,new FeatureAttribute(typeof<UITypeRootControllerAdornerProvider>))
            x.CreateTable()

(*
不同的创建形式
http://msdn.microsoft.com/en-us/library/bb907342(v=vs.90).aspx
*)