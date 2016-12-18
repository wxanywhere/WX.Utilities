namespace WX.Utilities.WPFDesignerX.Windows
open System.Configuration
open System.Windows.Markup
open System.Runtime.InteropServices

(*
[<Sealed>]
type internal Settings()= 
  inherit ApplicationSettingsBase()
  static let defaultInstance = ApplicationSettingsBase.Synchronized(new Settings()) :?> Settings  
  static member public Default
    with get ()=defaultInstance

  [<UserScopedSettingAttribute()>]
  member this.UserSetting
    with get() = this.Item("UserSetting") :?> string
    and set(value : string) = this.Item("UserSetting") <- value

  [<ApplicationScopedSettingAttribute()>]
  member this.AnnotationDataFilePath
    with get() = this.Item("AnnotationDataFilePath") :?> string

*)
(*
[<global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>]
[<global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")>]
[<Sealed>]
type internal Settings()= 
  inherit global.System.Configuration.ApplicationSettingsBase() 
    
  static let defaultInstance = global.System.Configuration.ApplicationSettingsBase.Synchronized(new Settings()) :?> Settings  
  static member public Default
    with get ()=defaultInstance

  [<global.System.Configuration.ApplicationScopedSettingAttribute()>]
  [<global.System.Diagnostics.DebuggerNonUserCodeAttribute()>]
  [<global.System.Configuration.DefaultSettingValueAttribute("")>]
  member this.AnnotationDataFilePath
    with get ()=this.Item("AnnotationDataFilePath"):?>string
  *)