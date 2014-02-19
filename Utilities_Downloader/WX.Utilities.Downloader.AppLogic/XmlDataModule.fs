
namespace WX.Utilities.Common
open System
open System.Windows
open System.IO
open System.Xml
open System.Xml.Linq
open System.Reflection


[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module XmlData=
  let NS="http://schemas.wx.com/2013"
  let Prefix="m"
  let mutable FilePath=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"DownloadInfo.cfg")
  let mutable XDoc:XDocument option=None
  let mutable XDownloadingFileInfos:XElement option=None
  let mutable XDownloadedFileInfos:XElement option=None

  let SaveXDoc()=
    match XDoc with
    | Some x ->
        try
          x.Save FilePath
          true
        with _ ->false
    | _ ->false