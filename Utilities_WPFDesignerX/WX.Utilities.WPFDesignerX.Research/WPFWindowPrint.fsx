
#r "PresentationFramework"
#r "PresentationCore"
#r "WindowsBase"
#r "System.Xaml"
#r "UIAutomationTypes"
open System
open System.Xaml
open System.Xml
open System.Globalization
open System.IO
open System.Windows
open System.Windows.Markup
open System.Windows.Controls
open System.Windows.Documents
open System.Windows.Media
open System.Windows.Media.Imaging


#I @"D:\Workspace\WX\Dev\WPFDesignerX\WX.Utilities.WPFDesignerX.Assemblies\Debug"
#r  "WX.FModule"
#r "WX.Utilities.WPFDesignerX.Windows"
#r  "WX.Utilities.WPFDesignerX.TestApp"
open WX
open WX.Utilities.WPFDesignerX.Windows
open WX.Utilities.WPFDesignerX.TestApp

#I @"D:\Workspace\WX\Dev\WPFDesignerX示例xxx\系统支撑\M00\bin\Debug"
#r "M00.Screens.dll"
#r "PMS.BusinessControl.dll"
open M00Screens



//let button=new Button()
//button.GetType().Name
//
//
//typeof<Button>.IsSubclassOf typeof<FrameworkElement>
//
//typeof<FrameworkElement>


let GetCommentedElements (root:FrameworkElement)=
  let size =new Size(root.Width,root.Height)|>ref
  match root with
  | NotNull x ->
      let rec GetCommentedElements (visual:Visual)=
        seq{
          match visual with
          | NotNull y ->
              match y with
              | :? FrameworkElement as z ->
                  z.UpdateLayout()
                  z.UpdateDefaultStyle()
                  
//                  printfn "%A" (z.GetType().Name)
//                  printfn "%A" z.ActualWidth
//                  printfn "%A" z.ActualHeight
//                  printfn "%A" z.Width
//                  printfn "%A" z.Height
                  match z.GetValue (Extension.IDProperty) with
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
                | :? UserControl as z when z.Parent=null ->Some  (z:>FrameworkElement)
                | _ ->None
                with
                | Some y ->
                      y.Measure(!size)
                      y.Arrange (new Rect(!size))
                      printfn "%A" (y.GetType().Name)
                      printfn "%A" y.ActualWidth
                      printfn "%A" y.ActualHeight
                      printfn "%A" y.Width
                      printfn "%A" y.Height
                | _ ->()
              match y with 
              | :? Window as z ->
                  match z.Content with
                  | :? FrameworkElement as w ->
                      yield! GetCommentedElements w
                      (*
                      for n in 0..(VisualTreeHelper.GetChildrenCount w)-1 do
                        yield! VisualTreeHelper.GetChild (w,n):?>Visual|>GetCommentedElements
                      *)
                  | _ ->()
              | _ ->
                  for n in 0..(VisualTreeHelper.GetChildrenCount y)-1 do
                    yield! VisualTreeHelper.GetChild (y,n):?>Visual|>GetCommentedElements
          | _ ->()
        }
      GetCommentedElements x|>Seq.toArray
  | _ ->
      raise <| new ArgumentNullException("root")

