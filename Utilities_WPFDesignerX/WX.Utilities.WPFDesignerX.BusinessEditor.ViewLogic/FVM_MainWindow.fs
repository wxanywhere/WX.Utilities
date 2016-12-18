namespace WX.Utilities.WPFDesignerX.BusinessEditor
open System
open System.IO
open System.Collections.ObjectModel
open System.Windows
open System.Windows.Data
open System.Windows.Controls
open System.Windows.Input
open System.Xml
open System.Xml.Linq
open System.Text
open WX
open WX.Utilities.WPFDesignerX.Common

type FVM_MainWindow()=
  inherit ViewModelBase()
  
  member this.Initialize()=
    if this._Annotation=Null() then 
      this.Annotation<-new Annotation(UIElementID=Guid.NewGuid())//仅用于测试环境
    this.Title<- 
      "标注界面元素 -- ["+this._Annotation.UIElementName+"]"
      +
      match CommonData.IsInDesignTime, CommonData.EditorOperateScope with
      | true,EditorOperateScope.View ->"（设计模式下，仅能查看）"
      | false,EditorOperateScope.View ->"（所属界面未启用，仅能查看）"
      | _ ->String.Empty
    match CommonData.EditorOperateScope with
    | EditorOperateScope.All | EditorOperateScope.ViewAndEdit ->
        this.IsReadOnlyXQMS<-false
        this.IsReadOnlyXWMS<-false
    | _->
        this.IsReadOnlyXQMS<-true
        this.IsReadOnlyXWMS<-true
    match 
      match XmlData.XDoc with
      | None  ->
          if not CommonData.IsInDesignTime && File.Exists XmlData.FilePath|>not then this.CreateAnnotationDataFile()
          match 
            (
            try
              match XDocument.Load(XmlData.FilePath) with
              | NotNull x ->Some x
              | _ ->None
            with _ ->None)
            with
          | Some x ->
              XmlData.XDoc<-Some x
              match x.Descendants(XName.Get("Option",XmlData.NS)) with
              | y when Seq.length y=1 ->
                  XmlData.XOption<-y|>Seq.head|>Some
              | _ ->()
              Some x
          | _ ->None
      | x ->x
      with
    | Some x ->
        match x.Descendants(XName.Get("ServiceInfos",XmlData.NS)) with
        | y when Seq.length y=1 ->
            XmlData.XServiceInfos<-  y|>Seq.head|>Some
        | _ ->()
        match x.Descendants(XName.Get("Annotations",XmlData.NS)) with
        | y when Seq.length y=1 ->
            XmlData.XAnnotations<-  y|>Seq.head|>Some
        | _ ->()
        this.D_ServiceInfoView.Clear()
        this.RetrieveAnnotationData(this._Annotation)
        match this._Annotation.ServiceInfos with
        | NotNull x ->x|>Seq.iter(fun a->this.D_ServiceInfoView.Add a)
        | _ ->()
    | _ ->
        match MessageBox.Show ( @"标注数据加载失败, 是否关闭当前窗口！","错误",MessageBoxButton.YesNo,MessageBoxImage.Question) with
        | MessageBoxResult.Yes ->this.CloseCommand.Execute()
        | _ ->()
    match 
      new Uri("/WX.Utilities.WPFDesignerX.BusinessEditor.View;Component/Resources/ViewModelTemplate.xaml",UriKind.Relative)
      with 
    | NotNull x ->
        match 
          (
          try
            match Application.LoadComponent x with
            | :? ResourceDictionary as y->
                Application.Current.Resources.MergedDictionaries.Add y
                Some y
            | _ ->None
          with _ ->None)
          with
        | None ->
            match MessageBox.Show ( @"界面资源加载失败，是否关闭当前窗口","错误",MessageBoxButton.YesNo,MessageBoxImage.Question) with
            | MessageBoxResult.Yes ->this.CloseCommand.Execute()
            | _ ->()
        | _ ->()
    | _ -> 
        match MessageBox.Show ( @"'界面资源没有找到,是否关闭当前窗口","错误",MessageBoxButton.YesNo,MessageBoxImage.Question) with
        | MessageBoxResult.Yes ->this.CloseCommand.Execute()
        | _ ->()

  member this.CreateAnnotationDataFile()=
    use xw= new XmlTextWriter(XmlData.FilePath , Encoding.UTF8) //or use vx=XmlWriter.Create(this.D_FilePath)
    xw.Formatting<-Formatting.Indented
    xw.WriteStartDocument()
    xw.WriteStartElement(XmlData.Prefix,"AnnotationData",XmlData.NS)
    xw.WriteStartElement(XmlData.Prefix,"Option",XmlData.NS)
    xw.WriteAttributeString(XmlData.Prefix,"IsDisplayAnnotation",XmlData.NS,"True")
    xw.WriteAttributeString(XmlData.Prefix,"IsDisplayController",XmlData.NS,"True")
    xw.WriteAttributeString(XmlData.Prefix,"IsDesignAudited",XmlData.NS,"False")
    xw.WriteAttributeString(XmlData.Prefix,"DocumentTitle",XmlData.NS,String.Empty)
    xw.WriteEndElement()
    xw.WriteStartElement (XmlData.Prefix,"ServiceInfos",XmlData.NS)
    xw.WriteEndElement()
    xw.WriteStartElement (XmlData.Prefix,"Annotations",XmlData.NS)
    xw.WriteEndElement()
    xw.WriteEndElement()
    xw.WriteEndDocument()

  member this.RetrieveAnnotationData (annotation:Annotation)=
    annotation.ServiceInfos<-[||]
    match XmlData.XAnnotations,XmlData.XServiceInfos, annotation with
    | Some xa,Some xb,r  ->
        match 
          xa.Elements(XName.Get("Annotation",XmlData.NS))
          |>Seq.tryFind(fun a->
              match a.Attribute(XName.Get("UIElementID",XmlData.NS)) with
              | NotNull z ->
                  match z.Value|>Guid.TryParse with
                  | true, w ->w=r.UIElementID
                  | _ ->false
              | _ ->false
              )
          with
        | Some z ->
            r.EditStatus<-EditStatus.Modify
            match 
              z.Attribute(XName.Get("UIElementName",XmlData.NS)),
              z.Attribute(XName.Get("UITypeName",XmlData.NS)),
              z.Attribute(XName.Get("UIName",XmlData.NS)),
              z.Attribute(XName.Get("IsInTabItem",XmlData.NS)),
              z.Attribute(XName.Get("IsTabItem",XmlData.NS))
              with
            | NotNull wa,NotNull wb,NotNull wc, NotNull wd,NotNull we->
                match wd.Value|>bool.TryParse, we.Value|>bool.TryParse with
                | (true,ua),(true,ub) when 
                    ua<>r.IsInTabItem || ub<>r.IsTabItem || wa.Value<>r.UIElementName ||
                    wb.Value<>r.UITypeName || wc.Value<>r.UIName -> 
                    r.IsDirtyOfUIInfo<-true
                | _ ->()
            | _ ->()
            match z.Attribute(XName.Get("RequirementDescription",XmlData.NS)) with
            | NotNull w ->
                r.RequirementDescription<-w.Value
                r.RequirementDescriptionCopy<-w.Value //顺序固定
            | _ ->()
            match z.Attribute(XName.Get("BehaviorDescription",XmlData.NS)) with
            | NotNull w ->
                r.BehaviorDescription<-w.Value
                r.BehaviorDescriptionCopy<-w.Value //顺序固定
            | _ ->()
            match z.Descendants(XName.Get("Services",XmlData.NS)), xb.Elements(XName.Get("ServiceInfo",XmlData.NS)) with
            | wa, wb when Seq.length wa=1 ->
                r.ServiceInfos<-
                  seq{
                    for n in (Seq.head wa).Descendants(XName.Get("Service",XmlData.NS)) do
                      match new ServiceInfo() with
                      | w ->
                          match n.Attribute(XName.Get("ServiceName",XmlData.NS)) with
                          | NotNull u ->
                              w.ServiceName<-u.Value
                              match
                                wb
                                |>Seq.tryFind(fun a->
                                    match a.Attribute(XName.Get("ServiceName",XmlData.NS)) with
                                    | NotNull va ->va.Value=u.Value
                                    | _ ->false
                                    )
                                with
                              | Some u ->
                                  match u.Attribute(XName.Get("ServiceDescription",XmlData.NS)) with
                                  | NotNull v ->
                                      w.ServiceDescription<-v.Value
                                  | _ ->()
                                  match u.Attribute(XName.Get("ServiceCode",XmlData.NS)) with
                                  | NotNull v ->
                                      w.ServiceCode<-v.Value
                                  | _ ->()
                                  match u.Attribute(XName.Get("ReferenceCount",XmlData.NS)) with 
                                  | NotNull v ->
                                      match v.Value|>Int32.TryParse with
                                      | true,s ->
                                          w.ReferenceCount<-s
                                      | _ ->()
                                  | _ ->()
                                  match u.Attribute(XName.Get("ServiceCodeChangedType",XmlData.NS)) with
                                  | NotNull v ->
                                      match v.Value|>Int32.TryParse with
                                      | true , s ->
                                          match s with
                                          | 0 ->w.ServiceCodeChangedType<-ChangedType.Unchanged
                                          | 1 ->w.ServiceCodeChangedType<-ChangedType.Added
                                          | 2->w.ServiceCodeChangedType<-ChangedType.Modified
                                          | 3->w.ServiceCodeChangedType<-ChangedType.Dirtied
                                          | _ ->w.ServiceCodeChangedType<-ChangedType.Unchanged
                                      | _ ->()
                                  | _ ->()
                              | _ ->()
                          | _ ->()
                          yield w
                  }
                  |>Seq.toArray
            | _ ->()
        | _ ->
            r.EditStatus<-EditStatus.Add
    | _ ->()

  //--------------------------------------------------------------------

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

  [<DV>] val mutable private _Annotation:Annotation
  member this.Annotation 
    with get ()=this._Annotation
    and set v=
      if this._Annotation<>v then
        this._Annotation<-v
        this.OnPropertyChanged "Annotation"

  [<DV>] val mutable private _IsReadOnlyXQMS:bool
  member this.IsReadOnlyXQMS 
    with get ()=this._IsReadOnlyXQMS
    and set v=
      if this._IsReadOnlyXQMS<>v then
        this._IsReadOnlyXQMS<-v
        this.OnPropertyChanged "IsReadOnlyXQMS"

  [<DV>] val mutable private _IsReadOnlyXWMS:bool
  member this.IsReadOnlyXWMS 
    with get ()=this._IsReadOnlyXWMS
    and set v=
      if this._IsReadOnlyXWMS<>v then
        this._IsReadOnlyXWMS<-v
        this.OnPropertyChanged "IsReadOnlyXWMS"

  [<DV>] val mutable private _IsCheckedAllFW:bool
  member this.IsCheckedAllFW 
    with get ()=this._IsCheckedAllFW
    and set v=
      if this._IsCheckedAllFW<>v then
        this._IsCheckedAllFW<-v
        this.OnPropertyChanged "IsCheckedAllFW"
        this.D_ServiceInfoView|>Seq.iter (fun a->a.IsChecked<-v)

  //--------------------------------------------------------------------

  //添加服务信息
  [<DV>]val mutable _CMD_FWAppend:ICommand
  member this.CMD_FWAppend
    with get ()=
      if this. _CMD_FWAppend=null then
        this. _CMD_FWAppend<-new RelayCommand(
          (fun _->
            match new FVM_FWXZ() with
            | vm ->
                vm.Initialize(this.D_ServiceInfoView|>Seq.toArray,this.Annotation.ServiceInfos)
                vm.RequestClose.Add(fun e->
                  match e.Data, this.Annotation with
                  | (:? (ServiceInfo[]) as x),y ->
                      x
                      |>Seq.iter (fun a->this.D_ServiceInfoView.Add a) 
                      match 
                        this.D_ServiceInfoView
                        |>Seq.forall (fun a->y.ServiceInfos|>Seq.exists(fun b->b.ServiceName=a.ServiceName))
                        &&
                        y.ServiceInfos
                        |>Seq.forall (fun a->this.D_ServiceInfoView|>Seq.exists(fun b->b.ServiceName=a.ServiceName))
                        with
                      | true ->y.IsDirtyOfServiceInfos<-false
                      | _ ->y.IsDirtyOfServiceInfos<-true
                  | _ ->()
                  )
                this.ShowDialog(vm)
            ),
          (fun _ ->
            match CommonData.EditorOperateScope with
            | EditorOperateScope.All | EditorOperateScope.ViewAndEdit ->true
            | _-> false
            ))
      this. _CMD_FWAppend  

  //服务信息移除
  [<DV>]val mutable _CMD_FWRemove:ICommand
  member this.CMD_FWRemove
    with get ()=
      if this. _CMD_FWRemove=null then
        this. _CMD_FWRemove<-new RelayCommand(
          (fun _->
            this.D_ServiceInfoView
            |>Seq.filter (fun a->a.IsChecked)
            |>Seq.map (fun a->a.ReferenceCount<-a.ReferenceCount-1;a) //只针对更改前的数据实例
            |>Seq.toArray
            |>Seq.iter (fun a->this.D_ServiceInfoView.Remove a|>ignore)
            match this.Annotation with
            | x ->
                match 
                  this.D_ServiceInfoView
                  |>Seq.forall (fun a->x.ServiceInfos|>Seq.exists(fun b->b.ServiceName=a.ServiceName))
                  &&
                  x.ServiceInfos
                  |>Seq.forall (fun a->this.D_ServiceInfoView|>Seq.exists(fun b->b.ServiceName=a.ServiceName))
                  with
                | true ->x.IsDirtyOfServiceInfos<-false
                | _ ->x.IsDirtyOfServiceInfos<-true
            ),
          (fun _ ->
            match CommonData.EditorOperateScope with
            | EditorOperateScope.All | EditorOperateScope.ViewAndEdit ->true
            | _-> false
            &&
            this.D_ServiceInfoView
            |>Seq.exists (fun a->a.IsChecked) 
             ))
      this. _CMD_FWRemove  

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

  [<DV>]val mutable _CMD_XX:ICommand
  member this.CMD_XX
    with get ()=
      if this. _CMD_XX=null then
        this. _CMD_XX<-new RelayCommand(
          (fun _->
            match new FVM_XX() with
            | vm ->
                vm.Initialize()
                this.ShowDialog(vm)
            ),
          (fun _ ->
            match CommonData.EditorOperateScope with
            | EditorOperateScope.All | EditorOperateScope.ViewAndOption ->true
            | _-> false
            ))
      this. _CMD_XX  

  [<DV>]val mutable _CMD_Save:ICommand
  member this.CMD_Save
    with get ()=
      if this. _CMD_Save=null then
        this. _CMD_Save<-new RelayCommand(
          (fun _->
            match this.Annotation with
            | NotNull x ->
                match XmlData.XAnnotations with
                | Some w  ->
                    match x.EditStatus with
                    | EditStatus.Add  ->
                        match  new XElement(XName.Get("Annotation",XmlData.NS)) with
                        |  z->
                            z.SetAttributeValue(XName.Get ("UIElementID",XmlData.NS),string x.UIElementID)
                            z.SetAttributeValue(XName.Get ("UIElementName",XmlData.NS),x.UIElementName)
                            z.SetAttributeValue(XName.Get ("UITypeName",XmlData.NS),x.UITypeName)
                            z.SetAttributeValue(XName.Get ("UIName",XmlData.NS),x.UIName)
                            z.SetAttributeValue(XName.Get ("IsInTabItem",XmlData.NS),x.IsInTabItem)
                            z.SetAttributeValue(XName.Get ("IsTabItem",XmlData.NS),x.IsTabItem)
                            z.SetAttributeValue(XName.Get ("TabControlNumber",XmlData.NS),x.TabControlNumber)
                            z.SetAttributeValue(XName.Get ("TabItemNumber",XmlData.NS),x.TabItemNumber)
                            z.SetAttributeValue(XName.Get ("TabItemHeader",XmlData.NS),x.TabItemHeader)
                            z.SetAttributeValue(XName.Get ("RequirementDescription",XmlData.NS),x.RequirementDescription)
                            z.SetAttributeValue(XName.Get ("BehaviorDescription",XmlData.NS),x.BehaviorDescription)
                            match 
                                new XElement(XName.Get("Services",XmlData.NS)),
                                XmlData.XServiceInfos.Value.Elements() with
                            | ua,ub -> 
                                z.AddFirst ua
                                for n in this.D_ServiceInfoView do
                                  match new XElement(XName.Get("Service",XmlData.NS)) with //更新服务的引用计数
                                  | v ->
                                      v.SetAttributeValue(XName.Get ("ServiceName",XmlData.NS),n.ServiceName)
                                      ua.AddFirst v
                                      match 
                                        ub
                                        |>Seq.tryFind(fun a->
                                            match a.Attribute(XName.Get("ServiceName",XmlData.NS)) with
                                            | NotNull s ->s.Value=n.ServiceName
                                            | _ ->false
                                            )
                                        with
                                      | Some s ->
                                          s.SetAttributeValue(XName.Get("ReferenceCount",XmlData.NS),n.ReferenceCount)
                                          (*
                                          match s.Attribute(XName.Get("ReferenceCount",XmlData.NS)) with
                                          | NotNull t ->
                                              match t.Value|>Int32.TryParse with
                                              | true, r ->
                                                  s.SetAttributeValue(XName.Get("ReferenceCount",XmlData.NS),r+1)
                                              | _ ->()
                                          | _ ->()
                                          *)
                                      | _ ->()
                            w.AddFirst z
                    | EditStatus.Modify when w.HasElements->
                        match 
                          w.Elements(XName.Get("Annotation",XmlData.NS))
                          |>Seq.tryFind(fun a->
                              match a.Attribute(XName.Get("UIElementID",XmlData.NS)) with
                              | NotNull z ->
                                  match z.Value|>Guid.TryParse with
                                  | true, u ->u=x.UIElementID
                                  | _ ->false
                              | _ ->false
                              )
                          with
                        | Some z ->
                            z.SetAttributeValue(XName.Get("UIElementName",XmlData.NS),x.UIElementName)
                            z.SetAttributeValue(XName.Get("UITypeName",XmlData.NS),x.UITypeName)
                            z.SetAttributeValue(XName.Get("UIName",XmlData.NS),x.UIName)
                            z.SetAttributeValue(XName.Get("IsInTabItem",XmlData.NS),x.IsInTabItem)
                            z.SetAttributeValue(XName.Get("IsTabItem",XmlData.NS),x.IsTabItem)
                            z.SetAttributeValue(XName.Get("RequirementDescription",XmlData.NS),x.RequirementDescription)
                            z.SetAttributeValue(XName.Get("BehaviorDescription",XmlData.NS),x.BehaviorDescription)
                            z.RemoveNodes()
                            match new XElement(XName.Get("Services",XmlData.NS)) with
                            | u -> 
                                z.AddFirst u
                                for n in this.D_ServiceInfoView do
                                  match new XElement(XName.Get("Service",XmlData.NS)) with
                                  | v ->
                                      v.SetAttributeValue(XName.Get ("ServiceName",XmlData.NS),n.ServiceName)
                                      u.AddFirst v
                            match XmlData.XServiceInfos.Value.Elements() with //更新服务引用次数
                            | s ->
                                match //增加的部分
                                  this.D_ServiceInfoView
                                  |>Seq.filter (fun a->x.ServiceInfos|>Seq.exists(fun b->b.ServiceName=a.ServiceName)|>not)
                                  |>Seq.toArray 
                                  with
                                | u ->
                                    for n in u do
                                      match 
                                        s
                                        |>Seq.tryFind(fun a->
                                            match a.Attribute(XName.Get("ServiceName",XmlData.NS)) with
                                            | NotNull v ->v.Value=n.ServiceName
                                            | _ ->false
                                            )
                                        with
                                      | Some v ->
                                          v.SetAttributeValue(XName.Get("ReferenceCount",XmlData.NS),n.ReferenceCount)
                                      | _ ->()
                                match //删除的部分
                                  x.ServiceInfos
                                  |>Seq.filter (fun a->this.D_ServiceInfoView|>Seq.exists(fun b->b.ServiceName=a.ServiceName)|>not)
                                  |>Seq.toArray 
                                  with
                                | u ->
                                    for n in u do
                                      match 
                                        s
                                        |>Seq.tryFind(fun a->
                                            match a.Attribute(XName.Get("ServiceName",XmlData.NS)) with
                                            | NotNull v ->v.Value=n.ServiceName
                                            | _ ->false
                                            )
                                        with
                                      | Some v ->
                                          v.SetAttributeValue(XName.Get("ReferenceCount",XmlData.NS),n.ReferenceCount)
                                      | _ ->()

                        | _ ->MessageBox.Show ("找不到需要修改的标注数据，保存失败！","错误提示")|>ignore
                    | _ ->()
                    try
                      if XmlData.SaveXDoc() then
                        x.IsDirtyOfUIInfo<-false
                        x.IsDirtyOfServiceInfos<-false
                        x.RequirementDescriptionCopy<-x.RequirementDescription
                        x.BehaviorDescriptionCopy<-x.BehaviorDescription
                        x.ServiceInfos<-this.D_ServiceInfoView|>Seq.toArray
                    with _ ->MessageBox.Show ("保存标注数据失败！","错误提示",MessageBoxButton.OK,MessageBoxImage.Error)|>ignore
                | _ ->MessageBox.Show ("标注数据无法获取，保存失败！","错误提示",MessageBoxButton.OK,MessageBoxImage.Error)|>ignore
            | _ ->()
            ),
          (fun _ ->
            match CommonData.EditorOperateScope with
            | EditorOperateScope.All | EditorOperateScope.ViewAndEdit ->true
            | _-> false
            &&
            match this.Annotation with
            | NotNull x ->
                x.IsDirtyOfUIInfo || x.IsDirtyOfRequirementDescription || 
                x.IsDirtyOfBehaviorDescription || x.IsDirtyOfServiceInfos
            | _ ->false
            ))
      this. _CMD_Save  

  [<DV>]val mutable _CMD_SaveClose:ICommand
  member this.CMD_SaveClose
    with get ()=
      if this. _CMD_SaveClose=null then
        this. _CMD_SaveClose<-new RelayCommand(
          (fun _->
            this.CMD_Save.Execute()
            this.CloseCommand.Execute()
            ),
          (fun _ ->this.CMD_Save.CanExecute()))
      this. _CMD_SaveClose  