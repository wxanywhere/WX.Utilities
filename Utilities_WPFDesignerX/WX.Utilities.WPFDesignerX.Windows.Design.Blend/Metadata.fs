namespace WX.Utilities.WPFDesignerX.Windows.Design

open System
open System.Collections.Generic
open System.Text
open System.ComponentModel
open System.Windows.Media
open System.Windows.Controls
open System.Windows
open Microsoft.Windows.Design.Features
open Microsoft.Windows.Design.Metadata
open WX.Utilities.WPFDesignerX.Windows

type internal Metadata()=
  interface IProvideAttributeTable with
    member this.AttributeTable
      with get():AttributeTable= 
        match new AttributeTableBuilder() with
        | x ->
            x.AddCustomAttributes(typeof<Control>,new FeatureAttribute(typeof<BusinessEditorAdornerProvider>))
            x.CreateTable()
