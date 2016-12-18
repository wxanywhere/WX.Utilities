namespace WX.Utilities.WPFDesignerX.Windows

open System
open System.IO
open System.ComponentModel
open System.Windows
open System.Windows.Controls
open System.Windows.Input
open System.Windows.Media
open System.Windows.Media.Imaging
open System.Windows.Documents
open System.Xml.Linq
open WX
open WX.Utilities.WPFDesignerX.Common
open WX.Utilities.WPFDesignerX.BusinessEditor

(*
Adorners Overview
http://msdn.microsoft.com/en-us/library/ms743737.aspx
*)
type AnnotationAdorner(adornedElement:FrameworkElement,elementID:Guid,?actualElement:FrameworkElement) as this=
  inherit Adorner(adornedElement)
  static let BitmapSource=new BitmapImage(new Uri("pack://application:,,,/WX.Utilities.WPFDesignerX.Windows;component/Resources/BusinessEditor.gif",UriKind.Absolute))
  static let mutable EditorViewIns:MainWindow option=None
  static let mutable EditorViewModelIns:FVM_MainWindow option=None //View的DataContext可能会丢失
  do
    match CommonData.IsInDesignTime with
    | x when x || (not x && XmlData.IsDisplayAnnotation) ->
        adornedElement.IsVisibleChanged.Add(fun _ ->this.InvalidateVisual()) //必须的，当控件在Design-Time不可见时(包括Designer窗口切换及控件Visibility属改变等情况)，即时渲染Adorner,以保证Draw的内容同步保持可见性
        this._UIElementID<-elementID
        this.IsClipEnabled<-true //必须的，否则Adorner 绘制的图形将会覆盖于Designer Window之上，启用后性能开销较大
        this.ClipToBounds<-true
        this.VisualBitmapScalingMode <- BitmapScalingMode.NearestNeighbor
        this.MouseEnter.Add(fun e->
          this.Cursor<-Cursors.Hand 
          if x then this.ToolTip<-"设计模式下，只能查看标注"
          else if (this.TryGetUITypeRoot adornedElement).IsNone then
            this.ToolTip<-"界面未启用，只能查看标注"
          )
        this.MouseLeftButtonDown.Add(fun e->
          CommonData.IsAnnotationDataDirty<-true //使得重新加载界面时，数据得以刷新, 以确保设计时和运行时的数据能够基本保持同步
          match 
            if CommonData.IsInDesignTime && XmlData.XDoc.IsNone then 
              DataHelper.RetrieveAnnotationData(true)
            else true
            with
          | true ->
              match 
                match EditorViewIns,EditorViewModelIns with
                | Some v,Some vm ->
                    v,vm
                | _ ->
                    match new MainWindow(), new FVM_MainWindow() with
                    | v, vm ->
                        EditorViewModelIns<-Some vm
                        vm.MainWindow<-v
                        EditorViewIns<-Some v
                        match Application.Current.MainWindow with
                        | :? MainWindow ->() 
                        | u ->
                            u.Closing.Add (fun e->
                              match XmlData.DisplayAnnotationRestoreFlag with
                              | true ->
                                  match XmlData.XOption with
                                  | Some x ->
                                      x.SetAttributeValue(XName.Get("IsDisplayAnnotation",XmlData.NS),string true)
                                      XmlData.SaveXDoc()|>ignore
                                      XmlData.IsDisplayAnnotation<-true
                                  | _ ->()
                              | _ ->()
                              e.Cancel<-false
                              Application.Current.Shutdown()
                              )
                        v.Closing.Add(fun e->
                          e.Cancel<-true
                          v.Visibility<-Visibility.Hidden
                          )
                        v.DataContext<-vm
                        v,vm
                with
              | v,vm ->
                  match adornedElement,new Annotation() with
                  | y,z ->
                      vm.Annotation<-z
                      z.UIElementID<-this.UIElementID
                      z.UIElementName<-
                        match actualElement with
                        | Some w ->w.GetType().Name
                        | _ ->y.GetType().Name
                      match CommonData.IsInDesignTime, this.TryGetUITypeRoot y with
                      | false,Some w ->
                          CommonData.UIAssembly<-w.GetType().Assembly|>Some
                          CommonData.EditorOperateScope<-EditorOperateScope.All
                          z.UITypeName<-w.GetType().Name
                      | false,_ ->
                          CommonData.EditorOperateScope<-EditorOperateScope.View
                      | _ ->
                          CommonData.EditorOperateScope<-EditorOperateScope.View
                      z.IsInTabItem<-this.HasTabItemParent y
                      z.IsTabItem<-y :? TabItem
                      vm.Initialize()
                      v.WindowStartupLocation<-WindowStartupLocation.Manual
                      match this.PointToScreen(e.GetPosition this) with
                      | w ->
                          match 
                            if v.ActualHeight=0.0 || v.ActualWidth=0.0 then v.Height,v.Width
                            else v.ActualHeight,v.ActualWidth
                            with
                          | vh,vw ->
                              v.Top<-
                                if w.Y+5.0+vh>SystemParameters.WorkArea.Height then
                                  SystemParameters.WorkArea.Height-vh
                                else w.Y+5.0
                              v.Left<-
                                if w.X+5.0+vw >SystemParameters.WorkArea.Width then
                                  SystemParameters.WorkArea.Width-vw
                                else w.X+5.0
                      //v.Topmost<-true 正式发布时应该启用
                      if v.Visibility=Visibility.Hidden || v.Visibility=Visibility.Collapsed then
                        v.ShowDialog()|>ignore
                      else 
                        v.WindowState<-WindowState.Normal
                        v.Activate()|>ignore
          | _ ->()
          )
    | _ ->()

  [<DV>]val mutable private _UIElementID:Guid
  member this.UIElementID
    with get()=this._UIElementID
    and set v=this._UIElementID<-v

  member this.HasTabItemParent (element:FrameworkElement)=
    match element with
    | NotNull x ->
        let rec TryGetParentTabItem (visual:Visual)=
          match VisualTreeHelper.GetParent(visual) with
          | :? TabItem as y -> Some y
          | :? Visual as y ->TryGetParentTabItem y
          | _ ->None
        match TryGetParentTabItem x with
        | Some _ ->true
        | _ ->false
    | _ ->
        raise <| new ArgumentNullException("element")

  //类似于PresentationSource.FromDependencyObject(element)
  member this.GetRootVisual(element:FrameworkElement)=
    match element with
    | NotNull x ->
        let rec GetRootVisual (visual:Visual)=
          match VisualTreeHelper.GetParent visual with
          | NotNull y -> y:?>Visual|>GetRootVisual
          | _ ->visual
        GetRootVisual x
    | _ ->
        raise <| new ArgumentNullException("element")

  member this.TryGetUITypeRoot(element:FrameworkElement):FrameworkElement option=
    match element,CommonData.IsUITypeRootProperty with
    | NotNull xa, Some xb ->
        let rec TryGetUITypeRoot (visual:Visual)=
          match visual with
          | :? FrameworkElement as y ->
              match y.GetValue xb with
              | :? bool as z when z ->Some y
              | _ ->VisualTreeHelper.GetParent y|>unbox<Visual>|>TryGetUITypeRoot
          | NotNull y -> VisualTreeHelper.GetParent y|>unbox<Visual>|>TryGetUITypeRoot
          | _ ->None
        TryGetUITypeRoot xa
    | _ ->
        raise <| new ArgumentNullException("element")

  //用于DesignTime下Custom Root Visual 的获取
  member this.TryGetCustomRootVisual(rootVisual:Visual)=
    match rootVisual with
    | NotNull x ->
        let rec GetCustomRootVisual(visual:Visual)=
          seq{
            match visual with
            | :? Window as y ->yield y:>FrameworkElement
            | :? UserControl as y->yield y:>FrameworkElement
            | NotNull y ->
                for n in 0..(VisualTreeHelper.GetChildrenCount y)-1 do
                  yield! VisualTreeHelper.GetChild (y,n):?>Visual|>GetCustomRootVisual
            | _ ->()
          }
        GetCustomRootVisual x|>Seq.tryFind (fun _ ->true)
    | _ ->
        raise <| new ArgumentNullException("rootVisual")

  override this.OnRender(drawingContext:DrawingContext )=
    match BitmapSource,adornedElement,CommonData.IsInDesignTime with
    | x, y, z when (z || (not z && XmlData.IsDisplayAnnotation)) && y.IsVisible && y.Visibility=Visibility.Visible ->
        match 
          match actualElement with
          | Some _ ->this.ActualWidth-x.Width+y.Margin.Right,1.0-y.Margin.Top
          | _ ->this.ActualWidth-x.Width,1.0
          with
        | wa,wb ->
            match new Rect(wa, wb, x.Width, x.Height) with
            | u ->
                drawingContext.DrawImage(BitmapSource,u)
    | _ ->()



