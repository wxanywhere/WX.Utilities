namespace WX.Utilities.WPFDesignerX.BusinessEditor
open System
open System.Xml.Linq
open System.Collections.ObjectModel
open System.Windows
open System.Windows.Data
open System.Windows.Controls
open System.Windows.Input
open WX
open WX.Utilities.WPFDesignerX.Common

type FVM_FWXZ()=
  inherit ViewModelBase()
  let mutable _ServiceInfos:ServiceInfo[] option=None

  member this.Initialize(savedBeforeServiceInfos:ServiceInfo[],originalServiceInfos:ServiceInfo[])=
    this.Title<-"服务信息选择"
    this.D_ServiceInfoView.Clear()
    match 
      XmlData.XServiceInfos, 
      originalServiceInfos
      |>Seq.filter(fun a->savedBeforeServiceInfos|>Seq.exists(fun b->b.ServiceName=a.ServiceName)|>not) 
      with
    | Some xa, xb when xa.HasElements ->
        seq{
          match xa.Descendants(XName.Get("ServiceInfo",XmlData.NS)) with
          | x  ->
              for n in x do
                match new ServiceInfo() with
                | y ->
                    match n.Attribute(XName.Get("ServiceName",XmlData.NS)) with
                    | NotNull z ->y.ServiceName<-z.Value
                    | _ ->()
                    match n.Attribute(XName.Get("ServiceDescription",XmlData.NS)) with
                    | NotNull z ->y.ServiceDescription<-z.Value
                    | _ ->()
                    match n.Attribute(XName.Get("ServiceCode",XmlData.NS)) with
                    | NotNull z ->y.ServiceCode<-z.Value
                    | _ ->()
                    match n.Attribute(XName.Get("ReferenceCount",XmlData.NS)) with //冗余
                    | NotNull z ->
                        match z.Value|>Int32.TryParse with
                        | true,u ->
                            y.ReferenceCount<-u
                        | _ ->()
                    | _ ->()
                    match n.Attribute(XName.Get("ServiceCodeChangedType",XmlData.NS)) with
                    | NotNull z ->
                        match z.Value|>Int32.TryParse with
                        | true , u ->
                            match u with
                            | 0 ->y.ServiceCodeChangedType<-ChangedType.Unchanged
                            | 1 ->y.ServiceCodeChangedType<-ChangedType.Added
                            | 2->y.ServiceCodeChangedType<-ChangedType.Modified
                            | 3->y.ServiceCodeChangedType<-ChangedType.Dirtied
                            | _ ->y.ServiceCodeChangedType<-ChangedType.Unchanged
                        | _ ->()
                    | _ ->()
                    yield y
        }
        |>Seq.toArray
        |>Seq.filter(fun a->savedBeforeServiceInfos|>Seq.exists(fun b->b.ServiceName=a.ServiceName)|>not)
        |>Seq.iter (fun a->
            match xb|>Seq.tryFind (fun b->b.ServiceName=a.ServiceName) with
            | Some z ->a.ReferenceCount<-z.ReferenceCount
            | _ ->()
            this.D_ServiceInfoView.Add a)
        _ServiceInfos<-this.D_ServiceInfoView|>Seq.toArray|>Some
    | _ ->()

  [<DV>]val mutable private _D_ServiceInfoView:ObservableCollection<ServiceInfo>
  member  this.D_ServiceInfoView
    with get ():ObservableCollection<_>=
      if this._D_ServiceInfoView=null then
        this._D_ServiceInfoView<-new ObservableCollection<_>()
      this._D_ServiceInfoView

  [<DV>] val mutable private _D_ServiceInfoSelected:ServiceInfo
  member this.D_ServiceInfoSelected 
    with get ()=this._D_ServiceInfoSelected
    and set v=
      if this._D_ServiceInfoSelected<>v then
        this._D_ServiceInfoSelected<-v
        this.OnPropertyChanged "D_ServiceInfoSelected"

  [<DV>] val mutable private _D_QueryStr:string
  member this.D_QueryStr 
    with get ()=this._D_QueryStr
    and set v=
      if this._D_QueryStr<>v then
        this._D_QueryStr<-v
        this.OnPropertyChanged "D_QueryStr"
        this.CMD_Query.Execute()

  [<DV>] val mutable private _IsContentProcessing_Query:bool
  member this.IsContentProcessing_Query 
    with get ()=this._IsContentProcessing_Query
    and set v=
      if this._IsContentProcessing_Query<>v then
        this._IsContentProcessing_Query<-v
        this.OnPropertyChanged "IsContentProcessing_Query"

  //服务信息增加
  [<DV>]val mutable _CMD_FWAdd:ICommand
  member this.CMD_FWAdd
    with get ()=
      if this. _CMD_FWAdd=null then
        this. _CMD_FWAdd<-new RelayCommand(fun _->
          match new FVM_FWXZ_Add() with
          | vm ->
              vm.Initialize()
              vm.RequestClose.Add(fun e->
                match e.Data with
                | :? ServiceInfo as x ->
                    this.D_ServiceInfoView.Add x
                | _ ->()
                )
              this.ShowDialog(vm)
          )
      this. _CMD_FWAdd  

  //服务信息修改
  [<DV>]val mutable _CMD_FWModify:ICommand
  member this.CMD_FWModify
    with get ()=
      if this. _CMD_FWModify=null then
        this. _CMD_FWModify<-new RelayCommand(
          (fun _->
            match 
              new FVM_FWXZ_Modify(), 
              this.D_ServiceInfoView  
              |>Seq.tryFind (fun a->a.IsChecked)
              with
            | vm, Some xs ->
                vm.Initialize xs
                vm.RequestClose.Add(fun e->
                  match e.Data with
                  | :? ServiceInfo as x ->
                      match this.D_ServiceInfoView.IndexOf xs with
                      | y ->
                          this.D_ServiceInfoView.[y]<-x
                  | _ ->()
                  )
                this.ShowDialog(vm)
            | _ ->()
            ),
          (fun _ ->
            this.D_ServiceInfoView
            |>Seq.filter(fun a->a.IsChecked)|>Seq.length=1
            ))
      this. _CMD_FWModify  

  //服务信息删除
  [<DV>]val mutable _CMD_FWDelete:ICommand
  member this.CMD_FWDelete
    with get ()=
      if this. _CMD_FWDelete=null then
        this. _CMD_FWDelete<-new RelayCommand(
          (fun _->
            match MessageBox.Show ("勾选的记录将被删除，是否继续？","操作确认",MessageBoxButton.YesNo,MessageBoxImage.Question) with
            | MessageBoxResult.OK -> 
                match 
                  this.D_ServiceInfoView
                  |>Seq.filter (fun a->a.IsChecked && a.ReferenceCount=0)
                  |>Seq.toArray with
                | x->
                    for n in x do
                      match 
                        XmlData.XServiceInfos.Value.Elements()
                        |>Seq.tryFind(fun a->
                            match a.Attribute(XName.Get("ServiceName",XmlData.NS)) with
                            | NotNull y ->y.Value=n.ServiceName
                            | _ ->false
                            )
                        with
                      | Some y ->
                          y.Remove()
                      | _ ->()
                    if XmlData.SaveXDoc() then
                      x|>Seq.iter(fun a->this.D_ServiceInfoView.Remove a|>ignore)
            | _ ->()
            ),
          (fun _ ->
            this.D_ServiceInfoView|>Seq.exists (fun a->a.IsChecked) && 
            this.D_ServiceInfoView|>Seq.filter(fun a->a.IsChecked)|>Seq.forall (fun a->a.ReferenceCount=0)
            ))
      this. _CMD_FWDelete  

  [<DV>]val mutable _CMD_FWView:ICommand
  member this.CMD_FWView
    with get ()=
      if this. _CMD_FWView=null then
        this. _CMD_FWView<-new RelayCommand(
          (fun _->
            match this.D_ServiceInfoSelected with
            | NotNull x ->
                match new FVM_FWXX() with
                | vm ->
                    vm.Initialize x
                    this.ShowDialog(vm)
            | _ ->()
            ),
          (fun _ ->this.D_ServiceInfoSelected<>Null()))
      this. _CMD_FWView  

  [<DV>]val mutable _CMD_Select:ICommand
  member this.CMD_Select
    with get ()=
      if this. _CMD_Select=null then
        this. _CMD_Select<-new RelayCommand(
          (fun _->
            this.D_ServiceInfoView
            |>Seq.filter (fun a->a.IsChecked)
            |>Seq.map (fun a->a.ReferenceCount<-a.ReferenceCount+1;a)
            |>Seq.toArray
            |>this.CloseCommand.Execute
            ),
          (fun _ ->this.D_ServiceInfoView|>Seq.exists (fun a->a.IsChecked)))
      this. _CMD_Select  

  [<DV>]val mutable _CMD_Query:ICommand
  member this.CMD_Query
    with get ():ICommand=
      if this. _CMD_Query=null then
        this. _CMD_Query<-new RelayCommand(
          (fun _->
            this.IsContentProcessing_Query<-true
            this.D_ServiceInfoView.Clear()
            match new AsyncWorker<_>(async{
              //do! Async.Sleep 5000
              return 
                match _ServiceInfos, this.D_QueryStr with
                | Some x, y when String.IsNullOrWhiteSpace y |>not->
                    x
                    |>Seq.filter(fun a->a.ServiceName.ToLower().Contains (y.ToLower()))
                    |>Seq.toArray
                | Some x, _ ->x
                | _ ->[||]
              }) with
            | wk ->
                wk.Completed.Add (fun r->
                  r|>Seq.iter (fun a->this.D_ServiceInfoView.Add a)
                  this.IsContentProcessing_Query<-false
                  )
                wk.RunAsync()|>ignore
            ),
          (fun _ ->
            this.IsContentProcessing_Query|>not))
      this. _CMD_Query  