namespace WX.Utilities.WPFDesignerX.Windows

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Media
open System.Windows.Documents
open WX

[<AutoOpen>]
module TypeExtension=()


//=============================================
(*
  type AdornerLayer with
    //正确，但AdornerLayer只有位于控件元素的上一级，才能调用Adorner实例的OnRender方法
    static member GetAdornerLayer (window:Window)=
      match AdornerLayer.GetAdornerLayer (window:>Visual) with
      | NotNull x->x
      | _ ->
          match new AdornerDecorator() with 
          | y ->
              match window.Content with
              | :? UIElement as z ->
                  window.Content<-y //必须的
                  y.Child<-z
              | _ ->
                  window.Content<-y
              y.AdornerLayer

    //正确，但AdornerLayer只有位于控件元素的上一级，才能调用Adorner实例的OnRender方法
    static member GetAdornerLayerX (window:Window)=
      match window with
      | NotNull x ->
          let rec GetAdornerLayer (visual:Visual)=
            seq{
              match visual with
              | :? AdornerDecorator as y ->yield y.AdornerLayer
              | :? ScrollContentPresenter as y->yield y.AdornerLayer
              | NotNull y ->
                  for n in 0..(VisualTreeHelper.GetChildrenCount y)-1 do
                    yield! VisualTreeHelper.GetChild (y,n):?>Visual|>GetAdornerLayer
              | _ ->()
            }
          match GetAdornerLayer x|>Seq.tryFind(fun _->true) with 
          | Some y ->y
          | _ ->null
      | _ ->
          raise <| new ArgumentNullException("window")

  //获取上一级控件
  static member GetPreLevelElement (element:FrameworkElement)=
    match element with
    | NotNull x ->
        let rec GetPreLevelElement (visual:Visual)=
          seq{
            match VisualTreeHelper.GetParent(visual) with
            | :? Control as y ->yield y
            | :? Visual as y ->yield! GetPreLevelElement y
            | _ ->()
          }
        GetPreLevelElement x|>Seq.tryFind(fun _->true)
    | _ ->
        raise <| new ArgumentNullException("element")
*)