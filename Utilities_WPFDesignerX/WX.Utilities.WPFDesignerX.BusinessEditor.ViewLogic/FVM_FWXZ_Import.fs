namespace WX.Utilities.WPFDesignerX.BusinessEditor
open System
open System.Windows
open System.Windows.Data
open System.Windows.Controls
open System.Windows.Input
open WX

type FVM_FWXZ_Import()=
  inherit ViewModelBase()

  member this.Initialize()=
    this.Title<-"服务信息导入"