//=======================================
(*
//参考，不适用于嵌套UserControl的设计
member this.TryGetUIContainer(element:FrameworkElement)=
  match element with
  | NotNull x ->
      let rec TryGetUIContainer (visual:Visual)=
        match visual with
        | :? Window as y -> y:>FrameworkElement|>Some
        | :? UserControl as y ->y:>FrameworkElement|>Some
        | NotNull y ->VisualTreeHelper.GetParent y|>unbox<Visual>|>TryGetUIContainer
        | _ ->None
      TryGetUIContainer x
  | _ ->
      raise <| new ArgumentNullException("element")

//获取RootElement
Something along these lines should work: 
1. PresentationSource.FromVisual(wpfControl) -> ElementHost's internal HwndSource 
2. HwndSource.Handle -> native window's handle (hwndWpf)
3. Win32.GetParent(hwndWpf) -> ElementHost's window (hElementHost) 
4. System.Windows.Forms.Control.FromHandle(hElementHost) -> ElementHost


//for f# 3.0
member val UIElementID=GuidDefaultValue with get, set

let mutable regionX:float option=None
let mutable regionY:float option=None
  this.MouseEnter
  |>Observable.filter (fun e->
      match regionX, regionY with
      | Some x, Some y ->
          match e.GetPosition(adornedElement) with
          | z ->z.X>=x && z.Y<=y
      | _ ->false
      )
  |>Observable.add(fun e->
      this.Cursor<-Cursors.Hand 
      )
//---------------------------------------------------
//Right reference
type AnnotationAdorner=
  inherit Adorner
  val mutable private _UIElementID:Guid
  new (adornedElement:UIElement,elementID:Guid) as this =
    { inherit Adorner(adornedElement); _UIElementID=elementID } then
      this.MouseEnter.Add(fun e->
        this.Cursor<-Cursors.Hand 
        )
      this.MouseLeftButtonDown.Add(fun e->
        match new UC_BusinessEditor(),new FVM_BusinessEditor() with
        | v, vm ->
            v.DataContext<-vm
            match this.UIElementID with
            | x when x>GuidDefaultValue ->
                vm.UIElementID<-x
                vm.EditStatus<-EditStatus.Modify
            | _ ->
                vm.UIElementID<-Guid.NewGuid()
                vm.EditStatus<-EditStatus.Add
            v.ShowDialog()|>ignore
        )
  member this.UIElementID
    with get()=this._UIElementID
    and set v=this._UIElementID<-v

  override this.MeasureOverride(constr:Size)=
    this.InvalidateVisual()
    base.MeasureOverride constr 

  override this.ArrangeOverride(finalSize:Size)=
    this.InvalidateVisual()
    base.ArrangeOverride finalSize

    this.VisualCacheMode<-new BitmapCache()
    this.VisualBitmapScalingMode <- BitmapScalingMode.NearestNeighbor;
    RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor)
    RenderOptions.SetEdgeMode(this, EdgeMode.Aliased)
*)