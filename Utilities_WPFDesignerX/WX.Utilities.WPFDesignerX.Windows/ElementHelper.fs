namespace WX.Utilities.WPFDesignerX.Windows

open System
open System.Windows
open System.Windows.Input
open System.Windows.Media
open System.Windows.Media.Imaging
open System.Windows.Documents
open WX
open WX.Utilities.WPFDesignerX.Common
open WX.Utilities.WPFDesignerX.BusinessEditor

type ElementHelper=
  static member public INS = ElementHelper()
  private new() = {}

  member this.UpateAdorner(element:FrameworkElement,elementID:Guid)=
    match 
      match element with
      | :? Window as x ->  //Window是root元素，Runtime下无AdornerLayer,而Designtime可以有AdornerLayer
          match x.Content with
          | :? FrameworkElement as y ->AdornerLayer.GetAdornerLayer y,y, Some x
          | _ ->null,null,None
      | x ->AdornerLayer.GetAdornerLayer x,x,None
      with
    | NotNull xa,NotNull xb, xc ->
        match 
          match xa.GetAdorners xb with
          | NotNull y ->
              match y|>Seq.tryPick (fun a->match a  with :? AnnotationAdorner as x ->Some x | _ ->None) with
              | Some z  ->
                  if elementID>GuidDefaultValue |>not then
                    xa.Remove z
                  else 
                    z.UIElementID<-elementID
                  false
              | None when elementID>GuidDefaultValue ->true
              | _ ->false
          | _ when elementID>GuidDefaultValue ->true
          | _ ->false
          with
        | true ->
            match 
              match xc with
              | Some y ->new AnnotationAdorner(xb,elementID,y)
              | _ ->new AnnotationAdorner(xb,elementID)
              with
            | y ->
                XmlData.IsDisplayAnnotationChanged
                |>Observable.add(fun e ->
                    match e.Flag with
                    | true ->xa.Add y
                    | _ -> xa.Remove y)
                xa.Add y
        | _ ->()
    | _ ->()