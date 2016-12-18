namespace WX.Utilities.WPFDesignerX.Windows

(*
open System
open System.Globalization
open System.ComponentModel
open System.Windows
open System.Windows.Controls
open System.Windows.Input
open System.Windows.Media
open System.Windows.Media.Imaging
open System.Windows.Documents
open WX
open WX.Utilities.WPFDesignerX.BusinessEditor

(*
Adorners Overview
http://msdn.microsoft.com/en-us/library/ms743737.aspx
*)
type EditorNumberAdorner(adornedElement:FrameworkElement, elementNumber) as this=
  inherit Adorner(adornedElement)
  do
    //adornedElement.IsVisibleChanged.Add(fun _ ->this.InvalidateVisual()) 
    this.IsClipEnabled<-true 
    this.ClipToBounds<-false

  override this.OnRender(drawingContext:DrawingContext )=
    match adornedElement with
    | x ->
        //http://stackoverflow.com/questions/9454756/draw-oval-to-array
        drawingContext.DrawEllipse(null,  new Pen(Brushes.Blue,1.0) ,new Point(this.ActualWidth-24.0,9.0),8.0,8.0)
        drawingContext.DrawText(
          new FormattedText( string elementNumber, CultureInfo.CurrentCulture,FlowDirection.LeftToRight,  new Typeface("Verdana"),8.0,Brushes.Blue),
          new Point(this.ActualWidth-32.0,1.0))
*)