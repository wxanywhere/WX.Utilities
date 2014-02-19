namespace WX.Utilities.Excel.Model
open System
open System.Collections.ObjectModel
open WX.Utilities.Common

type SheetInfo()=
  inherit ModelBase()
  [<DV>]val mutable private _SheetHeader:string
  member this.SheetHeader
    with get ()=this._SheetHeader
    and set v=
      if this._SheetHeader<>v then
        this._SheetHeader<-v
        this.OnPropertyChanged "SheetHeader"

  [<DV>]val mutable private _SheetNum:int
  member this.SheetNum
    with get ()=this._SheetNum
    and set v=
      if this._SheetNum<>v then
        this._SheetNum<-v
        this.OnPropertyChanged "SheetNum"

  [<DV>]val mutable private _NeedExportFlag:bool
  member this.NeedExportFlag
    with get ()=this._NeedExportFlag
    and set v=
      if this._NeedExportFlag<>v then
        this._NeedExportFlag<-v
        this.OnPropertyChanged "NeedExportFlag"

  [<DV>]
  val mutable private _SheetColumnInfoView:ObservableCollection<SheetColumnInfo>
  member x.SheetColumnInfoView
    with get ()=
      if x._SheetColumnInfoView=Null() then
        x._SheetColumnInfoView<-new ObservableCollection<SheetColumnInfo>()
      x._SheetColumnInfoView

  [<DV>]
  val mutable private _HeaderRowItems:ObservableCollection<KeyValue<int,string>>
  member x.HeaderRowItems
    with get ()=
      if x._HeaderRowItems=Null() then
        x._HeaderRowItems<-new ObservableCollection<KeyValue<int,string>>()
      x._HeaderRowItems

  [<DV>]
  val mutable private _HeaderRowSelectedItem:KeyValue<int,string>
  member x.HeaderRowSelectedItem 
    with get ()=x._HeaderRowSelectedItem 
    and set v=
      if  x._HeaderRowSelectedItem<>v  then
        x._HeaderRowSelectedItem <- v
        x.OnPropertyChanged "HeaderRowSelectedItem"