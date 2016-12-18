namespace WX.Utilities.WPFDesignerX.Windows.Design

open System
open System.Windows
open System.ComponentModel 
open System.Windows.Controls
open Microsoft.Windows.Design.Interaction
open Microsoft.Windows.Design.Model
open Microsoft.Windows.Design.Metadata
open WX
open WX.Utilities.WPFDesignerX.Windows

type BusinessEditorAdornerProvider() as this=
  inherit PrimarySelectionAdornerProvider()
  let mutable _ModelItem:ModelItem option=None
  let mutable _ModelEditingScope:ModelEditingScope option=None
  let mutable _AdornerPanel=new AdornerPanel()
  let mutable _ChkEditor=new CheckBox()

  do
    AdornerPanel.SetAdornerHorizontalAlignment( _ChkEditor, AdornerHorizontalAlignment.Stretch);
    AdornerPanel.SetAdornerVerticalAlignment(_ChkEditor, AdornerVerticalAlignment.OutsideTop);
    AdornerPanel.SetAdornerMargin(_ChkEditor, new Thickness(0.0, 0.0, 0.0, 10.0));
    _AdornerPanel.Children.Add _ChkEditor|>ignore 
    this.Adorners.Add _AdornerPanel
    _ChkEditor.Loaded.Add(fun _ ->_ChkEditor.IsChecked<-this.GetEditorEnabledStatus())
    _ChkEditor.MouseEnter.Add(fun _ ->_ChkEditor.Cursor<-Input.Cursors.Hand)
    _ChkEditor.MouseLeftButtonDown.Add(fun _ -> 
      match _ModelItem with
      | Some x ->_ModelEditingScope<-Some <| x.BeginEdit()
      | _ ->()
      )
    _ChkEditor.MouseLeftButtonUp.Add(fun _ ->
      match _ModelEditingScope with
      | Some x ->
          x.Complete()
          x.Dispose()
          _ModelEditingScope<-None
      | _ ->()
      )
    _ChkEditor.Checked|>Observable.map (fun _->true)
    |>Observable.merge (_ChkEditor.Unchecked|>Event.map (fun _ ->false))
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
            x.ClearValue()
        )   

  member private this.GetEditorEnabledStatus()=
    match _ModelItem with
    | Some x ->
        match x.Properties.[Extension.IDPropertyIdentifier].ComputedValue with
        | :? Guid as y -> Nullable<_>(y<>GuidDefaultValue)
        | _ ->new Nullable<_>()
    | _ ->new Nullable<_>()

  member private this.ModelItem_PropertyChanged=
    new PropertyChangedEventHandler(fun o e ->
      //(*
      match e.PropertyName with
      | "ID" ->_ChkEditor.IsChecked<-this.GetEditorEnabledStatus()
      | _ ->()
      //*)
      )

  override this.Activate (item:ModelItem)=
    _ModelItem<-Some item 
    _ModelItem.Value.PropertyChanged.AddHandler this.ModelItem_PropertyChanged
    base.Activate(item) 

  override this.Deactivate()=
    _AdornerPanel.Children.Clear()
    _ModelItem.Value.PropertyChanged.RemoveHandler this.ModelItem_PropertyChanged
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
*)

(* WinForm下参考
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