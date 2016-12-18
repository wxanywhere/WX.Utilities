namespace WX.Utilities.WPFDesignerX.Windows

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Documents
open System.Windows.Media
open System.Windows.Media.Imaging
open System.ComponentModel
open WX

type XButton ()=
  inherit Button()
  static let IDXPropertyR=
    DependencyProperty.Register("IDX", typeof<Guid>, typeof<XButton>, 
      new FrameworkPropertyMetadata(new Guid(),
        new PropertyChangedCallback(fun o e->
          match o with
          | :? XButton as x ->
              ElementHelper.INS.UpateAdorner (x,x.IDX)
          | _ ->()
        ))) 

  static member IDXProperty with get ()=IDXPropertyR

  [<Description("是否处于执行状态")>]
  [<Browsable(true)>]
  [<DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>]
  [<Category("WX")>]
  member this.IDX 
    with get():Guid= this.GetValue(IDXPropertyR):?>Guid
    and set (v:Guid)=this.SetValue(IDXPropertyR,v)

  override this.OnRender(drawingContext:DrawingContext)=
    ElementHelper.INS.UpateAdorner (this,this.IDX)
    base.OnRender(drawingContext)