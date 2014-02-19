namespace WX.Utilities.Common
open System
open System.Windows
open System.Windows.Data

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module BusinessComm=
  let Timer=
    match new System.Timers.Timer() with
    | x ->
        x.Interval<-1000.0
        x.Enabled<-true
        x

  let GetSizeStr length=
    match length with
    | x ->
        if x>= int64 1073741824L then
          String.Format("{0:F2} GB",float x/1073741824.00)
        elif x>=1048576L then
          String.Format("{0:F2} MB",float x/1048576.00)
        elif x>=1024L then
          String.Format("{0:F2} KB",float x/1024.00)
        else 
          String.Format("{0:F2} B",float x)
//-----------------------------------------------------------------------------------------------


[<AutoOpen>]
module TypeAlias=
  type DV=Microsoft.FSharp.Core.DefaultValueAttribute

//-----------------------------------------------------------------------------------------------

[<AutoOpen>] 
module ConstantModule=
  let Null()=Unchecked.defaultof<_>
  let GuidDefaultValue=new Guid("00000000-0000-0000-0000-000000000000")
  let DefaultGuidValue=new Guid("00000000-0000-0000-0000-000000000001") 

//-----------------------------------------------------------------------------------------------

[<AutoOpen>]  
module ActiveModule=
  let (|NotNull|Null|) obj=
    if obj<>Unchecked.defaultof<_> then NotNull obj
    else Null

  let (|EqualsIn|_|) (conditionElements:string seq) (input:string) =
    match conditionElements,input with
    | x,y when Seq.isEmpty x || y=null ->None
    | x,y->
        if x|>Seq.exists (fun a->a<>null && a.ToLowerInvariant()|>y.ToLowerInvariant().Equals) then Some y
        else None

//-----------------------------------------------------------------------------------------------

[<AutoOpen>]
module ControlExtensions=
  type System.Windows.Controls.ListView with
    member this.SelectedItemBinding path=
      BindingOperations.SetBinding(this,System.Windows.Controls.ListView.SelectedItemProperty,new Binding(path))|>ignore
    
    member this.ItemsSourceBinding path=
      BindingOperations.SetBinding(this,System.Windows.Controls.ListView.ItemsSourceProperty,new Binding(path))|>ignore

//-----------------------------------------------------------------------------------------------

[<AutoOpen>] 
module TypeModule=
  type KeyValue<'a,'b>=
    [<DefaultValue>] 
    val mutable private _Key:'a
    [<DefaultValue>]
    val mutable private _Value:'b
    new ()={}
    new (key,value) as this={} then
      this._Key<-key
      this._Value<-value  
    member this.Key
      with get ()=this._Key
      and set v=this._Key<-v
    member this.Value
      with get ()=this._Value
      and set v=this._Value<-v