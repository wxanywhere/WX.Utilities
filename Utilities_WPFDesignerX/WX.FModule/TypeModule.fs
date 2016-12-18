namespace WX
open System
open System.Collections
open System.Text.RegularExpressions
open Microsoft.FSharp.Collections

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

  type BindingDictionaryX<'a,'b when 'a:equality and 'b:equality>(?defaultValue:'b)=
    inherit Generic.Dictionary<'a,'b>()
    member this.Item
      with get key=
        if this.ContainsKey key|>not then base.Add (key,match defaultValue with Some x ->x | _ ->Unchecked.defaultof<'b> )  //不自动创建实例
        base.[key]
      and set key v=
        if this.ContainsKey key|>not then base.Add (key,match defaultValue with Some x->x | _ -> Unchecked.defaultof<'b> )
        if base.[key]<>v then base.[key]<-v