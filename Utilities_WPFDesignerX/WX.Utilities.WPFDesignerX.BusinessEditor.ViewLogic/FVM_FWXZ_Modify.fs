namespace WX.Utilities.WPFDesignerX.BusinessEditor
open System
open System.Collections.ObjectModel
open System.Xml.Linq
open System.Windows
open System.Windows.Data
open System.Windows.Controls
open System.Windows.Input
open WX
open WX.Utilities.WPFDesignerX.Common

type FVM_FWXZ_Modify()=
  inherit ViewModelBase()

  member this.Initialize (entityOriginal:ServiceInfo)=
    this.Title<-"服务信息修改"
    match entityOriginal with
    | NotNull x->
        this.D_FWOriginal<-x
        this.D_FWModify<-
          match new ServiceInfo() with
          | y ->
              y.ServiceName<-x.ServiceName
              y.ServiceNameCopy<-x.ServiceName
              y.ServiceDescription<-x.ServiceDescription
              y.ServiceDescriptionCopy<-x.ServiceDescription
              y.ServiceCode<-x.ServiceCode
              y.ServiceCodeCopy<-x.ServiceCode
              y.ServiceCodeChangedType<-x.ServiceCodeChangedType
              y.ServiceCodeChangedTypeCopy<-x.ServiceCodeChangedType
              y.ReferenceCount<-x.ReferenceCount
              y
        match 
          this.D_CodeChangedTypeItems
          |>Seq.tryFind(fun a->a.Key=x.ServiceCodeChangedType)
          with
        | Some y->this.D_CodeChangedTypeItem<-y
        | _ ->this.D_CodeChangedTypeItem<-this.D_CodeChangedTypeItems.[0]
    | _ ->()

  [<DV>] val mutable private _D_FWOriginal:ServiceInfo
  member this.D_FWOriginal 
    with get ():ServiceInfo=this._D_FWOriginal
    and set v=
      if this._D_FWOriginal<>v then
        this._D_FWOriginal<-v

  [<DV>] val mutable private _D_FWModify:ServiceInfo
  member this.D_FWModify 
    with get ()=this._D_FWModify
    and set v=
      if this._D_FWModify<>v then
        this._D_FWModify<-v
        this.OnPropertyChanged "D_FWModify"

  [<DV>] val mutable private _D_CodeChangedTypeItem:KeyValue<ChangedType,string>
  member this.D_CodeChangedTypeItem 
    with get ()=this._D_CodeChangedTypeItem
    and set v=
      if this._D_CodeChangedTypeItem<>v then
        this._D_CodeChangedTypeItem<-v
        this.OnPropertyChanged "D_CodeChangedTypeItem"
        if v<>Null() then
          match this.D_FWModify with
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
            match this.D_FWModify with
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
                    y.SetAttributeValue(XName.Get("ServiceDescription",XmlData.NS),x.ServiceDescription)
                    y.SetAttributeValue(XName.Get("ServiceCode",XmlData.NS),x.ServiceCode)
                    y.SetAttributeValue(XName.Get("ServiceCodeChangedType",XmlData.NS),int x.ServiceCodeChangedType|>string)
                    if XmlData.SaveXDoc() then
                      this.CloseCommand.Execute x
                | _ -> ()
            | _ ->()
            ),
          (fun _ ->
            match this.D_FWModify with
            | NotNull x ->
                x.IsDirtyOfServiceDescription||
                x.IsDirtyOfServiceCode || x.IsDirtyOfServiceCodeChangedType
            | _ ->false
            ))
      this. _CMD_Save  