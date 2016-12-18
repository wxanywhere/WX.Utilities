namespace WX
open System
open System.Collections.ObjectModel
open System.Text.RegularExpressions

[<AutoOpen>] 
module ConstantModule=
  let Null()=Unchecked.defaultof<_>
  let GuidDefaultValue=new Guid("00000000-0000-0000-0000-000000000000")
  let DefaultGuidValue=new Guid("00000000-0000-0000-0000-000000000001") 