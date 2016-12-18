namespace WX.Utilities.WPFDesignerX.Windows.Design

open System
open System.Xml.Linq
open System.Windows
open System.ComponentModel 
open System.Windows.Controls
open System.Windows.Data
open System.Windows.Media
open Microsoft.Windows.Design.Interaction
open Microsoft.Windows.Design.Model
open Microsoft.Windows.Design.Metadata
open WX
open WX.Utilities.WPFDesignerX.Common
open WX.Utilities.WPFDesignerX.Windows


type AnnotationControllerAdornerProvider()=
  inherit PrimarySelectionAdornerProvider()
  let mutable _ModelItem:ModelItem option=None
  let mutable _ModelEditingScope:ModelEditingScope option=None
  let mutable _AdornerPanel=None
  let mutable _CheckAnnotation=None

  member this.InitializeAnnotationControllerAdorner()= 
    _AdornerPanel<-new AdornerPanel()|>Some
    _CheckAnnotation<-new CheckBox()|>Some
    AdornerPanel.SetAdornerHorizontalAlignment( _CheckAnnotation.Value, AdornerHorizontalAlignment.Left)
    AdornerPanel.SetAdornerVerticalAlignment(_CheckAnnotation.Value, AdornerVerticalAlignment.OutsideTop)
    AdornerPanel.SetAdornerMargin(_CheckAnnotation.Value, new Thickness(0.0, 1.0, 0.0, 0.0));
    _AdornerPanel.Value.Children.Add _CheckAnnotation.Value|>ignore 
    this.Adorners.Add _AdornerPanel.Value
    _CheckAnnotation.Value.Loaded.Add(fun _ ->_CheckAnnotation.Value.IsChecked<-this.GetAnnotationSetedStatus())
    _CheckAnnotation.Value.MouseLeftButtonDown.Add(fun _ -> 
      match _ModelItem with
      | Some y ->_ModelEditingScope<-Some <| y.BeginEdit()
      | _ ->()
      )
    _CheckAnnotation.Value.MouseLeftButtonUp.Add(fun _ ->
      match _ModelEditingScope with
      | Some y ->
          y.Complete()
          y.Dispose()
          _ModelEditingScope<-None
      | _ ->()
      )
    _CheckAnnotation.Value.Checked|>Observable.map (fun _->true)
    |>Observable.merge (_CheckAnnotation.Value.Unchecked|>Event.map (fun _ ->false))
    |>Observable.filter (fun _ ->_ModelItem.IsSome)
    |>Observable.add (fun e ->
        match e, _ModelItem.Value.Properties.[Extension.IDPropertyIdentifier] with
        | true,x ->
            match 
              match x.IsSet with
              | true ->
                  match x.ComputedValue with
                  | :? Guid as y when y>GuidDefaultValue ->false
                  | _ ->true
              | _ ->true
              with
            |  true ->x.SetValue (Guid.NewGuid())|>ignore
            | _ ->()
        | _,x ->
            match x.ComputedValue with
            | :? Guid as xa when xa>GuidDefaultValue ->
                match MessageBox.Show("标注数据将被删除，是否继续","删除提示",MessageBoxButton.YesNo,MessageBoxImage.Warning) with
                | MessageBoxResult.Yes ->
                    match XmlData.XDoc with
                    | Some xb ->
                        match xb.Descendants(XName.Get("Annotations",XmlData.NS)) with
                        | xc when Seq.length xc=1 ->
                            match 
                              (Seq.head xc).Elements(XName.Get("Annotation",XmlData.NS))
                              |>Seq.tryFind(fun a->
                                  match a.Attribute(XName.Get("UIElementID",XmlData.NS)) with
                                  | NotNull xd ->
                                      match xd.Value|>Guid.TryParse with
                                      | true, xe ->xe=xa
                                      | _ ->false
                                  | _ ->false
                                  )
                              with
                            | Some z ->
                                match z.Descendants(XName.Get("Services",XmlData.NS)) with
                                | w when Seq.length w=1 ->
                                    match (Seq.head w).Descendants(XName.Get("Service",XmlData.NS)) with
                                    | u  when Seq.length u>0 ->
                                        match xb.Descendants(XName.Get("ServiceInfos",XmlData.NS)) with
                                        | v when Seq.length v=1 ->
                                            match (Seq.head v).Elements(XName.Get("ServiceInfo",XmlData.NS)) with
                                            | xw when Seq.length xw>0 ->
                                                for n in u do
                                                  match n.Attribute(XName.Get("ServiceName",XmlData.NS)) with
                                                  | NotNull xu ->
                                                      match
                                                        xw
                                                        |>Seq.tryFind(fun a->
                                                            match a.Attribute(XName.Get("ServiceName",XmlData.NS)) with
                                                            | NotNull xv ->xv.Value=xu.Value
                                                            | _ ->false
                                                            )
                                                        with
                                                      | Some xv ->
                                                          match xv.Attribute(XName.Get("ReferenceCount",XmlData.NS)) with 
                                                          | NotNull yw ->
                                                              match yw.Value|>Int32.TryParse with
                                                              | true,yu ->
                                                                  xv.SetAttributeValue(XName.Get("ReferenceCount",XmlData.NS),yu-1)
                                                              | _ ->()
                                                          | _ ->()
                                                      | _ ->()
                                                  | _ ->()
                                            | _ ->()
                                        | _ ->()
                                    | _ ->()
                                | _ ->()
                                z.Remove()|>ignore
                                xb.Save XmlData.FilePath
                            | _ ->()
                        | _ ->()
                    | _ ->()
                    x.ClearValue()
                | _ ->
                    _CheckAnnotation.Value.IsChecked<-Nullable<_> true
            | _ ->()
        )   

  member this.UpdateAnnotationControllerStatus()=
    match _CheckAnnotation,_AdornerPanel,_ModelItem with
    | Some xa,Some xb,Some xc ->
        match xc.Root.Properties.[Extension.IsUITypeRootPropertyIdentifier] with
        | y when y.IsSet ->  
          match y.ComputedValue with
          | :? bool as z when z -> xa.IsEnabled<-true
          | _ ->xa.IsEnabled<-false
        | _ ->xa.IsEnabled<-false
        if xa.IsEnabled then
          xa.Cursor<-Input.Cursors.Hand
          xa.SetValue(ToolTipService.ShowOnDisabledProperty,false)
          xa.ToolTip<-"设置标注"
        else 
          xb.Cursor<-Input.Cursors.Help
          xa.SetValue(ToolTipService.ShowOnDisabledProperty,true)
          xa.ToolTip<-"要控制标注，请先启用界面"
    | _ ->()

  member private this.GetAnnotationSetedStatus()=
    match _ModelItem with
    | Some x ->
        match x.Properties.[Extension.IDPropertyIdentifier].ComputedValue with
        | :? Guid as y -> Nullable<_>(y<>GuidDefaultValue)
        | _ ->new Nullable<_>()
    | _ ->new Nullable<_>()

  member private this.ModelItem_AnnotationControllerChanged=
    new PropertyChangedEventHandler(fun o e ->
      match e.PropertyName with
      | "ID" ->
          match _CheckAnnotation with
          | Some x ->x.IsChecked<-this.GetAnnotationSetedStatus()
          | _ ->()
      | "IsUITypeRoot" ->
          this.UpdateAnnotationControllerStatus()
      | _ ->()
      )

  override this.Activate (item:ModelItem)=
    if not XmlData.IsDesignAudited && XmlData.IsDisplayController then
      _ModelItem<-Some item
      _ModelItem.Value.PropertyChanged.AddHandler this.ModelItem_AnnotationControllerChanged
      this.InitializeAnnotationControllerAdorner()
      this.UpdateAnnotationControllerStatus()
    base.Activate(item) 

  override this.Deactivate()=
    match _AdornerPanel with
    | Some x->x.Children.Clear()
    | _ ->()
    match _ModelItem with
    | Some x ->
        x.PropertyChanged.RemoveHandler this.ModelItem_AnnotationControllerChanged
    | _ ->()
    base.Deactivate()


