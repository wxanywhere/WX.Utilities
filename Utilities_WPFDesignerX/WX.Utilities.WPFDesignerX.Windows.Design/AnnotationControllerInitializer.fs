namespace WX.Utilities.WPFDesignerX.Windows.Design

(* Reference
open System
open Microsoft.Windows.Design
open Microsoft.Windows.Design.Model
open Microsoft.Windows.Design.Interaction
open WX
open WX.Utilities.WPFDesignerX.Windows

type AnnotationControllerInitializer() =
  inherit DefaultInitializer()

  member this.SetDefaultValue(property:ModelProperty,value:obj)=
    if obj.Equals(property.DefaultValue,value) then
      if property.IsSet then property.ClearValue()
    else
      property.SetValue value |>ignore

  override this.InitializeDefaults(item:ModelItem, context:EditingContext)=
    this.SetDefaultValue (item.Properties.[Extension.IDPropertyIdentifier],Guid.NewGuid())
    let newItem = ModelFactory.CreateItem(context, Extension.TypeId, CreateOptions.InitializeDefaults);
    item.Content.Collection.Add(newItem)
*)