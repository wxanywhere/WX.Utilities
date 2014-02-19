namespace WX.Utilities.Excel.Model
open System
open System.Collections.ObjectModel
open WX.Utilities.Common

type SheetColumnInfo()=
  inherit ModelBase()
  [<DV>]val mutable private _ColumnHeaderName:string
  member this.ColumnHeaderName
    with get ()=this._ColumnHeaderName
    and set v=
      if this._ColumnHeaderName<>v then
        this._ColumnHeaderName<-v
        this.OnPropertyChanged "ColumnHeaderName"

  [<DV>]val mutable private _ColumnNum:int
  member this.ColumnNum
    with get ()=this._ColumnNum
    and set v=
      if this._ColumnNum<>v then
        this._ColumnNum<-v
        this.OnPropertyChanged "ColumnNum"

  [<DV>]val mutable private _ColumnLevel:int
  member this.ColumnLevel
    with get ()=this._ColumnLevel
    and set v=
      if this._ColumnLevel<>v then
        this._ColumnLevel<-v
        this.OnPropertyChanged "ColumnLevel"

  [<DV>]
  val mutable private _ColumnLevelItems:ObservableCollection<KeyValue<int,string>>
  member x.ColumnLevelItems
    with get ()=
      if x._ColumnLevelItems=Null() then
        x._ColumnLevelItems<-new ObservableCollection<KeyValue<int,string>>()
      x._ColumnLevelItems

  [<DV>]
  val mutable private _ColumnLevelSelectedItem:KeyValue<int,string>
  member x.ColumnLevelSelectedItem 
    with get ()=x._ColumnLevelSelectedItem 
    and set v=
      if  x._ColumnLevelSelectedItem<>v  then
        x._ColumnLevelSelectedItem <- v
        x.OnPropertyChanged "ColumnLevelSelectedItem"
        if v<>Null() then
          x.ColumnLevel<-v.Key