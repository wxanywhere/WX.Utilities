namespace WX.Utilities.WPFDesignerX.BusinessEditor
open System
open System.Windows
open System.Windows.Data
open System.Windows.Controls
open System.Windows.Input
open WX

type FVM_FWXX()=
  inherit ViewModelBase()

  member this.Initialize (entity:ServiceInfo)=
    this.Title<-"服务信息"
    this.D_FWXX<-entity

  [<DV>] val mutable private _D_FWXX:ServiceInfo
  member this.D_FWXX 
    with get ()=this._D_FWXX
    and set v=
      if this._D_FWXX<>v then
        this._D_FWXX<-v
        this.OnPropertyChanged "D_FWXX"
