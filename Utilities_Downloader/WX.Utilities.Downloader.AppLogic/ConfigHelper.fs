

namespace WX.Utilities.Common
open System
open System.IO
open System.Xml
open System.Text.RegularExpressions
open System.Configuration
open System.Runtime.Serialization

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Config=
  let MaxDownloadingNumber=
    match Int32.TryParse(ConfigurationManager.AppSettings.["MaxDownloadingNumber"]) with
    | true, x-> 
        match x with
        | _ when x>50-> 50
        | _ when x<1-> 1
        | _ ->x
    | _ ->10

  let FileTheadNumber=
    match Int32.TryParse(ConfigurationManager.AppSettings.["FileTheadNumber"]) with
    | true, x ->
        match x with
        | _ when x>20-> 20
        | _ when x<1-> 1
        | _ ->x
    | _ ->5

  let BufferSize=
    match Int64.TryParse(ConfigurationManager.AppSettings.["BufferSize"]) with
    | true, x ->
        match x with
        | _ when x>16L-> 16L
        | _ when x<2L-> 2L
        | _ ->x
    | _ ->8L

  let DefaultDownloadFolder=
    match ConfigurationManager.AppSettings.["DefaultDownloadFolder"] with
    | NotNull x ->x
    | _ ->"D:\XDOWNLOAD"

    