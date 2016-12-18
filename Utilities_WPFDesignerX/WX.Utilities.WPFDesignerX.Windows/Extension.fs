namespace WX.Utilities.WPFDesignerX.Windows

open System
open System.IO
open System.Xml.Linq
open System.Windows
open System.Windows.Controls
open System.ComponentModel
open WX
open WX.Utilities.WPFDesignerX.Common
open WX.Utilities.WPFDesignerX.BusinessEditor

type Extension ()=
  static let IDPropertyR=
    DependencyProperty.RegisterAttached( "ID",typeof<Guid>,typeof<Extension>,
      new FrameworkPropertyMetadata(new Guid(),
        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault ||| FrameworkPropertyMetadataOptions.Journal,
        new PropertyChangedCallback(fun o e->
          match CommonData.IsInitialized with
          | false -> 
              CommonData.IsUITypeRootProperty<-Some Extension.IsUITypeRootProperty
              CommonData.IDProperty<-Some Extension.IDProperty
              CommonData.IsInDesignTime<-DesignerProperties.GetIsInDesignMode  o
              CommonData.IsInitialized<-true
          | _ ->()
          match CommonData.IsAnnotationDataDirty with
          | true ->
              CommonData.IsAnnotationDataDirty<-false
              DataHelper.RetrieveAnnotationData(CommonData.IsInDesignTime)|>ignore
          | _ ->()
          if CommonData.IsInDesignTime || (not CommonData.IsInDesignTime && XmlData.IsDisplayAnnotation) then
             match o, e.NewValue with
             | :? FrameworkElement as x, (:? Guid as y) ->
                 x.Loaded.Add(fun _ -> ElementHelper.INS.UpateAdorner (x,y))
                 if x.IsLoaded then ElementHelper.INS.UpateAdorner (x,y)
             | _ ->()
        ),
        new CoerceValueCallback(fun _ value->
          match value with
          | Null ->Guid.NewGuid() |>box
          | _ ->value
          )
        ),
        new ValidateValueCallback(fun o ->
           o :? Guid
          )
        )

  static let IsUITypeRootPropertyR=
    DependencyProperty.RegisterAttached( "IsUITypeRoot",typeof<bool>,typeof<Extension>,
      new FrameworkPropertyMetadata(false,
        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault ||| FrameworkPropertyMetadataOptions.Journal,
        new PropertyChangedCallback(fun o e->()),
        new CoerceValueCallback(fun _ value->
          match value with
          | Null ->false|>box
          | _ ->value
          )
        ),
        new ValidateValueCallback(fun o ->
           o :? bool
          )
        )

  static member IDProperty with get ()=IDPropertyR
  static member IsUITypeRootProperty with get()=IsUITypeRootPropertyR

  static member GetID (obj:DependencyObject)=
    obj.GetValue(IDPropertyR):?>Guid

  static member SetID(obj:DependencyObject, value:Guid)=
    obj.SetValue(IDPropertyR,value)

  static member GetIsUITypeRoot (obj:DependencyObject)=
    obj.GetValue(IsUITypeRootPropertyR):?>bool

  static member SetIsUITypeRoot(obj:DependencyObject, value:bool)=
    obj.SetValue(IsUITypeRootPropertyR,value)


//=============================================
(*
  //错误参考
  static member  IDProperty=
    DependencyProperty.RegisterAttached( "ID",typeof<Guid>,typeof<UIElementExtension>,...
  //错误信息
  'ID' property was already registered by 'UIElementExtension'.
*)