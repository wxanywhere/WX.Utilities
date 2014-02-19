namespace WX.Utilities.Downloader.Model
open System
open System.Xml
open System.Xml.Linq
open System.Net
open System.Windows.Input
open System.Collections
open System.Collections.ObjectModel
open WX.Utilities.Common
open WX.Utilities.Downloader.ViewModel

type TaskStatus=
  | Wait
  | LineUp
  | Pausing
  | Paused
  | Downloading
  | Downloaded
  | Deleting
  | Deleted
  | Error
  override this.ToString()=
    match this with
    | Wait ->"等待..."
    | LineUp ->"排队..."
    | Pausing ->"暂停中..."
    | Paused ->"暂停"
    | Downloading ->"下载..."
    | Downloaded ->"下载完成"
    | Deleting ->"删除..."
    | Deleted ->"删除"
    | Error ->"错误"

type DownloadFileInfo()=
  inherit ModelBase()

  [<DV>]val mutable private _IsSelected:bool
  member this.IsSelected
    with get ()=this._IsSelected
    and set v=
      if this._IsSelected<>v then
        this._IsSelected<-v
        this.OnPropertyChanged "IsSelected"

  [<DV>]val mutable private _FileName:string
  member this.FileName
    with get ()=this._FileName
    and set v=
      if this._FileName<>v then
        this._FileName<-v
        this.OnPropertyChanged "FileName"

  [<DV>]val mutable private _FileExtension:string
  member this.FileExtension
    with get ()=this._FileExtension
    and set v=
      if this._FileExtension<>v then
        this._FileExtension<-v
        this.OnPropertyChanged "FileExtension"

  [<DV>]val mutable private _FilePath:string
  member this.FilePath
    with get ()=this._FilePath
    and set v=
      if this._FilePath<>v then
        this._FilePath<-v
        this.OnPropertyChanged "FilePath"

  [<DV>]val mutable private _FolderPath:string
  member this.FolderPath
    with get ()=this._FolderPath
    and set v=
      if this._FolderPath<>v then
        this._FolderPath<-v
        this.OnPropertyChanged "FolderPath"

  [<DV>]val mutable private _DownloadPath:string
  member this.DownloadPath
    with get ()=this._DownloadPath
    and set v=
      if this._DownloadPath<>v then
        this._DownloadPath<-v
        this.OnPropertyChanged "DownloadPath"

  [<DV>]val mutable private _FileSize:string
  member this.FileSize
    with get ()=this._FileSize
    and set v=
      if this._FileSize<>v then
        this._FileSize<-v
        this.OnPropertyChanged "FileSize"

  [<DV>]val mutable private _FileLength:int64
  member this.FileLength
    with get ()=this._FileLength
    and set v=
      if this._FileLength<>v then
        this._FileLength<-v
        this.OnPropertyChanged "FileLength"

  [<DV>]val mutable private _ElapsedMillisecond:int
  member this.ElapsedMillisecond
    with get ()=this._ElapsedMillisecond
    and set v=
      if this._ElapsedMillisecond<>v then
        this._ElapsedMillisecond<-v
        this.OnPropertyChanged "ElapsedMillisecond"

  [<DV>]val mutable private _ElapsedTime:string
  member this.ElapsedTime
    with get ()=this._ElapsedTime
    and set v=
      if this._ElapsedTime<>v then
        this._ElapsedTime<-v
        this.OnPropertyChanged "ElapsedTime"

  [<DV>]val mutable private _RemainTime:string
  member this.RemainTime
    with get ()=this._RemainTime
    and set v=
      if this._RemainTime<>v then
        this._RemainTime<-v
        this.OnPropertyChanged "RemainTime"

  [<DV>]val mutable private _ProgressValue:int
  member this.ProgressValue
    with get ()=this._ProgressValue
    and set v=
      if this._ProgressValue<>v then
        this._ProgressValue<-v
        this.OnPropertyChanged "ProgressValue"

  [<DV>]val mutable private _ProgressPercent:string
  member this.ProgressPercent
    with get ()=this._ProgressPercent
    and set v=
      if this._ProgressPercent<>v then
        this._ProgressPercent<-v
        this.OnPropertyChanged "ProgressPercent"

  [<DV>]val mutable private _Rate:string
  member this.Rate
    with get ()=this._Rate
    and set v=
      if this._Rate<>v then
        this._Rate<-v
        this.OnPropertyChanged "Rate"

  [<DV>]val mutable private _FinishedTime:DateTime
  member this.FinishedTime
    with get ()=this._FinishedTime
    and set v=
      if this._FinishedTime<>v then
        this._FinishedTime<-v
        this.OnPropertyChanged "FinishedTime"

  [<DV>]val mutable private _IsNewDownload:bool
  member this.IsNewDownload
    with get ()=this._IsNewDownload
    and set v=this._IsNewDownload<-v

  [<DV>]val mutable private _Guid:Guid
  member this.Guid
    with get ()=this._Guid
    and set v=this._Guid<-v

  [<DV>]val mutable private _RateQueue:Generic.Queue<float>
  member this.RateQueue
    with get ()=this._RateQueue
    and set v=this._RateQueue<-v

  [<DV>]val mutable private _PreDownloadedLength:int64
  member this.PreDownloadedLength
    with get ()=this._PreDownloadedLength
    and set v=this._PreDownloadedLength<-v

  [<DV>]val mutable private _Task:AsyncWorker<TaskStatus>
  member this.Task
    with get ()=this._Task
    and set v=this._Task<-v

  [<DV>]val mutable private _TaskStatus:TaskStatus
  member this.TaskStatus
    with get ():TaskStatus=this._TaskStatus
    and set v=this._TaskStatus<-v

  [<DV>]val mutable private _RateNum:int64
  member this.RateNum
    with get ()=this._RateNum
    and set v=this._RateNum<-v

  [<DV>]val mutable private _IsCanResumeDownload:bool
  member this.IsCanResumeDownload
    with get ()=this._IsCanResumeDownload
    and set v=this._IsCanResumeDownload<-v

  [<DV>]val mutable private _DownloadPartials:DownloadPartialInfo[]
  member this.DownloadPartials
    with get ()=this._DownloadPartials
    and set v=this._DownloadPartials<-v