//=================================
(*
DesignModeValueProvider Class
http://msdn.microsoft.com/en-us/library/microsoft.windows.design.model.designmodevalueprovider.aspx

ModelItem Class
http://msdn.microsoft.com/en-us/library/microsoft.windows.design.model.modelitem.aspx

ModelEditingScope Class
http://msdn.microsoft.com/en-us/library/microsoft.windows.design.model.modeleditingscope.aspx

AttributeTableBuilder 类
http://msdn.microsoft.com/zh-cn/library/microsoft.windows.design.metadata.attributetablebuilder(v=vs.90).aspx

How to: Create a Surrogate Policy,PropertyIdentifier pi = new PropertyIdentifier( typeof( DockPanel ), "Dock" );
http://msdn.microsoft.com/zh-cn/library/microsoft.windows.design.metadata.attributetablebuilder(v=vs.90).aspx
WPF and Silverlight Designer Extensibility Samples 
http://archive.msdn.microsoft.com/DesignerExtensbility

 How to update XAML with changes to an attached property during creation
http://social.msdn.microsoft.com/forums/is/blend/thread/47671fe1-9537-4262-ab82-b10554cb494d

Showing Attached Properties in the Cider WPF Designer
http://blogs.msdn.com/b/jnak/archive/2008/01/17/showing-attached-properties-in-the-cider-wpf-designer.aspx

[WPF / MVVM] How to get data in “design time” ?
http://weblogs.asp.net/thomaslebrun/archive/2009/05/04/wpf-mvvm-how-to-get-data-in-design-time.aspx

程序参考
_CheckAnnotation.Value.MouseEnter.Add(fun _ ->_CheckAnnotation.Value.Cursor<-Input.Cursors.Hand)

WinForm下参考
namespace WX.Utilities.WPFDesignerX.Windows.Controls
open System
open System.Drawing.Design
open System.ComponentModel
open WX.Utilities.WPFDesignerX.Windows

type ControlBusinessUITypeEditor()=
  inherit UITypeEditor()
  let _GuidDefaultValue=new Guid()
  override this.EditValue (context:ITypeDescriptorContext,provider:IServiceProvider,value:obj)=
    match new UC_BusinessEditor(),new FVM_BusinessEditor() with
    | v, vm ->
        v.DataContext<-vm
        match value with
        | :? Guid as z when z>_GuidDefaultValue->()  //修改

        | _ -> //新增
            vm.UIElementID<-Guid.NewGuid()
        match v.ShowDialog() with
        | x when x.HasValue && x.Value ->
            box vm.UIElementID
        | _ ->base.EditValue(context, provider, value)

  override this.GetEditStyle(typeDescriptorContext:ITypeDescriptorContext)=
    UITypeEditorEditStyle.Modal 
  *)