namespace WX.Utilities.WPFDesignerX.BusinessEditor
open System.Windows
open System.Windows.Data

[<AutoOpen>]
module ExtensionModule=
  type System.Windows.Window with
    member this.SetDialogContentBinding path=
      BindingOperations.SetBinding(this,Window.ContentProperty,new Binding(path))|>ignore
      this.SizeToContent<-SizeToContent.WidthAndHeight //Double.NaN设置'Auto'无效
      this.ResizeMode<-ResizeMode.NoResize
      this.Owner<-Application.Current.MainWindow
      this.WindowStartupLocation<-WindowStartupLocation.CenterOwner
    
(*
Right reference
this.SetBinding(Window.ContentProperty,new Binding(path))|>ignore

*)