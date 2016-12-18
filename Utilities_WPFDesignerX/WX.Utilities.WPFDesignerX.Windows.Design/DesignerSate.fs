namespace WX.Utilities.WPFDesignerX.Windows.Design

(* Reference
open System
open System.Collections.Generic
open Microsoft.Windows.Design.Model
open WX

type DesignerProperty<'a>(name:String)=
  let mutable _Name=String.Empty
  do 
    _Name<-name
  override this.ToString()=_Name
  
[<AutoOpen>]    
module DesignerState=
  type StateTable()=
    inherit Dictionary<obj,obj>()
  type DesignerStateDictionary()=
    inherit Dictionary<ModelItem,StateTable>()

  type ModelItem with
    member this.GetDesignerProperty<'a>(property:DesignerProperty<'a>,factory:Func<ModelItem,'a>)=
      match 
        match this.Context.Services.GetService<DesignerStateDictionary>() with
        | NotNull x ->x
        | _  ->
            match new DesignerStateDictionary() with
            | y ->
                this.Context.Services.Publish y
                y
        with
      | x ->
          match x.TryGetValue(this) with
          | true, y ->
              match y.TryGetValue(property) with
              | true, z ->z:?>'a
              | _ when  factory<>null->
                  match factory.Invoke(this) with
                  | z ->
                      y.[property]<-z
                      z
              | _ ->Unchecked.defaultof<'a>
          | _ ->Unchecked.defaultof<'a>
        
    member this.GetDesignerProperty<'a> (property:DesignerProperty<'a>)=
      this.GetDesignerProperty(property,null)

    member this.SetDesignerProperty<'a>(property:DesignerProperty<'a>,value:'a)=
      match 
        match this.Context.Services.GetService<DesignerStateDictionary>() with
        | NotNull x ->x
        | _ ->
            match new DesignerStateDictionary() with
            | y ->
                this.Context.Services.Publish y
                y
        with
      | x ->
          match x.TryGetValue(this) with
          | true, y ->
              y.[property]<-value
          | _ ->
              x.[this]<-new StateTable()

    member this.ClearDesignerProperty<'a>(property:DesignerProperty<'a>)=
      match this.Context.Services.GetService<DesignerStateDictionary>() with
      | NotNull x ->
          match x.TryGetValue (this) with
          | true, y->
              y.Remove(property)|>ignore
              if y.Count=0 then x.Remove (this)|>ignore
          | _ ->()
      | _ ->()
 
 //*)