namespace WX.Utilities.Downloader.ViewModel

open System
open System.IO
open System.Net
open System.Xml
open System.Xml.Linq
open System.Collections.ObjectModel
open System.Windows
open System.Windows.Data
open System.Windows.Input
open System.ComponentModel
open System.Windows.Forms
open WX.Utilities.Common
open WX.Utilities.Downloader.Model

type FVM_NewDownloadTask() as this=
  inherit ViewModelBase()

  do this.Initialize()
  member this.Initialize()=
    this.Title<-"新建任务"
    this.FolderPath<- Config.DefaultDownloadFolder
    this.IsCanExcuteDownload<-true

  [<DV>]val mutable _IsCanExcuteDownload:bool
  member this.IsCanExcuteDownload
    with get()= this._IsCanExcuteDownload
    and set v=
      if v<>this._IsCanExcuteDownload then
        this._IsCanExcuteDownload<-v
        this.OnPropertyChanged "IsCanExcuteDownload"

  [<DV>]val mutable private _D_DownloadingFiles:ObservableCollection<DownloadFileInfo>
  member this.D_DownloadingFiles
    with get ():ObservableCollection<DownloadFileInfo>=
      if this._D_DownloadingFiles=null then
        this._D_DownloadingFiles<-new ObservableCollection<_>()
      this._D_DownloadingFiles

  [<DV>]val mutable _UrlStr:string
  member this.UrlStr
    with get()= this._UrlStr
    and set v=
      if v<>this._UrlStr then
        this._UrlStr<-v
        this.OnPropertyChanged "UrlStr"
        if String.IsNullOrWhiteSpace v|>not then
          this.IsCanExcuteDownload<-false
          match this.FolderPath,this.UrlStr.Split([|'\r';'\n'|],StringSplitOptions.RemoveEmptyEntries) with
          | xf, x ->
              let work=new AsyncWorker<_>(async{
                match 
                  x
                  |>Seq.map (fun a->
                    match new DownloadFileInfo() with
                    | z -> 
                        z.Guid<-Guid.NewGuid()
                        match a.Split('/') with
                        | w ->
                            z.FileName<-w.[w.Length-1]
                        z.FileExtension<-z.FileName.Substring(z.FileName.LastIndexOf('.')+1)
                        z.DownloadPath<-a
                        z.IsNewDownload<-true
                        z.FilePath<-Path.Combine(xf,z.FileName)
                        z.FolderPath<-xf
                        z.TaskStatus<-TaskStatus.LineUp
                        z.Rate<-string z.TaskStatus
                        z
                    )
                  |>Seq.toArray
                  with
                | y ->
                    let count=ref 0
                    let fileName=ref null
                    (*将导致任务不能暂停 Why?
                    for m in y do
                      try
                        let request=HttpWebRequest.Create(m.DownloadPath):?>HttpWebRequest
                        use! rep= request.AsyncGetResponse()
                        let headerInfo=rep.Headers.Get("Content-Length")
                        rep.Close();request.Abort()
                        m.IsCanResumeDownload<-true
                        match Int64.TryParse(headerInfo) with
                        | (true, xa)->
                            m.FileSize<-BusinessComm.GetSizeStr xa
                            m.FileLength<-xa
                        | _ ->
                            m.TaskStatus<-TaskStatus.Error
                            m.Rate<-string m.TaskStatus
                      with _ ->
                        m.FileSize<-BusinessComm.GetSizeStr 0L
                        m.TaskStatus<-TaskStatus.Error
                        m.Rate<-string m.TaskStatus
                    //*)
                    y
                    |>Seq.groupBy(fun b->b.FileName)
                    |>Seq.toArray
                    |>Array.iter (fun (_,b)->
                      b
                      |>Seq.toArray
                      |>fun bx ->
                        bx
                        |>Array.iteri(fun i c->
                          count:=i
                          if bx.Length>1 then 
                            fileName:=c.FileName.Insert(c.FileName.LastIndexOf('.'),String.Format("({0})",!count+1))
                            c.FilePath<-Path.Combine(xf,!fileName)
                          else fileName:=c.FileName
                          while File.Exists c.FilePath || File.Exists (c.FilePath+Value.DldFileSuffix) || bx|>Array.filter(fun d->d.Guid<>c.Guid)|>Array.exists(fun d->d.FilePath=c.FilePath) do
                            fileName:=c.FileName.Insert(c.FileName.LastIndexOf('.'),String.Format("({0})",!count+1))
                            c.FilePath<-Path.Combine(xf,!fileName)
                            incr count
                          c.FileName<- !fileName
                          )
                      )
                    return y
                })
              work.Completed.Add(fun r->
                this.IsCanExcuteDownload<-true
                this.D_DownloadingFiles.Clear()
                this.IsExpandedFolder<-true
                r|>Seq.iter(fun a->this.D_DownloadingFiles.Add a)
                )
              work.RunAsync()|>ignore

  [<DV>]val mutable _IsExpandedFolder:bool
  member this.IsExpandedFolder
    with get()= this._IsExpandedFolder
    and set v=
      if v<>this._IsExpandedFolder then
        this._IsExpandedFolder<-v
        this.OnPropertyChanged "IsExpandedFolder"

  [<DV>]val mutable _FolderPath:string
  member this.FolderPath
    with get()= this._FolderPath
    and set v=
      if v<>this._FolderPath then
        this._FolderPath<-v
        this.OnPropertyChanged "FolderPath"

  [<DV>]val mutable _CMD_Download:ICommand 
  member this.CMD_Download
    with get ()=
      if this._CMD_Download=null then
        this._CMD_Download<-new RelayCommand((fun _ ->
          match this.FolderPath with
          | xf ->
              if Directory.Exists xf|>not then
                Directory.CreateDirectory(xf)|>ignore
              this.D_DownloadingFiles
              |>Seq.toArray
              |> this.CloseCommand.Execute 
          ),
          (fun _ ->String.IsNullOrWhiteSpace this.UrlStr|>not)) 
      this._CMD_Download

  [<DV>]val mutable _CMD_Browse:ICommand 
  member this.CMD_Browse
    with get ()=
      if this._CMD_Browse=null then
        this._CMD_Browse<-new RelayCommand((fun _ ->
          let dialog = new System.Windows.Forms.FolderBrowserDialog()
          match dialog.ShowDialog() with
          | DialogResult.OK ->
              this.FolderPath <- dialog.SelectedPath
          | _ ->()
          ),
          (fun _ ->String.IsNullOrWhiteSpace this.UrlStr|>not)) 
      this._CMD_Browse




