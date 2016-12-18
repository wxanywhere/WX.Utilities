namespace WX
open System
open System.Text.RegularExpressions
open Microsoft.FSharp.Collections

[<AutoOpen>]  
module ActiveModule=
  let (|NotNull|Null|) obj=
    if obj<>Unchecked.defaultof<_> then NotNull obj
    else Null

  let (|NotNullOrWhiteSpace|NullOrWhiteSpace|)  (input:string) =
    if String.IsNullOrWhiteSpace input then NullOrWhiteSpace
    else NotNullOrWhiteSpace input

  let (|NotNullOrWhiteSpaceX|NullOrWhiteSpaceX|)  (input:string) =
    if String.IsNullOrWhiteSpace input then NullOrWhiteSpaceX
    else NotNullOrWhiteSpaceX (input.ToLowerInvariant())

  let (|EqualsIn|_|) (conditionElements:string seq) (input:string) =
    match conditionElements,input with
    | x,y when Seq.isEmpty x || y=null ->None
    | x,y->
        if x|>Seq.exists (fun a->a<>null && a.ToLowerInvariant()|>y.ToLowerInvariant().Equals) then Some y
        else None