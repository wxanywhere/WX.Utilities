namespace WX.Utilities.WPFDesignerX.Common
open System
open System.Windows
open System.IO
open System.Xml
open System.Xml.Linq
open System.Reflection
open WX

type IsDisplayAnnotationChangedEventArgs (flag:bool)=
  inherit EventArgs()
  member this.Flag=flag
type IsDisplayAnnotationChangedEventHandler=delegate of obj*IsDisplayAnnotationChangedEventArgs->unit

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module XmlData=
  let private _IsDisplayAnnotationChangedEvent=new Event<IsDisplayAnnotationChangedEventHandler,IsDisplayAnnotationChangedEventArgs>()
  [<CLIEvent>]     
  let IsDisplayAnnotationChanged=_IsDisplayAnnotationChangedEvent.Publish
  let OnIsDisplayAnnotationChanged (flag:bool)=
    if IsDisplayAnnotationChanged<>Null() then
      _IsDisplayAnnotationChangedEvent.Trigger(null,new IsDisplayAnnotationChangedEventArgs(flag))   

  let NS="http://schemas.ydtf.com/2013"
  let Prefix="m"
  let mutable FilePath=String.Empty
  let mutable XDoc:XDocument option=None
  let mutable XOption:XElement option=None
  let mutable XServiceInfos:XElement option=None
  let mutable XAnnotations:XElement option=None
  let mutable IsDisplayAnnotation=true
  let mutable DisplayAnnotationRestoreFlag=false
  let mutable IsDisplayController=true
  let mutable IsDesignAudited=false
  let mutable DocumentTitle=String.Empty

  let SaveXDoc()=
    match XDoc with
    | Some x ->
        try
          x.Save FilePath
          true
        with _ ->false
    | _ ->false

  let LoadOptionData(filePath:string)=
    if File.Exists filePath then
      match 
        (
        try
          match XDocument.Load(FilePath) with
          | NotNull x ->Some x
          | _ ->None
        with _ ->None)
        with
      | Some x ->
          XDoc<-Some x
          match x.Descendants(XName.Get("Option",NS)) with
          | y when Seq.length y=1 ->
              match Seq.head y with
              | z ->
                  XOption<-Some z
                  match z.Attribute(XName.Get("IsDisplayAnnotation",NS)) with
                  | NotNull w ->
                      match w.Value|>Boolean.TryParse with
                      | true, u ->
                          IsDisplayAnnotation<-u
                      | _ ->()
                  | _ ->() 
                  match z.Attribute(XName.Get("IsDisplayController",NS)) with
                  | NotNull w ->
                      match w.Value|>Boolean.TryParse with
                      | true, u ->
                          IsDisplayController<-u
                      | _ ->()
                  | _ ->() 
                  match z.Attribute(XName.Get("IsDesignAudited",NS)) with
                  | NotNull w ->
                      match w.Value|>Boolean.TryParse with
                      | true, u ->
                          IsDesignAudited<-u
                      | _ ->()
                  | _ ->() 
                  match z.Attribute(XName.Get("DocumentTitle",NS)) with
                  | NotNull w ->
                      DocumentTitle<-w.Value
                  | _ ->() 
          | _ ->()
      | _ ->
          MessageBox.Show ( @"标注数据加载失败！","数据错误",MessageBoxButton.OK)|>ignore