namespace WX.Utilities.WPFDesignerX.Windows

open System
open System.IO
open System.Windows
open System.Windows.Controls
open System.Windows.Media
open System.Windows.Documents
open WX
open WX.Utilities.WPFDesignerX.Common
open WX.Utilities.WPFDesignerX.BusinessEditor

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module DataHelper=
  let RuntimeDataFilePath=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"AnnotationData.xml")
  let RetrieveAnnotationData(isInDesignTime:bool)=
    match isInDesignTime with
    | true ->
        match XmlData.FilePath with
        | NotNullOrWhiteSpace x when File.Exists x ->
            XmlData.LoadOptionData(x)
            true
        | _ ->
            match new FVM_DataFilePathEditor(),new DataFilePathEditor() with
            | vm,v -> 
                v.DataContext<-vm
                vm.MainWindow<-v
                vm.Initialize()
                vm.RequestClose.Add(fun e->
                  match e.Data with
                  | :? string as x ->
                      XmlData.FilePath<-x
                      XmlData.LoadOptionData(x)
                  | _ ->()
                  )
                match Application.Current.MainWindow with
                | NotNull x when x :? DataFilePathEditor|>not ->
                    v.Owner<-x
                    v.WindowStartupLocation<-WindowStartupLocation.CenterOwner
                | _ ->
                    v.WindowStartupLocation<-WindowStartupLocation.Manual
                v.ResizeMode<-ResizeMode.NoResize
                v.ShowDialog()|>ignore
                vm.DialogResult
    | _ ->
        XmlData.FilePath<-RuntimeDataFilePath
        XmlData.LoadOptionData(RuntimeDataFilePath)
        true

(*
//使用Properties.Settings方式，有问题，所有工程都会指向一个同一个配置路径
//列如"C:\Users\Administrator\AppData\Local\Microsoft_Corporation\Blend.exe_StrongName_shgz2ao03fq3ntpaaxgsb5s4qubvzkks\4.0.20525.0\user.config"
let RetrieveAnnotationData(isInDesignTime:bool)=
  match isInDesignTime with
  | true ->
      match Settings.Default.AnnotationDataFilePath with
      | NotNullOrWhiteSpace x when File.Exists x ->
          XmlData.FilePath<-x
          XmlData.LoadOptionData(x)
          true
      | _ ->
          match new FVM_DataFilePathEditor(),new DataFilePathEditor() with
          | vm,v -> 
              v.DataContext<-vm
              vm.MainWindow<-v
              vm.Initialize()
              vm.RequestClose.Add(fun e->
                match e.Data with
                | :? string as x ->
                    XmlData.FilePath<-x
                    Settings.Default.AnnotationDataFilePath<-x
                    Settings.Default.Save()
                    XmlData.LoadOptionData(x)
                | _ ->()
                )
              match Application.Current.MainWindow with
              | NotNull x when x :? DataFilePathEditor|>not ->
                  v.Owner<-x
                  v.WindowStartupLocation<-WindowStartupLocation.CenterOwner
              | _ ->
                  v.WindowStartupLocation<-WindowStartupLocation.Manual
              v.ResizeMode<-ResizeMode.NoResize
              v.ShowDialog()|>ignore
              vm.DialogResult
  | _ ->
      XmlData.FilePath<-RuntimeDataFilePath
      XmlData.LoadOptionData(RuntimeDataFilePath)
      true
*)

(*
Visual Studio Automation Object Model. EnvDTE interfaces 
http://www.viva64.com/en/b/0169/
*)