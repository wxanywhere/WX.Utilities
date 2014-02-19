
namespace WX.Utilities.Common
open System
open System.Windows
open System.IO
open System.Xml
open System.Xml.Linq
open System.Reflection


[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Value=
  let DldFileSuffix=".x"
  let CfgFileSuffix=".x.cfg"