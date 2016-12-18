namespace WX.Utilities.WPFDesignerX.BusinessEditor
open System
open System.Windows
open System.Windows.Data
open System.Windows.Controls
open System.Windows.Input
open WX

type FVM_FWGLXX()=
  inherit ViewModelBase()

  member this.Initialize()=
    this.Title<-"服务关联信息"