namespace WX.Utilities.WPFDesignerX.BusinessEditor
open System
open System.Collections.ObjectModel
open System.Xml.Linq
open System.Windows
open System.Windows.Data
open System.Windows.Controls
open System.Windows.Input
open Microsoft.Win32
open WX
open WX.Utilities.WPFDesignerX.Common

type FVM_FWXZ_Add()=
  inherit ViewModelBase()

  member this.Initialize()=
    this.Title<-"服务信息增加"
    this.D_FWAdd<-new ServiceInfo()
    this._D_CodeChangedTypeItem<-this.D_CodeChangedTypeItems.[0]

  [<DV>] val mutable private _D_FWAdd:ServiceInfo
  member this.D_FWAdd 
    with get ()=this._D_FWAdd
    and set v=
      if this._D_FWAdd<>v then
        this._D_FWAdd<-v
        this.OnPropertyChanged "D_FWAdd"

  [<DV>] val mutable private _D_CodeChangedTypeItem:KeyValue<ChangedType,string>
  member this.D_CodeChangedTypeItem 
    with get ()=this._D_CodeChangedTypeItem
    and set v=
      if this._D_CodeChangedTypeItem<>v then
        this._D_CodeChangedTypeItem<-v
        this.OnPropertyChanged "D_CodeChangedTypeItem"
        if v<>Null() then
          match this.D_FWAdd with
          | NotNull x->
              x.ServiceCodeChangedType<-v.Key
          | _ ->()

  [<DV>]val mutable private _D_CodeChangedTypeItems:ObservableCollection<KeyValue<ChangedType,string>>
  member  this.D_CodeChangedTypeItems
    with get ():ObservableCollection<KeyValue<_,_>>=
      if this._D_CodeChangedTypeItems=Null() then
        this._D_CodeChangedTypeItems<-
          match new ObservableCollection<KeyValue<_,_>>() with
          | x ->
              x.Add(new KeyValue<_,_>(ChangedType.Unchanged,"未变更"))
              x.Add(new KeyValue<_,_>(ChangedType.Added,"新增"))
              x.Add(new KeyValue<_,_>(ChangedType.Modified,"修改"))
              x
      this._D_CodeChangedTypeItems

  [<DV>]val mutable _CMD_Save:ICommand
  member this.CMD_Save
    with get ()=
      if this. _CMD_Save=null then
        this. _CMD_Save<-new RelayCommand(
          (fun _->
            match this.D_FWAdd with
            | NotNull x->
                match 
                  XmlData.XServiceInfos.Value.Elements()
                  |>Seq.tryFind(fun a->
                      match a.Attribute(XName.Get("ServiceName",XmlData.NS)) with
                      | NotNull y ->y.Value=x.ServiceName
                      | _ ->false
                      )
                  with
                | Some y ->
                    MessageBox.Show("服务名已存在，不能保存，请更改！","操作提示")|>ignore
                | _ ->
                    match new XElement(XName.Get("ServiceInfo",XmlData.NS)) with
                    |  y->
                        y.SetAttributeValue(XName.Get("ServiceName",XmlData.NS),x.ServiceName)
                        y.SetAttributeValue(XName.Get("ServiceDescription",XmlData.NS),x.ServiceDescription)
                        y.SetAttributeValue(XName.Get("ServiceCode",XmlData.NS),x.ServiceCode)
                        y.SetAttributeValue(XName.Get("ServiceCodeChangedType",XmlData.NS),int x.ServiceCodeChangedType|>string)
                        y.SetAttributeValue(XName.Get("ReferenceCount",XmlData.NS),x.ReferenceCount)
                        XmlData.XServiceInfos.Value.AddFirst y
                        if XmlData.SaveXDoc() then
                          this.CloseCommand.Execute x
            | _ ->()
            ),
          (fun _ ->
            match this.D_FWAdd with
            | NotNull x ->
              x.IsDirtyOfServiceName || x.IsDirtyOfServiceDescription ||
              x.IsDirtyOfServiceCode || x.IsDirtyOfServiceCodeChangedType
            | _ ->false
            ))
      this. _CMD_Save  