let GetTabControls (root:FrameworkElement)=
  let size =new Size(root.Width,root.Height)|>ref
  match root with
  | NotNull x ->
      let rec GetTabControls (visual:Visual)=
        seq{
          match visual with
          | NotNull y ->
              match y with
              | :? TabControl as z when z.Visibility=Visibility.Visible ->yield z
              | _ ->()
              match 
                match y with
                | :? Window as z ->
                    match z.Content with
                    | :? FrameworkElement as w ->Some w
                    | _ ->None
                | :? UserControl as z when z.Parent=null ->Some  (z:>FrameworkElement)
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
      GetTabControls x|>Seq.toArray
  | _ ->
      raise <| new ArgumentNullException("root")


//let w=new MainWindow()
//let w=new UC_Test()
//let w=new M00Screens.UC_XTZC_YHWH()
let w=new M00Screens.UC_XTZC_GCWH()
//let fileName= @"D:\Workspace\WX\Dev\WPFDesignerX示例\系统支撑\M00\M00Screens\UC_XTZC_YHWH.xaml"
//let reader = XmlReader.Create(fileName)
//let w=XamlReader.Load(reader):?>UserControl


w.UpdateLayout()

let tabControls= GetTabControls w
tabControls.[0].GetHashCode()
match tabControls.[0].Items.Item 2 with
| :? TabItem as x ->x.IsSelected<-true
| _ ->()

VisualTreeHelper.GetChildrenCount tabControls.[0]

VisualTreeHelper.GetChild(tabControls.[0],0)

tabControls.Length

let elements= GetCommentedElements w

(*
for n in tabControls.[0].Items do
  match n with
  | :? TabItem as x ->
*)

//w.ApplyTemplate()
//let w=new UC_Test() 
//w.UpdateLayout()
//w.InvalidateMeasure()
//w.InvalidateArrange()
//w.InvalidateVisual()

//w.ApplyTemplate()|>ignore
//w.Visibility<-Visibility.Hidden
//let xx=new Size(w.Width,w.Height)
//w.Measure xx
//w.Arrange (new Rect(xx))
//w.VerifyAccess()
//w.UpdateLayout()
//w.Activate()
//w.Show()



w.InvalidateVisual()
w.Visibility<-Visibility.Visible
w.Visibility<-Visibility.Collapsed
//w.Show()
w.Content :? FrameworkElement

VisualTreeHelper.GetChildrenCount (w.Content:?>FrameworkElement)
VisualTreeHelper.GetChildrenCount w
VisualTreeHelper.GetOffset w
let xxx=VisualTreeHelper.GetChild ((w.Content:?>FrameworkElement),2)
xxx.GetType().Name
VisualTreeHelper.GetChildrenCount (xxx:?> TreeView)
GetCommentedElements  (xxx:?> TreeView)

let elements= GetCommentedElements w
elements.Length

elements.[2].TranslatePoint(new Point(0.0,0.0),w)
elements.[2].TransformToVisual w

w.Loaded.Add(fun _ ->printf "%s" "ssssssssssssssssssssssss")
AdornerLayer.GetAdornerLayer elements.[0]

elements
|>Seq.iteri (fun i a ->
  match 
    match a with
    | :? Window as x ->  //Window是root元素，Runtime下无AdornerLayer,而Designtime可以有AdornerLayer
        match x.Content with
        | :? FrameworkElement as y ->AdornerLayer.GetAdornerLayer y,y
        | _ ->null,null
    | x -> AdornerLayer.GetAdornerLayer x,x
    with
  | NotNull xa,NotNull xb ->
      match xa.GetAdorners xb with
      | NotNull x ->
          printf "%A" (a.GetType().Name)
          for n in x do
            match n with
            | :? BusinessEditorAdorner as y ->xa.Remove y
            | :? EditorNumberAdorner as y ->xa.Remove y
            | y ->xa.Remove y
      | _ ->()
      match new EditorNumberAdorner(xb,i+96) with
      | x ->
          xa.Add x
          x.InvalidateVisual()
  | _ ->()
  )

//------------------
let v=w
let transform=v.LayoutTransform
v.LayoutTransform=null

let size=new Size(w.Width,w.Height)
v.Measure size
v.Arrange (new Rect(size))

//let bitmap=new RenderTargetBitmap(int size.Width, int size.Height,96.0,96.0,PixelFormats.Pbgra32)
let bitmap=new RenderTargetBitmap(int size.Width, int size.Height,96.0,96.0,PixelFormats.Pbgra32)
let renderContent=
  match v:>FrameworkElement with
  | :? Window ->
      match v.Content with
      | :? FrameworkElement as x ->
          x.RenderSize<-new Size(x.RenderSize.Width+x.Margin.Left+x.Margin.Right,x.RenderSize.Height+x.Margin.Top+x.Margin.Bottom)
          x 
      | _ ->null
  | _ ->v:>FrameworkElement

v.Width
//renderContent.RenderSize
//v.Content:>FrameworkElement

//renderContent.Margin
renderContent.Width<-renderContent.ActualWidth
renderContent.Height<-renderContent.ActualHeight


//v.RenderSize<-new Size(v.Width,v.Height)

bitmap.Render renderContent
let dv=new DrawingVisual()
let dc=dv.RenderOpen()

dc.DrawImage(bitmap,new Rect(0.0,0.0,v.Width,v.Height))
dc.DrawRectangle(null,new Pen(Brushes.Gray,1.0),new Rect(0.0,0.0,v.Width,v.Height))
elements
|>Seq.iteri (fun i a ->
    printf "%A" (a.GetType().Name)
    printf "%A" a.ActualWidth
    match a.TranslatePoint(new Point(a.ActualWidth-16.0+renderContent.Margin.Left ,1.0+renderContent.Margin.Top),renderContent), new Size(15.0,13.0) with //v.Content:?>FrameworkElement
    | x,y ->
        dc.DrawRectangle(null,new Pen(Brushes.Blue,1.0),new Rect(x,y))
    match a.TranslatePoint(new Point(a.ActualWidth-13.0,3.0),renderContent) with
    | x ->
        dc.DrawText(
          new FormattedText( string (i+1), CultureInfo.CurrentCulture,FlowDirection.LeftToRight,  new Typeface("Verdana"),8.0,Brushes.Blue),
          x)
        )
dc.Close()

bitmap.Render dv
let stream= new FileStream(@"d:\Temp\Testing\x.png",FileMode.OpenOrCreate)
let encoder =new PngBitmapEncoder()
encoder.Frames.Add (BitmapFrame.Create bitmap)
encoder.Save stream
stream.Close()
v.LayoutTransform<-transform

(*
let v=w
let transform=v.LayoutTransform
v.LayoutTransform=null

let size=new Size(w.Width,w.Height)
v.Measure size
v.Arrange (new Rect(size))

let bitmap=new RenderTargetBitmap(int size.Width, int size.Height,96.0,96.0,PixelFormats.Pbgra32)
//v.
bitmap.Render v

let stream= new FileStream(@"d:\Temp\Testing\x.png",FileMode.OpenOrCreate)
let encoder =new PngBitmapEncoder()
encoder.Frames.Add (BitmapFrame.Create bitmap)

encoder.Save stream
stream.Close()
v.LayoutTransform<-transform
*)


let uc=new System.Windows.Controls.UserControl()

(*
type EditorNumberAdorner(adornedElement:FrameworkElement, elementNumber) as this=
  inherit Adorner(adornedElement)
  do
    //adornedElement.IsVisibleChanged.Add(fun _ ->this.InvalidateVisual()) 
    this.IsClipEnabled<-true 
    this.ClipToBounds<-false

  override this.OnRender(drawingContext:DrawingContext )=
    match adornedElement with
    | x when true -> //x.Visibility=Visibility.Visible ->
        //http://stackoverflow.com/questions/9454756/draw-oval-to-array
        //drawingContext.DrawEllipse(null,  new Pen(Brushes.Blue,1.0) ,new Point(this.ActualWidth-22.0,7.0),6.0,6.0)
        drawingContext.DrawRectangle(null,new Pen(Brushes.Blue,1.0),new Rect(this.ActualWidth-16.0,1.0,15.0,13.0))
        drawingContext.DrawText(
          new FormattedText( string elementNumber, CultureInfo.CurrentCulture,FlowDirection.LeftToRight,  new Typeface("Arial"),8.0,Brushes.Blue),
          new Point(this.ActualWidth-13.0,3.0))
    | _ ->()
*)