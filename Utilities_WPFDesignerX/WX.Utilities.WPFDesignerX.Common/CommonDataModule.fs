namespace WX.Utilities.WPFDesignerX.Common
open System
open System.Windows
open System.Windows.Media
open System.IO
open System.Xml
open System.Xml.Linq
open System.Reflection
open WX

type EditorOperateScope=
  | All=0
  | View=1
  | ViewAndEdit=2
  | ViewAndOption=3

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CommonData=
  let mutable IsInitialized=false
  let mutable IsAnnotationDataDirty=true
  let mutable IsInDesignTime=false
  let mutable UIAssembly:Assembly option=None
  let mutable IDProperty:DependencyProperty option=None
  let mutable IsUITypeRootProperty:DependencyProperty option=None
  let mutable EditorOperateScope:EditorOperateScope=EditorOperateScope.All

  let TryGetUITypeRoot(element:FrameworkElement):FrameworkElement option=
    match element,IsUITypeRootProperty with
    | NotNull xa, Some xb ->
        let rec TryGetUITypeRoot (visual:Visual)=
          match visual with
          | :? FrameworkElement as y ->
              match y.GetValue xb with
              | :? bool as z when z ->Some y
              | _ ->VisualTreeHelper.GetParent y|>unbox<Visual>|>TryGetUITypeRoot
          | NotNull y -> VisualTreeHelper.GetParent y|>unbox<Visual>|>TryGetUITypeRoot
          | _ ->None
        TryGetUITypeRoot xa
    | _ ->
        raise <| new ArgumentNullException("element")