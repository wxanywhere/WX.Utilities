namespace WX.Utilities.Downloader.ViewModel

open System
open System.IO
open System.Xml
open System.Xml.Linq
open System.Collections.ObjectModel
open System.Windows
open System.Windows.Data
open System.Windows.Input
open System.ComponentModel
open System.Diagnostics
open WX.Utilities.Common
open WX.Utilities.Downloader.Model

type FVM_FinishedTaskes() =
  inherit ViewModelBase()

  [<DV>]val mutable private _D_DownloadedFiles:ObservableCollection<DownloadFileInfo>
  member this.D_DownloadedFiles
    with get ():ObservableCollection<_>=this._D_DownloadedFiles
    and set v=this._D_DownloadedFiles<-v

  [<DV>]val mutable _D_DownloadedFile:DownloadFileInfo
  member this.D_DownloadedFile
    with get()= this._D_DownloadedFile
    and set v=
      if v<>this._D_DownloadedFile then
        this._D_DownloadedFile<-v
        this.OnPropertyChanged "D_DownloadedFile"

  [<DV>]val mutable _CMD_File:ICommand 
  member this.CMD_File
    with get ()=
      if this._CMD_File=null then
        this._CMD_File<-new RelayCommand((fun _ ->
          match this.D_DownloadedFile with
          | NotNull xa ->
              if File.Exists xa.FilePath then
                try
                  Process.Start(xa.FilePath)|>ignore
                with _ ->()
          | _ ->()
          ),
          (fun a_ ->this.D_DownloadedFile<>Null()))
      this._CMD_File

  [<DV>]val mutable _CMD_Folder:ICommand 
  member this.CMD_Folder
    with get ()=
      if this._CMD_Folder=null then
        this._CMD_Folder<-new RelayCommand((fun _ ->
          match this.D_DownloadedFile with
          | NotNull xa ->
              if File.Exists xa.FilePath then
                try
                  Process.Start("Explorer.exe", "/select,"+xa.FilePath)|>ignore
                with _ ->()
          | _ ->()
          ),
          (fun a_ ->this.D_DownloadedFile<>Null()))
      this._CMD_Folder

  [<DV>]val mutable _CMD_Delete:ICommand 
  member this.CMD_Delete
    with get ()=
      if this._CMD_Delete=null then
        this._CMD_Delete<-new RelayCommand((fun _ ->
          match MessageBox.Show("选定的下载项信息将被删除，是否继续？","操作提示",MessageBoxButton.YesNo,MessageBoxImage.Question) with
          | MessageBoxResult.Yes ->
              this.D_DownloadedFiles
              |>Seq.filter (fun a->a.IsSelected)
              |>Seq.toArray
              |>Array.iter (fun a->
                 match
                   XmlData.XDownloadedFileInfos.Value.Elements(XName.Get("FileInfo",XmlData.NS))
                   |>Seq.tryFind(fun b->
                       match b.Attribute(XName.Get("Guid",XmlData.NS)) with
                       | NotNull va ->
                           match Guid.TryParse va.Value with
                           | true, vb ->vb=a.Guid
                           | _ ->false
                       | _ ->false
                       )
                   with
                 | Some u ->
                     u.Remove()
                     XmlData.SaveXDoc()|>ignore
                 | _ ->()
                 this.D_DownloadedFiles.Remove a|>ignore
                 )
          | _ ->()
          ),
          (fun _ ->this.D_DownloadedFiles|>Seq.exists(fun a->a.IsSelected)))
      this._CMD_Delete