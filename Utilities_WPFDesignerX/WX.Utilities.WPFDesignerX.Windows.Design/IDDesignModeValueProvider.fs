namespace WX.Utilities.WPFDesignerX.Windows.Design

(* Reference
open System
open System.Windows
open System.Windows.Media
open System.Windows.Controls
open Microsoft.Windows.Design.Model
open Microsoft.Windows.Design.Metadata
open WX
open WX.Utilities.WPFDesignerX.Windows

type IDDesignModeValueProvider() as this=
  inherit DesignModeValueProvider()
  do
    this.Properties.Add(typeof<Extension>,"ID")

  override this.TranslatePropertyValue(item:ModelItem,identifier:PropertyIdentifier, value:obj)=
    if identifier.DeclaringType=typeof<Button> && identifier.Name="Extension" then () 
    base.TranslatePropertyValue(item,identifier,value)
*)