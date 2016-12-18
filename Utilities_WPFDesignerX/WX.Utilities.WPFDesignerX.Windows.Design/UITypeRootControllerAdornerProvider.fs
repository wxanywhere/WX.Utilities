namespace WX.Utilities.WPFDesignerX.Windows.Design

open System
open System.Windows
open System.ComponentModel 
open System.Windows.Controls
open System.Windows.Data
open Microsoft.Windows.Design.Interaction
open Microsoft.Windows.Design.Model
open Microsoft.Windows.Design.Metadata
open WX
open WX.Utilities.WPFDesignerX.Common
open WX.Utilities.WPFDesignerX.Windows

type UITypeRootControllerAdornerProvider()=
  inherit PrimarySelectionAdornerProvider()
  let mutable _ModelItem:ModelItem option=None
  let mutable _ModelEditingScope:ModelEditingScope option=None
  let mutable _AdornerPanel=None
  let mutable _CheckUITypeRoot=None

  member this.InitializeUITypeRootControllerAdorner()= 
    _AdornerPanel<-new AdornerPanel()|>Some
    _CheckUITypeRoot<-new CheckBox(Content="启用界面")|>Some
    AdornerPanel.SetAdornerHorizontalAlignment( _CheckUITypeRoot.Value, AdornerHorizontalAlignment.Left)
    AdornerPanel.SetAdornerVerticalAlignment(_CheckUITypeRoot.Value, AdornerVerticalAlignment.OutsideTop)
    AdornerPanel.SetAdornerMargin(_CheckUITypeRoot.Value, new Thickness(60.0, 1.0, 0.0, 0.0));
    _AdornerPanel.Value.Children.Add _CheckUITypeRoot.Value|>ignore 
    this.Adorners.Add _AdornerPanel.Value
    _CheckUITypeRoot.Value.Loaded.Add(fun _ ->_CheckUITypeRoot.Value.IsChecked<-this.GetUITypeRootSetedStatus())
    _CheckUITypeRoot.Value.MouseEnter.Add(fun _ ->_CheckUITypeRoot.Value.Cursor<-Input.Cursors.Hand)
    _CheckUITypeRoot.Value.MouseLeftButtonDown.Add(fun _ -> 
      match _ModelItem with
      | Some x ->_ModelEditingScope<-Some <| x.BeginEdit()
      | _ ->()
      )
    _CheckUITypeRoot.Value.MouseLeftButtonUp.Add(fun _ ->
      match _ModelEditingScope with
      | Some x ->
          x.Complete()
          x.Dispose()
          _ModelEditingScope<-None
      | _ ->()
      )
    _CheckUITypeRoot.Value.Checked|>Observable.map (fun _->true)
    |>Observable.merge (_CheckUITypeRoot.Value.Unchecked|>Event.map (fun _ ->false))
    |>Observable.filter (fun _ ->_ModelItem.IsSome)
    |>Observable.add (fun e ->
        match e, _ModelItem.Value.Properties.[Extension.IsUITypeRootPropertyIdentifier] with
        | true,x ->
            match 
              match x.IsSet with
              | true ->
                  match x.ComputedValue with
                  | :? bool as y when y ->false
                  | _ ->true
              | _ ->true
              with
            |  true ->x.SetValue true|>ignore
            | _ ->()
        | _,x ->
            x.ClearValue()
        )   

  member private this.GetUITypeRootSetedStatus()=
    match _ModelItem with
    | Some x ->
        match x.Properties.[Extension.IsUITypeRootPropertyIdentifier].ComputedValue with
        | :? bool as y -> Nullable<_>(y)
        | _ ->new Nullable<_>()
    | _ ->new Nullable<_>()

  member private this.ModelItem_UITypeRootControllerChanged=
    new PropertyChangedEventHandler(fun o e ->
      match e.PropertyName with
      | "IsUITypeRoot" ->
          match _CheckUITypeRoot with
          | Some x ->x.IsChecked<-this.GetUITypeRootSetedStatus()
          | _ ->()
      | _ ->()
      )

  override this.Activate (item:ModelItem)=
    if not XmlData.IsDesignAudited && XmlData.IsDisplayController then
      _ModelItem<-Some item
      _ModelItem.Value.PropertyChanged.AddHandler this.ModelItem_UITypeRootControllerChanged
      this.InitializeUITypeRootControllerAdorner()
    base.Activate(item) 

  override this.Deactivate()=
    match _AdornerPanel with
    | Some x->x.Children.Clear()
    | _ ->()
    match _ModelItem with
    | Some x ->
        x.PropertyChanged.RemoveHandler this.ModelItem_UITypeRootControllerChanged
    | _ ->()
    base.Deactivate()