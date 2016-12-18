namespace WX.Utilities.WPFDesignerX.BusinessEditor
open System
open System.Collections
open System.Text
open System.IO
open System.Xml
open System.Xml.Linq
open System.Windows
open System.Windows.Data
open System.Windows.Controls
open System.Windows.Input
open System.Windows.Media
open System.Windows.Media.Imaging
open System.Globalization
open System.Reflection
open Microsoft.Win32
open Microsoft.FSharp.Collections
open DocumentFormat.OpenXml
open DocumentFormat.OpenXml.Wordprocessing
open DocumentFormat.OpenXml.Packaging
open WX
open WX.Utilities.WPFDesignerX.Common
open WX.Utilities.WPFDesignerX.Office.Word

type FVM_XX()=
  inherit ViewModelBase()

  member this.Initialize()=
    this.Title<-"标注选项"
    this.IsDisplayAnnotation<-XmlData.IsDisplayAnnotation
    this.IsDisplayController<-XmlData.IsDisplayController
    this.IsDesignAudited<-XmlData.IsDesignAudited
    this.DocumentTitle<-XmlData.DocumentTitle
    this.IsEnableDisplayController<-true

  [<DV>] val mutable private _IsDisplayAnnotation :bool
  member this.IsDisplayAnnotation  
    with get ()=this._IsDisplayAnnotation 
    and set v=
      if this._IsDisplayAnnotation <>v then
        this._IsDisplayAnnotation <-v
        this.OnPropertyChanged "IsDisplayAnnotation"
        match XmlData.XOption with
        | Some x ->
            x.SetAttributeValue(XName.Get("IsDisplayAnnotation",XmlData.NS),string v)
            XmlData.SaveXDoc()|>ignore
            XmlData.IsDisplayAnnotation<-v
        | _ ->()

  [<DV>] val mutable private _IsDisplayController :bool
  member this.IsDisplayController  
    with get ()=this._IsDisplayController 
    and set v=
      if this._IsDisplayController <>v then
        this._IsDisplayController <-v
        this.OnPropertyChanged "IsDisplayController"
        match XmlData.XOption with
        | Some x ->
            x.SetAttributeValue(XName.Get("IsDisplayController",XmlData.NS),string v)
            XmlData.SaveXDoc()|>ignore
            XmlData.IsDisplayController<-v
        | _ ->()

  [<DV>] val mutable private _IsDesignAudited :bool
  member this.IsDesignAudited  
    with get ()=this._IsDesignAudited 
    and set v=
      if this._IsDesignAudited <>v then
        this._IsDesignAudited <-v
        this.OnPropertyChanged "IsDesignAudited"
        match XmlData.XOption with
        | Some x ->
            x.SetAttributeValue(XName.Get("IsDesignAudited",XmlData.NS),string v)
            XmlData.SaveXDoc()|>ignore
            XmlData.IsDesignAudited<-v
        | _ ->()

  [<DV>] val mutable private _DocumentTitle :string
  member this.DocumentTitle  
    with get ()=this._DocumentTitle 
    and set v=
      if this._DocumentTitle <>v then
        this._DocumentTitle <-v
        this.OnPropertyChanged "DocumentTitle"
        if v<>null then
          match XmlData.XOption with
          | Some x ->
              x.SetAttributeValue(XName.Get("DocumentTitle",XmlData.NS),v)
              XmlData.SaveXDoc()|>ignore
              XmlData.DocumentTitle<-v
          | _ ->()

  [<DV>] val mutable private _IsEnableDisplayController :bool
  member this.IsEnableDisplayController  
    with get ()=this._IsEnableDisplayController 
    and set v=
      if this._IsEnableDisplayController <>v then
        this._IsEnableDisplayController <-v
        this.OnPropertyChanged "IsEnableDisplayController"

  //---------------------------------------------------------------

  [<DV>] val mutable private _IsExportChangedPartOnly :bool
  member this.IsExportChangedPartOnly  
    with get ()=this._IsExportChangedPartOnly 
    and set v=
      if this._IsExportChangedPartOnly <>v then
        this._IsExportChangedPartOnly <-v
        this.OnPropertyChanged "IsExportChangedPartOnly"

  //---------------------------------------------------------------

  [<DV>] val mutable private _D_ArchivePath :string
  member this.D_ArchivePath  
    with get ()=this._D_ArchivePath 
    and set v=
      if this._D_ArchivePath <>v then
        this._D_ArchivePath <-v
        this.OnPropertyChanged "D_ArchivePath"

  [<DV>] val mutable private _D_RollbackPath:string
  member this.D_RollbackPath 
    with get ()=this._D_RollbackPath
    and set v=
      if this._D_RollbackPath<>v then
        this._D_RollbackPath<-v
        this.OnPropertyChanged "D_RollbackPath"

  [<DV>] val mutable private _D_ComparePath:string
  member this.D_ComparePath 
    with get ()=this._D_ComparePath
    and set v=
      if this._D_ComparePath<>v then
        this._D_ComparePath<-v
        this.OnPropertyChanged "D_ComparePath"

  [<DV>] val mutable private _D_DocPath:string
  member this.D_DocPath 
    with get ()=this._D_DocPath
    and set v=
      if this._D_DocPath<>v then
        this._D_DocPath<-v
        this.OnPropertyChanged "D_DocPath"

  [<DV>] val mutable private _IsContentProcessing_ArchiveExport:bool
  member this.IsContentProcessing_ArchiveExport 
    with get ()=this._IsContentProcessing_ArchiveExport
    and set v=
      if this._IsContentProcessing_ArchiveExport<>v then
        this._IsContentProcessing_ArchiveExport<-v
        this.OnPropertyChanged "IsContentProcessing_ArchiveExport"

  [<DV>] val mutable private _IsContentProcessing_Rollback:bool
  member this.IsContentProcessing_Rollback 
    with get ()=this._IsContentProcessing_Rollback
    and set v=
      if this._IsContentProcessing_Rollback<>v then
        this._IsContentProcessing_Rollback<-v
        this.OnPropertyChanged "IsContentProcessing_Rollback"

  [<DV>] val mutable private _IsContentProcessing_DocExport:bool
  member this.IsContentProcessing_DocExport 
    with get ()=this._IsContentProcessing_DocExport
    and set v=
      if this._IsContentProcessing_DocExport<>v then
        this._IsContentProcessing_DocExport<-v
        this.OnPropertyChanged "IsContentProcessing_DocExport"

  [<DV>] val mutable private _IsContentProcessing_DataCleaning:bool
  member this.IsContentProcessing_DataCleaning 
    with get ()=this._IsContentProcessing_DataCleaning
    and set v=
      if this._IsContentProcessing_DataCleaning<>v then
        this._IsContentProcessing_DataCleaning<-v
        this.OnPropertyChanged "IsContentProcessing_DataCleaning"

  //---------------------------------------------------------------

  [<DV>]val mutable _CMD_DesignAudited:ICommand
  member this.CMD_DesignAudited
    with get ()=
      if this. _CMD_DesignAudited=null then
        this. _CMD_DesignAudited<-new RelayCommand(
          fun _->
            match this.IsDesignAudited with
            | true ->
                this.IsDisplayController<-false
                this.IsEnableDisplayController<-false
            | _ ->
                this.IsEnableDisplayController<-true
            )
      this. _CMD_DesignAudited  

  [<DV>]val mutable _CMD_DisplayAnnotation:ICommand
  member this.CMD_DisplayAnnotation
    with get ()=
      if this. _CMD_DisplayAnnotation=null then
        this. _CMD_DisplayAnnotation<-new RelayCommand(
          fun _->
            match CommonData.IsInDesignTime,this.IsDisplayAnnotation with
            | false, false ->
                MessageBox.Show("运行状态下，取消的标注显示，将在程序关闭时恢复！","操作提示",MessageBoxButton.OK,MessageBoxImage.Information)|>ignore
                XmlData.DisplayAnnotationRestoreFlag<-true
                XmlData.OnIsDisplayAnnotationChanged false
            | x, y->
                if not x && y then XmlData.DisplayAnnotationRestoreFlag<-false
                XmlData.OnIsDisplayAnnotationChanged y
            )
      this. _CMD_DisplayAnnotation  

  [<DV>]val mutable _CMD_ArchivePath:ICommand
  member this.CMD_ArchivePath
    with get ()=
      if this. _CMD_ArchivePath=null then
        this. _CMD_ArchivePath<-new RelayCommand(
          (fun _->
            match new SaveFileDialog() with
            | x ->
                x.FileName<-"AD_模块或功能代码_"+DateTime.Now.ToString("yyyy-MM-dd")
                x.DefaultExt<-".xml"
                x.Filter<-"xml document (*.xml)|*.xml"
                match x.ShowDialog() with
                | y  when y.HasValue && y.Value  ->
                    this.D_ArchivePath<-x.FileName
                | _ ->()
            ),
          (fun _ ->this.IsContentProcessing_ArchiveExport|>not))
      this. _CMD_ArchivePath  

  [<DV>]val mutable _CMD_ArchiveExport:ICommand
  member this.CMD_ArchiveExport
    with get ()=
      if this. _CMD_ArchiveExport=null then
        let syncContext = System.Threading.SynchronizationContext.Current
        this. _CMD_ArchiveExport<-new RelayCommand(
          (fun _->
            this.IsContentProcessing_ArchiveExport<-true
            match new AsyncWorker<_>(async{
              match this.D_ArchivePath with
              | x ->
                  try
                    using(new XmlTextWriter(x , Encoding.UTF8))(fun xw->
                      xw.Formatting<-Formatting.Indented
                      XmlData.XDoc.Value.WriteTo xw
                    )
                    match XDocument.Load x,CommonData.UIAssembly with
                    | NotNull z, Some s ->
                        match z.Descendants(XName.Get("AnnotationData",XmlData.NS)) with
                        | wr when Seq.length wr=1 ->
                            match new XElement(XName.Get("UIInfos",XmlData.NS)) with
                            | w ->
                                do! Async.SwitchToContext(syncContext)
                                for n in 
                                  s.GetTypes()
                                  |>Seq.filter(fun a->
                                      if a.IsSubclassOf typeof<FrameworkElement> then
                                        match Activator.CreateInstance(a) with
                                        | :? FrameworkElement as ye when CommonData.TryGetUITypeRoot ye|>Option.isSome ->true
                                        | _ ->false
                                      else false
                                      )
                                  |>Seq.toArray do
                                  match new XElement(XName.Get("UIInfo",XmlData.NS)) with
                                  | u ->
                                      u.SetAttributeValue(XName.Get("UITypeName",XmlData.NS),n.Name)
                                      w.AddFirst u
                                do! Async.SwitchToThreadPool()
                                (Seq.head wr).Add w
                                z.Save x
                        | _ ->()
                    | _ ->()
                    return true
                  with _ ->return false
              }) with
            | wk ->
                wk.Completed.Add (fun r->
                  this.IsContentProcessing_ArchiveExport<-false
                  match r with
                  | true ->MessageBox.Show ("导出归档文件成功！","操作提示",MessageBoxButton.OK,MessageBoxImage.Information)|>ignore
                  | _ ->MessageBox.Show ("导出归档文件失败！","错误",MessageBoxButton.OK,MessageBoxImage.Error)|>ignore
                  )
                wk.RunAsync()|>ignore
            ),
          (fun _ ->
            String.IsNullOrWhiteSpace this.D_ArchivePath|>not && 
            this.IsContentProcessing_ArchiveExport|>not))
      this. _CMD_ArchiveExport  

  [<DV>]val mutable _CMD_RollbackPath:ICommand
  member this.CMD_RollbackPath
    with get ()=
      if this. _CMD_RollbackPath=null then
        this. _CMD_RollbackPath<-new RelayCommand(
          (fun _->
            match new OpenFileDialog() with
            | x ->
                x.Multiselect<-false
                x.FileName<-""
                x.DefaultExt<-".xml"
                x.Filter<-"xml document (*.xml)|*.xml"
                x.CheckFileExists<-true
                match x.ShowDialog() with
                | y when y.HasValue && y.Value ->
                    this.D_RollbackPath<-x.FileName
                | _ ->()
            ),
          (fun _ ->this.IsContentProcessing_Rollback|>not))
      this. _CMD_RollbackPath  

  [<DV>]val mutable _CMD_Rollback:ICommand
  member this.CMD_Rollback
    with get ()=
      if this. _CMD_Rollback=null then
        this. _CMD_Rollback<-new RelayCommand(
          (fun _->
            match MessageBox.Show ("标注数据回滚后,标注窗口将会关闭，是否继续？","操作确认",MessageBoxButton.YesNo,MessageBoxImage.Question) with
            | MessageBoxResult.Yes ->
                this.IsContentProcessing_Rollback<-true
                match new AsyncWorker<_>(async{
                  match this.D_RollbackPath with
                  | x ->
                      return 
                        try
                          match XDocument.Load x with
                          | NotNull y ->
                              using(new XmlTextWriter(XmlData.FilePath , Encoding.UTF8))(fun xw->
                                xw.Formatting<-Formatting.Indented
                                y.WriteTo xw
                              )
                              match XDocument.Load XmlData.FilePath with //删除UI信息节点
                              | NotNull z->
                                  match z.Descendants(XName.Get("UIInfos",XmlData.NS)) with
                                  | w when Seq.length w=1 ->
                                      w.Remove() //取大于1可修正文档结构 
                                      z.Save(XmlData.FilePath)
                                  | _ ->()
                              | _ ->()
                              true
                          | _ ->false
                        with _ ->false
                  }) with
                | wk ->
                    wk.Completed.Add (fun r->
                      this.IsContentProcessing_Rollback<-false
                      match r with
                      | true ->
                          MessageBox.Show ("标注数据回滚成功,标注窗口将关闭!","操作提示",MessageBoxButton.OK,MessageBoxImage.Information)|>ignore
                          XmlData.XDoc<-None
                          this.CloseCommand.Execute()
                          match this.MainWindowModel with
                          | NotNull x->x.CloseCommand.Execute()
                          | _ ->()
                      | _ ->MessageBox.Show ("标注数据回滚失败!","错误提示",MessageBoxButton.OK,MessageBoxImage.Error)|>ignore
                      )
                    wk.RunAsync()|>ignore
            | _ ->()
            ),
          (fun _ ->
            File.Exists this.D_RollbackPath && 
            this.IsContentProcessing_Rollback|>not))
      this. _CMD_Rollback  

  [<DV>]val mutable _CMD_ComparePath:ICommand
  member this.CMD_ComparePath
    with get ()=
      if this. _CMD_ComparePath=null then
        this. _CMD_ComparePath<-new RelayCommand(
          (fun _->
            match new OpenFileDialog() with
            | x ->
                x.Multiselect<-false
                x.FileName<-""
                x.DefaultExt<-".xml"
                x.Filter<-"xml document (*.xml)|*.xml"
                x.CheckFileExists<-true
                match x.ShowDialog() with
                | y when y.HasValue && y.Value ->
                    this.D_ComparePath<-x.FileName
                | _ ->()
            ),
          (fun _ ->this.IsContentProcessing_DocExport|>not))
      this. _CMD_ComparePath  

  [<DV>]val mutable _CMD_DocPath:ICommand
  member this.CMD_DocPath
    with get ()=
      if this. _CMD_DocPath=null then
        this. _CMD_DocPath<-new RelayCommand(
          (fun _->
            match new SaveFileDialog() with
            | x ->
                let designTag= match this.IsDesignAudited with true ->"(已评定）" | _ ->String.Empty
                x.FileName<-"DD_模块或功能代码_"+DateTime.Now.ToString("yyyy-MM-dd")+designTag 
                x.DefaultExt<-".docx"
                x.Filter<-"Word document (*.docx)|*.docx"
                match x.ShowDialog() with
                | y  when y.HasValue && y.Value  ->
                    this.D_DocPath<-x.FileName
                | _ ->()
            ),
          (fun _ ->this.IsContentProcessing_DocExport|>not))
      this. _CMD_DocPath  

  //-------------------------------------------------------

  let GetTabControls (root:FrameworkElement)=
    root.Width<-if Double.IsNaN root.Width then 520.0 else root.Width
    root.Height<-if Double.IsNaN root.Height then 350.0 else root.Height
    root.UpdateLayout()
    let size =new Size(root.Width,root.Height)|>ref
    let serialNumber=ref 0
    match root with
    | NotNull x ->
        let rec GetTabControls (visual:Visual)=
          seq{
            match visual with
            | NotNull y ->
                match y with
                | :? TabControl as z when z.Visibility=Visibility.Visible ->
                    serialNumber|>incr
                    yield z,z.GetHashCode(),!serialNumber
                | _ ->()
                match 
                  match y with
                  | :? Window as z ->
                      match z.Content with
                      | :? FrameworkElement as w ->Some w
                      | _ ->None
                  | :? UserControl as z when z.Parent=null ->Some  (z:>FrameworkElement) //过滤嵌套的UserControl
                  | _ ->None
                  with
                | Some y ->
                      y.Measure(!size)
                      y.Arrange (new Rect(!size))
                | _ ->()
                match y with 
                | :? Window as z ->
                    match z.Content with
                    | :? FrameworkElement as w ->
                        yield! GetTabControls w
                    | _ ->()
                | _ ->
                    for n in 0..(VisualTreeHelper.GetChildrenCount y)-1 do
                      yield! VisualTreeHelper.GetChild (y,n):?>Visual|>GetTabControls
            | _ ->()
          }
        GetTabControls x
        |>Seq.sortBy (fun (_,_,a)-> -a) //须倒序
        |>Seq.toArray
    | _ ->
        raise <| new ArgumentNullException("root")

  let GetAnnotatedElements (root:FrameworkElement)=
    root.Width<-if Double.IsNaN root.Width then 520.0 else root.Width
    root.Height<-if Double.IsNaN root.Height then 350.0 else root.Height
    let size =new Size(root.Width,root.Height)|>ref
    match root,CommonData.IDProperty with
    | NotNull x, Some xa ->
        let rec GetAnnotatedElements (visual:Visual)=
          seq{
            match visual with
            | NotNull y ->
                match y with
                | :? FrameworkElement as z ->
                    match z.GetValue xa with
                    | :? Guid as w  when w>GuidDefaultValue ->
                        match z with
                        | :? Window ->yield z
                        | _ when z.Visibility=Visibility.Visible ->yield z
                        | _ ->()
                    | _ ->()
                | _ ->()
                match 
                  match y with
                  | :? Window as z ->
                      match z.Content with
                      | :? FrameworkElement as w ->Some w
                      | _ ->None
                  | :? UserControl as z when z.Parent=null ->Some  (z:>FrameworkElement) //过滤嵌套的UserControl
                  | _ ->None
                  with
                  | Some y ->
                        y.Measure(!size)
                        y.Arrange (new Rect(!size))
                  | _ ->()
                match y with 
                | :? Window as z ->
                    match z.Content with
                    | :? FrameworkElement as w ->
                        yield! GetAnnotatedElements w
                    | _ ->()
                | _ ->
                    for n in 0..(VisualTreeHelper.GetChildrenCount y)-1 do
                      yield! VisualTreeHelper.GetChild (y,n):?>Visual|>GetAnnotatedElements
            | _ ->()
          }
        GetAnnotatedElements x|>Seq.toArray
    | _ ->
        raise <| new ArgumentNullException("root")

  let GetAnnotationIDs (root:FrameworkElement)=
    root.Width<-if Double.IsNaN root.Width then 520.0 else root.Width
    root.Height<-if Double.IsNaN root.Height then 350.0 else root.Height
    let size =new Size(root.Width,root.Height)|>ref
    match root,CommonData.IDProperty with
    | NotNull x, Some xa ->
        let rec GetAnnotatedIDs (visual:Visual)=
          seq{
            match visual with
            | NotNull y ->
                match y with
                | :? FrameworkElement as z ->
                    match z.GetValue xa with
                    | :? Guid as w  when w>GuidDefaultValue ->yield w
                    | _ ->()
                | _ ->()
                match 
                  match y with
                  | :? Window as z ->
                      match z.Content with
                      | :? FrameworkElement as w ->Some w
                      | _ ->None
                  | :? UserControl as z when z.Parent=null ->Some  (z:>FrameworkElement) //过滤嵌套的UserControl
                  | _ ->None
                  with
                  | Some y ->
                        y.Measure(!size)
                        y.Arrange (new Rect(!size))
                  | _ ->()
                match y with 
                | :? Window as z ->
                    match z.Content with
                    | :? FrameworkElement as w ->
                        yield! GetAnnotatedIDs w
                    | _ ->()
                | _ ->
                    for n in 0..(VisualTreeHelper.GetChildrenCount y)-1 do
                      yield! VisualTreeHelper.GetChild (y,n):?>Visual|>GetAnnotatedIDs
            | _ ->()
          }
        GetAnnotatedIDs x|>Seq.toArray
    | _ ->
        raise <| new ArgumentNullException("root")

  let GetRenderedImage(root:FrameworkElement,annotedElements:FrameworkElement[],annotationData:WordAnnotation[])=
    root.Width<-if Double.IsNaN root.Width then 520.0 else root.Width
    root.Height<-if Double.IsNaN root.Height then 350.0 else root.Height
    match CommonData.IDProperty, root, root.LayoutTransform with
    | Some xi,x,y ->
        x.LayoutTransform<-null
        match new Size(x.Width,x.Height) with
        | z ->
            x.Measure z
            x.Arrange (new Rect(z))
            match 
              new RenderTargetBitmap(int z.Width, int z.Height,96.0,96.0,PixelFormats.Pbgra32),
              match x with
              | :? Window as w ->
                  match w.Content with
                  | :? FrameworkElement as u ->
                      //u.RenderSize<-new Size(u.RenderSize.Width+u.Margin.Left+u.Margin.Right,u.RenderSize.Height+u.Margin.Top+u.Margin.Bottom) //
                      u,true
                  | _ ->new Grid(Width=z.Width,Height=z.Height):>FrameworkElement,true
              | _ ->x,false
              with
            | wa, (wb,wc) ->
                wb.Width<-wb.ActualWidth
                wb.Height<-wb.ActualHeight
                wa.Render wb
                match new DrawingVisual() with
                | dv ->
                    using (dv.RenderOpen())(fun dc ->
                      dc.DrawImage(wa,new Rect(0.0,0.0,z.Width,z.Height))
                      dc.DrawRectangle(null,new Pen(Brushes.Gray,1.0),new Rect(0.0,0.0,z.Width,z.Height))
                      annotedElements
                      |>Seq.iteri (fun i a ->
                          match 
                            match annotationData|>Seq.tryFind(fun b->match a.GetValue xi with :? Guid as u->u=b.UIElementID | _ ->false) with
                            | Some u ->
                                match u.AnnotationChangedType with
                                | ChangedType.Added ->Brushes.Blue
                                | ChangedType.Modified->Brushes.Red
                                | _ ->Brushes.DarkGray
                            | _ ->Brushes.DarkGray
                            with
                          | za ->
                              match 
                                match wc,a with
                                | true,:? Window -> new Point(z.Width-16.0,1.0)
                                | true, _ ->a.TranslatePoint(new Point(a.ActualWidth-16.0+wb.Margin.Left,1.0+wb.Margin.Top),wb)
                                | _ ->a.TranslatePoint(new Point(a.ActualWidth-16.0,1.0),wb)
                                ,
                                new Size(15.0,13.0)
                                with
                              | ua,ub ->
                                  dc.DrawRectangle(za,new Pen(za,1.0),new Rect(ua,ub))
                              match 
                                match wc,a with
                                | true,:? Window-> new Point(z.Width-13.0,3.0)
                                | true, _ ->a.TranslatePoint(new Point(a.ActualWidth-13.0+wb.Margin.Left,3.0+wb.Margin.Top),wb)
                                | _ ->a.TranslatePoint(new Point(a.ActualWidth-13.0,3.0),wb)
                                with
                              | u ->
                                  dc.DrawText(
                                    new FormattedText( string (i+1), CultureInfo.CurrentCulture,FlowDirection.LeftToRight,  new Typeface("Verdana"),8.0,Brushes.White),
                                    u)
                          )
                      )
                    wa.Render dv
                    x.LayoutTransform<-y
                    x.Width<-z.Width
                    x.Height<-z.Height
                match new PngBitmapEncoder() with
                | u ->
                    u.Frames.Add (BitmapFrame.Create wa)
                    let stream=new MemoryStream()
                    u.Save stream
                    Some<|(stream.ToArray(), z.Height,z.Width) //Array.zeroCreate<byte> (int stream.Length)
    | _ ->None

  let GetAllServiceInfoData(serviceInfosNode:XElement)=
    match serviceInfosNode with
    | NotNull x->
        seq{
          for n in x.Elements() do
            match new WordServiceInfo() with
            | r ->
                match n.Attribute(XName.Get("ServiceName",XmlData.NS)) with
                | NotNull u ->r.ServiceName<-u.Value
                | _->()
                match n.Attribute(XName.Get("ServiceDescription",XmlData.NS)) with
                | NotNull u ->r.ServiceDescription<-u.Value
                | _ ->()
                match n.Attribute(XName.Get("ServiceCode",XmlData.NS)) with
                | NotNull u ->r.ServiceCode<-u.Value
                | _ ->()
                match n.Attribute(XName.Get("ReferenceCount",XmlData.NS)) with 
                | NotNull u ->
                    match u.Value|>Int32.TryParse with
                    | true,v ->r.ReferenceCount<-v
                    | _ ->()
                | _ ->()
                match n.Attribute(XName.Get("ServiceCodeChangedType",XmlData.NS)) with
                | NotNull u ->
                    match u.Value|>Int32.TryParse with
                    | true , v ->
                        match v with
                        | 0 ->r.ServiceCodeChangedType<-ChangedType.Unchanged
                        | 1 ->r.ServiceCodeChangedType<-ChangedType.Added
                        | 2->r.ServiceCodeChangedType<-ChangedType.Modified
                        | 3->r.ServiceCodeChangedType<-ChangedType.Dirtied
                        | _ ->r.ServiceCodeChangedType<-ChangedType.Unchanged
                    | _ ->()
                | _ ->()
                yield r
        }
        |>Seq.toArray
    | _ ->[||]

  let GetAllAnnotationData(annotationsNode:XElement,serviceInfos:WordServiceInfo[])=
    seq{
      match annotationsNode,serviceInfos with
      | NotNull xa, NotNull xb ->
          for n in xa.Elements() do
            match new WordAnnotation() with
            | r ->
                match n.Attribute(XName.Get("UIElementID",XmlData.NS)) with
                | NotNull y ->
                    match y.Value|>Guid.TryParse with
                    | true, z->r.UIElementID<-z
                    | _ ->()
                | _->()
                match n.Attribute(XName.Get("UIElementName",XmlData.NS)) with
                | NotNull y ->r.UIElementName<-y.Value
                | _->()
                match n.Attribute(XName.Get("UITypeName",XmlData.NS)) with
                | NotNull y ->r.UITypeName<-y.Value
                | _->()
                match n.Attribute(XName.Get("UIName",XmlData.NS)) with
                | NotNull y ->r.UIName<-y.Value
                | _->()
                match n.Attribute(XName.Get("IsInTabItem",XmlData.NS)) with
                | NotNull y ->
                    match y.Value|>Boolean.TryParse with
                    | true, z->r.IsInTabItem<-z
                    | _ ->()
                | _->()
                match n.Attribute(XName.Get("IsTabItem",XmlData.NS)) with
                | NotNull y ->
                    match y.Value|>Boolean.TryParse with
                    | true, z->r.IsTabItem<-z
                    | _ ->()
                | _->()
                match n.Attribute(XName.Get("TabControlNumber",XmlData.NS)) with
                | NotNull y ->
                    match y.Value|>Int32.TryParse with
                    | true, z->r.TabControlNumber<-z
                    | _ ->()
                | _->()
                match n.Attribute(XName.Get("TabItemNumber",XmlData.NS)) with
                | NotNull y ->
                    match y.Value|>Int32.TryParse with
                    | true, z->r.TabItemNumber<-z
                    | _ ->()
                | _->()
                match n.Attribute(XName.Get("TabItemHeader",XmlData.NS)) with
                | NotNull y ->r.TabItemHeader<-y.Value
                | _->()
                match n.Attribute(XName.Get("RequirementDescription",XmlData.NS)) with
                | NotNull y ->r.RequirementDescription<-y.Value
                | _->()
                match n.Attribute(XName.Get("BehaviorDescription",XmlData.NS)) with
                | NotNull y ->r.BehaviorDescription<-y.Value
                | _->()
                match n.Descendants(XName.Get("Services",XmlData.NS)) with
                | za when Seq.length za=1 ->
                    r.WordServiceInfos<-
                      seq{
                        for n in (Seq.head za).Descendants(XName.Get("Service",XmlData.NS)) do
                          match n.Attribute(XName.Get("ServiceName",XmlData.NS)) with
                          | NotNull w ->
                              match xb|>Seq.tryFind(fun a->a.ServiceName=w.Value) with
                              | Some u ->yield u
                              | _ ->()
                          | _ ->()
                      }
                      |>Seq.toArray
                | _ ->()
                yield r
      | _ ->()
    }
    |>Seq.toArray

  let GetAllHistoryUIInfoData(uiInfosNode:XElement)=
    uiInfosNode.Elements()
    |>Seq.map(fun a->
        match new WordUIInfo() with
        | r ->
            match a.Attribute(XName.Get("UITypeName",XmlData.NS)) with
            | NotNull w ->r.UITypeName<-w.Value
            | _ ->()
            r
        )
    |>Seq.toArray

  let GetAllUIInfoData(uis:FrameworkElement[])=
    uis
    |>Seq.map(fun a->
        match new WordUIInfo() with
        | r ->
            r.UITypeName<-a.GetType().Name
            r
        )
    |>Seq.toArray
                    
  let GetAnnotationData(sourceAnnotationData:WordAnnotation[],annotedElements:FrameworkElement[])=
    seq{
      match CommonData.IDProperty, sourceAnnotationData,annotedElements with
      | Some xa,xb,xc ->
          for n in xc do
            match 
              match n.GetValue xa with
              | :? Guid as y ->y
              | _ ->GuidDefaultValue
              with
            | y ->
                match  xb|>Seq.tryFind(fun a->a.UIElementID=y) with
                | Some z ->yield z
                | _ ->yield (new WordAnnotation(UIElementID=y))
      | _ ->()
    }
    |>Seq.toArray
                    
  [<DV>]val mutable _CMD_DocExport:ICommand
  member this.CMD_DocExport
    with get ()=
      if this. _CMD_DocExport=null then
        let usedAnnotations=new Generic.List<WordAnnotation>()
        let tabControlCodes=new Generic.List<int>()
        let syncContext = System.Threading.SynchronizationContext.Current
        this. _CMD_DocExport<-new RelayCommand(
          (fun _->
            this.IsContentProcessing_DocExport<-true
            match new AsyncWorker<_>(async{
              match 
                CommonData.UIAssembly,
                CommonData.IDProperty,
                (
                try
                  match this.GetType().Assembly.GetManifestResourceStream("Template.docx") with
                  | NotNull x ->Some x
                  | _ ->None
                with _ ->None) with
              | Some xa, Some xb, Some xc ->
                  do! Async.SwitchToContext(syncContext)
                  match 
                    xa.GetTypes()
                    |>Seq.choose(fun a->
                        if a.IsSubclassOf typeof<FrameworkElement> then
                          match Activator.CreateInstance(a) with
                          | :? FrameworkElement as ye->CommonData.TryGetUITypeRoot ye
                          | _ ->None
                        else None
                        )
                    |>Seq.toArray
                    with
                  | xaa ->
                     do! Async.SwitchToThreadPool()
                     match 
                       (
                       try
                         match XmlData.XAnnotations,XmlData.XServiceInfos with
                         | Some xr, Some xs ->
                             match 
                               match GetAllServiceInfoData xs with
                               | NotNull yb ->
                                   match GetAllAnnotationData (xr,yb),GetAllUIInfoData xaa with
                                   | NotNull ya,NotNull yc -> Some (ya,yb,yc)
                                   | _ ->None
                               | _ ->None
                               with
                             | Some (ya,yb,yc) ->
                                 for n in yc do
                                   match ya|>Seq.exists(fun a->a.UITypeName=n.UITypeName) with
                                   | true ->
                                       n.IsAnnotated<-true
                                   | _ ->()
                                 match 
                                   match this.D_ComparePath with
                                   | xp when File.Exists xp ->
                                       match XDocument.Load this.D_ComparePath with
                                       | NotNull y->
                                           match 
                                             y.Descendants(XName.Get("Annotations",XmlData.NS)),
                                             y.Descendants(XName.Get("ServiceInfos",XmlData.NS)),
                                             y.Descendants(XName.Get("UIInfos",XmlData.NS))
                                             with
                                           | za,zb,zc when Seq.length za=1 && Seq.length zb=1 && Seq.length zc=1 ->
                                               match GetAllServiceInfoData (Seq.head zb)  with
                                               | NotNull wb  ->
                                                   match GetAllAnnotationData(Seq.head za,wb),GetAllHistoryUIInfoData(Seq.head zc) with
                                                   | NotNull wa, NotNull wc-> Some (wa,wb,wc)
                                                   | _ ->None
                                               | _ ->None
                                           | _ ->None
                                       | _ ->None
                                   | _ ->None
                                   with
                                 | Some (wa,wb,wc)->
                                     for n in ya do
                                       match wa|>Seq.tryFind(fun a->a.UIElementID=n.UIElementID) with
                                       |  Some z ->
                                           match n.UIElementName,z.UIElementName with
                                           | NotNullOrWhiteSpace ua, NotNullOrWhiteSpace ub when ua<>ub->
                                               n.UIElementNameChangedType<- ChangedType.Modified
                                               n.AnnotationChangedType<-ChangedType.Modified
                                           | NotNullOrWhiteSpace ua, NullOrWhiteSpace ->
                                               n.UIElementNameChangedType<-ChangedType.Added
                                               n.AnnotationChangedType<-ChangedType.Modified
                                           | NullOrWhiteSpace, NotNullOrWhiteSpace _ ->
                                               n.UIElementNameChangedType<-ChangedType.Deleted
                                               n.AnnotationChangedType<-ChangedType.Modified
                                           |  _ ->()
                                           match n.RequirementDescription,z.RequirementDescription with
                                           | NotNullOrWhiteSpace ua, NotNullOrWhiteSpace ub when ua<>ub->
                                               n.RequirementDescriptionChangedType<- ChangedType.Modified
                                               n.AnnotationChangedType<-ChangedType.Modified
                                           | NotNullOrWhiteSpace ua, NullOrWhiteSpace ->
                                               n.RequirementDescriptionChangedType<-ChangedType.Added
                                               n.AnnotationChangedType<-ChangedType.Modified
                                           | NullOrWhiteSpace, NotNullOrWhiteSpace _ ->
                                               n.RequirementDescriptionChangedType<-ChangedType.Deleted
                                               n.AnnotationChangedType<-ChangedType.Modified
                                           |  _ ->()
                                           match n.BehaviorDescription,z.BehaviorDescription with
                                           | NotNullOrWhiteSpace ua, NotNullOrWhiteSpace ub when ua<>ub->
                                               n.BehaviorDescriptionChangedType<- ChangedType.Modified
                                               n.AnnotationChangedType<-ChangedType.Modified
                                           | NotNullOrWhiteSpace ua, NullOrWhiteSpace ->
                                               n.BehaviorDescriptionChangedType<-ChangedType.Added
                                               n.AnnotationChangedType<-ChangedType.Modified
                                           | NullOrWhiteSpace, NotNullOrWhiteSpace _ ->
                                               n.BehaviorDescriptionChangedType<-ChangedType.Deleted
                                               n.AnnotationChangedType<-ChangedType.Modified
                                           |  _ ->()
                                           for m in n.WordServiceInfos do
                                             match z.WordServiceInfos|>Seq.tryFind(fun a->a.ServiceName=m.ServiceName) with
                                             | Some w ->
                                                 match m.ServiceDescription,w.ServiceDescription with
                                                 | NotNullOrWhiteSpace ua, NotNullOrWhiteSpace ub when ua<>ub->
                                                     m.ServiceDescriptionChangedType<- ChangedType.Modified
                                                     m.ServiceChangedType<-ChangedType.Modified
                                                     n.AnnotationChangedType<-ChangedType.Modified
                                                 | NotNullOrWhiteSpace ua, NullOrWhiteSpace ->
                                                     m.ServiceDescriptionChangedType<-ChangedType.Added
                                                     m.ServiceChangedType<-ChangedType.Modified
                                                     n.AnnotationChangedType<-ChangedType.Modified
                                                 | NullOrWhiteSpace, NotNullOrWhiteSpace _ ->
                                                     m.ServiceDescriptionChangedType<-ChangedType.Deleted
                                                     m.ServiceChangedType<-ChangedType.Modified
                                                     n.AnnotationChangedType<-ChangedType.Modified
                                                 | _ ->()
                                                 match m.ServiceCode,w.ServiceCode with
                                                 | NotNullOrWhiteSpace ua, NotNullOrWhiteSpace ub when ua<>ub->
                                                     m.ServiceCodeChangedType<- ChangedType.Modified
                                                     m.ServiceChangedType<-ChangedType.Modified
                                                     n.AnnotationChangedType<-ChangedType.Modified
                                                 | NotNullOrWhiteSpace ua, NullOrWhiteSpace ->
                                                     m.ServiceCodeChangedType<-ChangedType.Added
                                                     m.ServiceChangedType<-ChangedType.Modified
                                                     n.AnnotationChangedType<-ChangedType.Modified
                                                 | NullOrWhiteSpace, NotNullOrWhiteSpace _ ->
                                                     m.ServiceCodeChangedType<-ChangedType.Deleted
                                                     m.ServiceChangedType<-ChangedType.Modified
                                                     n.AnnotationChangedType<-ChangedType.Modified
                                                 | _ ->()
                                                 match m.ReferenceCount,w.ReferenceCount with 
                                                 | ua,ub when ua<>ub ->
                                                     m.ReferenceCountChangedType<- ChangedType.Modified
                                                     m.ServiceChangedType<-ChangedType.Modified
                                                     n.AnnotationChangedType<-ChangedType.Modified
                                                 | _ ->()
                                             | _ ->
                                                 m.ServiceChangedType<-ChangedType.Added
                                                 m.ServiceDescriptionChangedType<-ChangedType.Added
                                                 m.ServiceCodeChangedType<-ChangedType.Added
                                       | _ -> 
                                           n.AnnotationChangedType<-ChangedType.Added
                                           n.UIElementNameChangedType<-ChangedType.Added
                                           n.RequirementDescriptionChangedType<-ChangedType.Added
                                           n.BehaviorDescriptionChangedType<-ChangedType.Added
                                           n.ServiceInfosChangedType<-ChangedType.Added
                                     for n in yc do
                                       match wc|>Seq.tryFind (fun a->a.UITypeName=n.UITypeName) with
                                       | None ->n.UIChangedType<-ChangedType.Added
                                       | _ ->()
                                       match ya|>Seq.filter(fun a->a.UITypeName=n.UITypeName)|>Seq.toArray with
                                       | w when w.Length>0 ->
                                           if w|>Array.forall(fun a->a.AnnotationChangedType=ChangedType.Added) then 
                                             n.AnnotationChangedType<-ChangedType.Added
                                           if w|>Array.exists(fun a->a.AnnotationChangedType=ChangedType.Modified) then
                                             n.AnnotationChangedType<-ChangedType.Modified
                                       | _ ->()
                                 | _ ->()
                                 Some (ya,yb,yc)
                             | _ ->None
                         | _ ->None
                       with _ ->None)
                       with
                     | Some (ya,yb,yc) ->
                         try
                           use ms = new MemoryStream()
                           xc.CopyTo ms 
                           xc.Close()
                           let doc=WordprocessingDocument.Open(ms,true)
                           match 
                             doc.MainDocumentPart.Document.Body.Descendants<BookmarkStart>()
                             |>Seq.cast<BookmarkStart>
                             |>Seq.toArray 
                             with
                           | ys when ys.Length>1 ->
                               match ys.[0].NextSibling<Run>() with
                               | NotNull yr ->
                                   yr.GetFirstChild<Text>().Text<-
                                     match XmlData.DocumentTitle with
                                     | NotNullOrWhiteSpace yt ->yt
                                     | _ ->"设计文档"
                               | _ ->()
                               match ys.[1].NextSibling<Run>() with
                               | NotNull yr ->
                                   yr.GetFirstChild<Text>().Text<-"自动创建于"+DateTime.Now.ToString("yyyy-MM-dd")
                               | _ ->()
                           | _ ->()
                           WordHelper.AddHeadingOneParagraph "界面清单"
                           |>doc.MainDocumentPart.Document.Body.AppendChild|>ignore
                           WordHelper.AddUITable yc
                           |>doc.MainDocumentPart.Document.Body.AppendChild|>ignore
                           WordHelper.AddBlankParagraph 2
                           |>Seq.iter (fun c->doc.MainDocumentPart.Document.Body.AppendChild c|>ignore)
                           WordHelper.AddHeadingOneParagraph "服务清单"
                           |>doc.MainDocumentPart.Document.Body.AppendChild|>ignore
                           WordHelper.AddServiceInfoTable yb
                           |>doc.MainDocumentPart.Document.Body.AppendChild|>ignore
                           WordHelper.AddBlankParagraph 2
                           |>Seq.iter (fun c->doc.MainDocumentPart.Document.Body.AppendChild c|>ignore)
                           WordHelper.AddHeadingOneParagraph "界面设计信息"
                           |>doc.MainDocumentPart.Document.Body.AppendChild|>ignore
                           do! Async.SwitchToContext(syncContext)
                           xaa
                           |>Seq.iter(fun a->
                               usedAnnotations.Clear()
                               tabControlCodes.Clear()
                               match yc|>Seq.tryFind(fun b->b.UITypeName=a.GetType().Name), GetTabControls a with
                               | Some wc, wd when wd.Length>0 ->
                                   let rec WriteDocData (tabControlInfos:(TabControl*int*int)[])=
                                     tabControlInfos|>Array.map(fun (_,b,_)->b)|>tabControlCodes.AddRange
                                     tabControlInfos
                                     |>Array.iteri (fun i (n,_,_) ->
                                        n.Items
                                        |>Seq.cast<TabItem>
                                        |>Seq.iteri(fun j b->
                                            match i,j with
                                            | _, 0 when i>0 ->()
                                            | _ ->
                                                b.IsSelected<-true
                                                //ye.UpdateLayout() //必要的,已移植到GetTabControls方法中
                                                match 
                                                  match j with
                                                  | 0 ->true
                                                  | _ ->
                                                      match 
                                                        GetTabControls a
                                                        |>Array.filter(fun (_,c,_)->tabControlCodes|>Seq.exists(fun d->d=c)|>not)
                                                        |>Array.sortBy(fun (_,_,c)-> -c)
                                                        with
                                                      | z when z.Length>0 ->
                                                          WriteDocData z
                                                          false
                                                      | _ ->true
                                                  with
                                                | true ->
                                                    match 
                                                      GetAnnotatedElements a
                                                      |>Seq.filter (fun c->
                                                          usedAnnotations
                                                          |>Seq.exists (fun d->
                                                              match c.GetValue xb with
                                                              | :? Guid as z ->z=d.UIElementID
                                                              | _ ->false
                                                              )
                                                          |>not
                                                          )
                                                      |>Seq.toArray
                                                      with
                                                    | z ->
                                                        match GetAnnotationData(ya,z) with
                                                        | w ->
                                                            match GetRenderedImage(a,z,w) with
                                                            | Some (ua,ub,uc) ->
                                                                match b.Header with
                                                                | NotNull u ->u.ToString()
                                                                | _ ->"未命名"
                                                                |>WordHelper.AddHeadingThreeParagraph
                                                                |>doc.MainDocumentPart.Document.Body.AppendChild|>ignore
                                                                WordHelper.InsertImage(doc,ua,ub,uc,ImagePartType.Png)|>ignore
                                                                WordHelper.AddBlankParagraph 1
                                                                |>Seq.iter (fun c->doc.MainDocumentPart.Document.Body.AppendChild c|>ignore)
                                                                WordHelper.AddAnnotationTable w
                                                                |>doc.MainDocumentPart.Document.Body.AppendChild|>ignore
                                                                WordHelper.AddBlankParagraph 1
                                                                |>Seq.iter (fun c->doc.MainDocumentPart.Document.Body.AppendChild c|>ignore)
                                                            | _ ->()
                                                            usedAnnotations.AddRange w
                                                | _ ->()
                                          )
                                     )
                                   WordHelper.AddHeadingTwoParagraph wc.UITypeName 
                                   |>doc.MainDocumentPart.Document.Body.AppendChild|>ignore
                                   WriteDocData wd
                               | Some wc, _ ->
                                   match GetAnnotatedElements a with
                                   | z ->
                                       match GetAnnotationData(ya,z) with
                                       | w ->
                                           match GetRenderedImage(a,z,w) with
                                           | Some (ua,ub,uc) ->
                                               WordHelper.AddHeadingTwoParagraph wc.UITypeName 
                                               |>doc.MainDocumentPart.Document.Body.AppendChild|>ignore
                                               WordHelper.InsertImage(doc,ua,ub,uc,ImagePartType.Png)|>ignore
                                               WordHelper.AddBlankParagraph 1
                                               |>Seq.iter (fun c->doc.MainDocumentPart.Document.Body.AppendChild c|>ignore)
                                               WordHelper.AddAnnotationTable w
                                               |>doc.MainDocumentPart.Document.Body.AppendChild|>ignore
                                               WordHelper.AddBlankParagraph 1
                                               |>Seq.iter (fun c->doc.MainDocumentPart.Document.Body.AppendChild c|>ignore)
                                           | _ ->()
                                           usedAnnotations.AddRange w
                               | _ ->()
                               )
                           do! Async.SwitchToThreadPool()
                           match doc.MainDocumentPart.GetPartsOfType<DocumentSettingsPart>() with
                           | xu when Seq.length xu>0 ->
                               match xu|>Seq.head, new UpdateFieldsOnOpen() with
                               | settingsPart,updateFields ->
                                   updateFields.Val<- new DocumentFormat.OpenXml.OnOffValue(true)
                                   settingsPart.Settings.PrependChild<UpdateFieldsOnOpen>(updateFields)|>ignore
                                   settingsPart.Settings.Save()
                           | _ ->()
                           use fs = new FileStream(this.D_DocPath, FileMode.Create) 
                           doc.Close() //关闭时才能全部保存
                           ms.WriteTo(fs) 
                           return true
                         with _ ->return false
                     | _ ->return false
              | _ -> return false
              }) with
            | wk ->
                wk.Completed.Add (fun r->
                  this.IsContentProcessing_DocExport<-false
                  match r with
                  | true -> MessageBox.Show("设计文档导出成功","操作提示",MessageBoxButton.OK,MessageBoxImage.Information)|>ignore
                  | _ -> MessageBox.Show("设计文档导出失败","操作提示",MessageBoxButton.OK,MessageBoxImage.Error)|>ignore
                  )
                wk.RunAsync()|>ignore
            ),
          (fun _ ->
            (File.Exists this.D_ComparePath || String.IsNullOrWhiteSpace this.D_ComparePath) &&
            String.IsNullOrWhiteSpace this.D_DocPath|>not && 
            this.IsContentProcessing_DocExport|>not))
      this. _CMD_DocExport  

  [<DV>]val mutable _CMD_DataCleaning:ICommand
  member this.CMD_DataCleaning
    with get ()=
      if this. _CMD_DataCleaning=null then
        let syncContext = System.Threading.SynchronizationContext.Current
        this. _CMD_DataCleaning<-new RelayCommand(
          (fun _->
            this.IsContentProcessing_DataCleaning<-true
            match new AsyncWorker<_>(async{
              try
                match CommonData.UIAssembly, (XmlData.FilePath,XmlData.XServiceInfos, XmlData.XAnnotations,XmlData.XDoc) with
                | Some xa, (NotNullOrWhiteSpace xb,Some xc, Some xd, Some xe)->
                    do! Async.SwitchToContext(syncContext)
                    match 
                      xa.GetTypes()
                      |>Seq.choose(fun a->
                          if a.IsSubclassOf typeof<FrameworkElement> then
                            match Activator.CreateInstance(a) with
                            | :? FrameworkElement as ya ->Some ya
                            | _ ->None
                          else None
                          )
                      |>Seq.toArray
                      with
                    | xa ->
                        match 
                          GetAllServiceInfoData xc, 
                          xa|>Seq.collect(fun a->GetAnnotationIDs a)|>Seq.toArray with
                        | ya,  yb ->
                            do! Async.SwitchToThreadPool()
                            GetAllAnnotationData (xd,ya)
                            |>Seq.filter (fun a->yb|>Seq.exists(fun b->b=a.UIElementID)|>not)
                            |>Seq.toArray
                            |>Seq.iter(fun a->
                                match 
                                  xd.Elements(XName.Get("Annotation",XmlData.NS))
                                  |>Seq.tryFind(fun b->
                                      match b.Attribute(XName.Get("UIElementID",XmlData.NS)) with
                                      | NotNull za ->
                                          match za.Value|>Guid.TryParse with
                                          | true, zb ->zb=a.UIElementID
                                          | _ ->false
                                      | _ ->false
                                      )
                                  with
                                | Some za ->
                                    match za.Descendants(XName.Get("Services",XmlData.NS)) with
                                    | zb when Seq.length zb=1 ->
                                        match (Seq.head zb).Descendants(XName.Get("Service",XmlData.NS)) with
                                        | zc  when Seq.length zc>0 ->
                                            match xc.Elements(XName.Get("ServiceInfo",XmlData.NS)) with
                                            | zd when Seq.length zd>0 ->
                                                for n in zc do
                                                  match n.Attribute(XName.Get("ServiceName",XmlData.NS)) with
                                                  | NotNull ze ->
                                                      match
                                                        zd
                                                        |>Seq.tryFind(fun a->
                                                            match a.Attribute(XName.Get("ServiceName",XmlData.NS)) with
                                                            | NotNull zf ->zf.Value=ze.Value
                                                            | _ ->false
                                                            )
                                                        with
                                                      | Some zf ->
                                                          match zf.Attribute(XName.Get("ReferenceCount",XmlData.NS)) with 
                                                          | NotNull zg ->
                                                              match zg.Value|>Int32.TryParse with
                                                              | true,zh ->
                                                                  zf.SetAttributeValue(XName.Get("ReferenceCount",XmlData.NS),zh-1)
                                                              | _ ->()
                                                          | _ ->()
                                                      | _ ->()
                                                  | _ ->()
                                            | _ ->()
                                        | _ ->()
                                    | _ ->()
                                    za.Remove()|>ignore
                                | _ ->()
                                )
                            xe.Save xb
                            return true
                | _ ->return false
              with _ ->return false
              }) with
            | wk ->
                wk.Completed.Add (fun r->
                  this.IsContentProcessing_DataCleaning<-false
                  match r with
                  | true -> MessageBox.Show("数据清理成功！","操作提示",MessageBoxButton.OK,MessageBoxImage.Information)|>ignore
                  | _ -> MessageBox.Show("数据清理失败","操作提示",MessageBoxButton.OK,MessageBoxImage.Error)|>ignore
                  )
                wk.RunAsync()|>ignore
            ),
          (fun _ ->this.IsContentProcessing_DataCleaning|>not))
      this. _CMD_DataCleaning  

(*
  let GetAllHistoryUIInfoData(xdoc:XDocument)=
    match xdoc with
    | NotNull x ->
        match x.Descendants(XName.Get("UIInfos",XmlData.NS)) with
        | y when Seq.length y=1 ->
            match Seq.head y with
            | z ->
                z.Elements()
                |>Seq.map(fun a->
                    match new WordUIInfo() with
                    | r ->
                        match a.Attribute(XName.Get("UITypeName",XmlData.NS)) with
                        | NotNull w ->r.UITypeName<-w.Value
                        | _ ->()
                        r
                    )
                |>Seq.toArray
        | _ ->[||]
    | _ ->[||]
*)