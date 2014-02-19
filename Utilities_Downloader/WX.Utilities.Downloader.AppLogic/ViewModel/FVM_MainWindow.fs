namespace WX.Utilities.Downloader.ViewModel

open System
open System.IO
open System.Text
open System.Xml
open System.Xml.Linq
open System.Collections
open System.Collections.Specialized
open System.Collections.ObjectModel
open System.Windows
open System.Windows.Data
open System.Windows.Input
open System.ComponentModel
open WX.Utilities.Common
open WX.Utilities.Downloader.Model

type FVM_MainWindow() as this=
  inherit ViewModelBase()

  let _Workspaces=new Generic.List<ViewModelBase>()
  let _DownloadingFileInfos=new ObservableCollection<DownloadFileInfo>()
  let _DownloadedFileInfos=new ObservableCollection<DownloadFileInfo>()
 
  do this.Initialized()

  member this.Initialized()=
    match 
      match XmlData.XDoc with
      | None  ->
          if File.Exists XmlData.FilePath|>not then this.CreateDownloadInfoCfg()
          match 
            (
            try
              match XDocument.Load(XmlData.FilePath) with
              | NotNull x ->Some x
              | _ ->None
            with _ ->None)
            with
          | Some x ->XmlData.XDoc<-Some x; Some x
          | _ ->None
      | x ->x
      with
    | Some x ->
        match x.Descendants(XName.Get("DownloadingFileInfos",XmlData.NS)) with
        | y when Seq.length y=1 ->
            XmlData.XDownloadingFileInfos<-  y|>Seq.head|>Some
        | _ ->()
        match x.Descendants(XName.Get("DownloadedFileInfos",XmlData.NS)) with
        | y when Seq.length y=1 ->
            XmlData.XDownloadedFileInfos<-  y|>Seq.head|>Some
        | _ ->()
        this.RetrieveDownloadInfo()
    | _ ->
        match MessageBox.Show ( @"标注数据加载失败, 是否关闭当前窗口！","错误",MessageBoxButton.YesNo,MessageBoxImage.Question) with
        | MessageBoxResult.Yes ->this.CloseCommand.Execute()
        | _ ->()

  member this.RetrieveDownloadInfo ()=
    match XmlData.XDownloadingFileInfos,XmlData.XDownloadedFileInfos with
    | Some xa,Some xb  ->
        seq{
          for m in xa.Elements(XName.Get("FileInfo",XmlData.NS)) do
            match m.Attribute(XName.Get("FilePath",XmlData.NS)) with
            | NotNull wa ->
                match File.Exists (wa.Value+Value.CfgFileSuffix) with
                | true ->
                    match new DownloadFileInfo() with
                    | ya ->
                        ya.IsNewDownload<-false
                        ya.IsCanResumeDownload<-true
                        ya.TaskStatus<-TaskStatus.Paused
                        ya.Rate<-string TaskStatus.Paused
                        match m.Attribute(XName.Get("Guid",XmlData.NS)) with
                        | NotNull wa ->
                            match Guid.TryParse wa.Value with
                            | true, wb ->ya.Guid<-wb
                            | _ ->()
                        | _ ->()
                        match m.Attribute(XName.Get("FileName",XmlData.NS)) with
                        | NotNull wa ->ya.FileName<-wa.Value
                        | _ ->()
                        match m.Attribute(XName.Get("FileExtension",XmlData.NS)) with
                        | NotNull wa ->ya.FileExtension<-wa.Value
                        | _ ->()
                        match m.Attribute(XName.Get("FilePath",XmlData.NS)) with
                        | NotNull wa ->ya.FilePath<-wa.Value
                        | _ ->()
                        match m.Attribute(XName.Get("FolderPath",XmlData.NS)) with
                        | NotNull wa ->ya.FolderPath<-wa.Value
                        | _ ->()
                        match m.Attribute(XName.Get("DownloadPath",XmlData.NS)) with
                        | NotNull wa ->ya.DownloadPath<-wa.Value
                        | _ ->()
                        match m.Attribute(XName.Get("FileSize",XmlData.NS)) with
                        | NotNull wa ->ya.FileSize<-wa.Value
                        | _ ->()
                        match m.Attribute(XName.Get("FileLength",XmlData.NS)) with
                        | NotNull wa ->
                            match Int64.TryParse wa.Value with
                            | true, wb ->ya.FileLength<-wb
                            | _ ->()
                        | _ ->()
                        match m.Attribute(XName.Get("ElapsedMillisecond",XmlData.NS)) with
                        | NotNull wa ->
                            match Int32.TryParse wa.Value with
                            | true, wb ->ya.ElapsedMillisecond<-wb
                            | _ ->()
                        | _ ->()
                        match m.Attribute(XName.Get("ProgressValue",XmlData.NS)) with
                        | NotNull wa ->
                            match Int32.TryParse wa.Value with
                            | true, wb ->ya.ProgressValue<-wb
                            | _ ->()
                        | _ ->()
                        match m.Attribute(XName.Get("ProgressPercent",XmlData.NS)) with
                        | NotNull wa ->ya.ProgressPercent<-wa.Value
                        | _ ->()
                        match m.Attribute(XName.Get("PreDownloadedLength",XmlData.NS)) with
                        | NotNull wa ->
                            match Int64.TryParse wa.Value with
                            | true, wb ->ya.PreDownloadedLength<-wb
                            | _ ->()
                        | _ ->()
                        ya.DownloadPartials<-
                          seq{
                            match XDocument.Load(ya.FilePath+Value.CfgFileSuffix) with
                            | NotNull wa ->
                                match wa.Descendants(XName.Get("PartialInfos",XmlData.NS)) with
                                | wb when Seq.length wb=1 ->
                                    let wc=wb|>Seq.head
                                    for n in wc.Elements(XName.Get("PartialInfo",XmlData.NS)) do
                                      match new DownloadPartialInfo() with
                                      | yb ->
                                          match n.Attribute(XName.Get("ID",XmlData.NS)) with
                                          | NotNull wa ->
                                              match Int32.TryParse wa.Value with
                                              | true, wb ->yb.ID<-wb
                                              | _ ->()
                                          | _ ->()
                                          match n.Attribute(XName.Get("PartialLength",XmlData.NS)) with
                                          | NotNull wa ->
                                              match Int64.TryParse wa.Value with
                                              | true, wb ->yb.PartialLength<-wb
                                              | _ ->()
                                          | _ ->()
                                          match n.Attribute(XName.Get("PartialEndPosition",XmlData.NS)) with
                                          | NotNull wa ->
                                              match Int64.TryParse wa.Value with
                                              | true, wb ->yb.PartialEndPosition<-wb
                                              | _ ->()
                                          | _ ->()
                                          match n.Attribute(XName.Get("DownloadedLength",XmlData.NS)) with
                                          | NotNull wa ->
                                              match Int64.TryParse wa.Value with
                                              | true, wb ->yb.DownloadedLength<-wb
                                              | _ ->()
                                          | _ ->()
                                          match n.Attribute(XName.Get("DownloadPosition",XmlData.NS)) with
                                          | NotNull wa ->
                                              match Int64.TryParse wa.Value with
                                              | true, wb ->yb.DownloadPosition<-wb
                                              | _ ->()
                                          | _ ->()
                                          yield yb
                                | _ ->()
                            | _ ->()
                          }
                          |>Seq.toArray
                        yield ya
                | _ ->
                    m.Remove()
                    XmlData.SaveXDoc()|>ignore
            | _ ->()
        }
        |>Seq.iter (fun a->_DownloadingFileInfos.Add a)
        seq{
          for m in xb.Elements(XName.Get("FileInfo",XmlData.NS)) do
            match new DownloadFileInfo() with
            | ya ->
                ya.IsNewDownload<-false
                ya.IsCanResumeDownload<-true
                ya.TaskStatus<-TaskStatus.Downloaded
                match m.Attribute(XName.Get("Guid",XmlData.NS)) with
                | NotNull wa ->
                    match Guid.TryParse wa.Value with
                    | true, wb ->ya.Guid<-wb
                    | _ ->()
                | _ ->()
                match m.Attribute(XName.Get("FileName",XmlData.NS)) with
                | NotNull wa ->ya.FileName<-wa.Value
                | _ ->()
                match m.Attribute(XName.Get("FileExtension",XmlData.NS)) with
                | NotNull wa ->ya.FileExtension<-wa.Value
                | _ ->()
                match m.Attribute(XName.Get("FilePath",XmlData.NS)) with
                | NotNull wa ->ya.FilePath<-wa.Value
                | _ ->()
                match m.Attribute(XName.Get("FolderPath",XmlData.NS)) with
                | NotNull wa ->ya.FolderPath<-wa.Value
                | _ ->()
                match m.Attribute(XName.Get("DownloadPath",XmlData.NS)) with
                | NotNull wa ->ya.DownloadPath<-wa.Value
                | _ ->()
                match m.Attribute(XName.Get("FileSize",XmlData.NS)) with
                | NotNull wa ->ya.FileSize<-wa.Value
                | _ ->()
                match m.Attribute(XName.Get("FileLength",XmlData.NS)) with
                | NotNull wa ->
                    match Int64.TryParse wa.Value with
                    | true, wb ->ya.FileLength<-wb
                    | _ ->()
                | _ ->()
                match m.Attribute(XName.Get("ElapsedMillisecond",XmlData.NS)) with
                | NotNull wa ->
                    match Int32.TryParse wa.Value with
                    | true, wb ->ya.ElapsedMillisecond<-wb
                    | _ ->()
                | _ ->()
                match m.Attribute(XName.Get("FinishedTime",XmlData.NS)) with
                | NotNull wa ->
                    match DateTime.TryParse wa.Value with
                    | true, wb ->ya.FinishedTime<-wb
                    | _ ->()
                | _ ->()
                yield ya
        }
        |>Seq.iter (fun a->_DownloadedFileInfos.Add a)
    | _ ->()

  member this.CreateDownloadInfoCfg()=
    use xw= new XmlTextWriter(XmlData.FilePath , Encoding.UTF8) //or use vx=XmlWriter.Create(XmlData.FilePath)
    xw.Formatting<-Formatting.Indented
    xw.WriteStartDocument()
    xw.WriteStartElement(XmlData.Prefix,"DownloadInfo",XmlData.NS)
    xw.WriteStartElement (XmlData.Prefix,"DownloadingFileInfos",XmlData.NS)
    xw.WriteEndElement()
    xw.WriteStartElement (XmlData.Prefix,"DownloadedFileInfos",XmlData.NS)
    xw.WriteEndElement()
    xw.WriteEndElement()
    xw.WriteEndDocument()

  [<DV>]val mutable _Workspace:ViewModelBase
  member this.Workspace
    with get()= this._Workspace
    and set v=
      if v<>this._Workspace then
        this._Workspace<-v
        this.OnPropertyChanged "Workspace"

  [<DV>]val mutable _DownloadRate:string
  member this.DownloadRate
    with get()= this._DownloadRate
    and set v=
      if v<>this._DownloadRate then
        this._DownloadRate<-v
        this.OnPropertyChanged "DownloadRate"

  [<DV>]val mutable _CMD_Downloading:ICommand 
  member this.CMD_Downloading
    with get ():ICommand=
      if this._CMD_Downloading=null then
        this._CMD_Downloading<-new RelayCommand(fun _ ->
          match _Workspaces|>Seq.tryFind (fun a->a :? FVM_DownloadingTask) with
          | Some x ->this.Workspace<-x
          | _ ->
              match new FVM_DownloadingTask() with
              | x ->
                  x.D_DownloadingFiles<-_DownloadingFileInfos
                  BusinessComm.Timer.Elapsed.Add(fun _ ->
                    this.DownloadRate<-
                      if x.D_DownloadingFiles|>Seq.exists (fun a->a.TaskStatus=Downloading) then
                        x.D_DownloadingFiles
                        |>Seq.filter(fun a->a.TaskStatus=Downloading)
                        |>Seq.sumBy (fun a->a.RateNum)
                        |>BusinessComm.GetSizeStr
                      else BusinessComm.GetSizeStr 0L
                    )
                  x.D_DownloadingFiles.CollectionChanged.Add(fun a->
                    match a.Action with
                    | NotifyCollectionChangedAction.Remove ->
                        match 
                          match _Workspaces|>Seq.tryFind (fun a->a :? FVM_FinishedTaskes) with
                          | Some (:? FVM_FinishedTaskes as x)  ->x
                          | _ ->
                              match new FVM_FinishedTaskes() with
                              | x ->
                                  x.D_DownloadedFiles<-_DownloadedFileInfos
                                  _Workspaces.Add x
                                  x
                          with
                        | x ->
                            for m in a.OldItems do
                              match m with
                              | :? DownloadFileInfo as xa when xa.TaskStatus=TaskStatus.Downloaded ->
                                  x.D_DownloadedFiles.Insert(0,xa)
                                  match new XElement(XName.Get("FileInfo",XmlData.NS)) with
                                  | xb->
                                      xb.SetAttributeValue(XName.Get("Guid",XmlData.NS),xa.Guid)
                                      xb.SetAttributeValue(XName.Get("FileName",XmlData.NS),xa.FileName)
                                      xb.SetAttributeValue(XName.Get("FileExtension",XmlData.NS),xa.FileExtension)
                                      xb.SetAttributeValue(XName.Get("FilePath",XmlData.NS),xa.FilePath)
                                      xb.SetAttributeValue(XName.Get("FolderPath",XmlData.NS),xa.FolderPath)
                                      xb.SetAttributeValue(XName.Get("DownloadPath",XmlData.NS),xa.DownloadPath)
                                      xb.SetAttributeValue(XName.Get("FileSize",XmlData.NS),xa.FileSize)
                                      xb.SetAttributeValue(XName.Get("FileLength",XmlData.NS),xa.FileLength)
                                      xb.SetAttributeValue(XName.Get("ElapsedMillisecond",XmlData.NS),xa.ElapsedMillisecond)
                                      xb.SetAttributeValue(XName.Get("FinishedTime",XmlData.NS),xa.FinishedTime)
                                      XmlData.XDownloadedFileInfos.Value.AddFirst xb
                                      XmlData.SaveXDoc()|>ignore
                              | _ ->()
                    | _ ->()
                    )
                  _Workspaces.Add x
                  this.Workspace<-x
          )  
      this._CMD_Downloading

  [<DV>]val mutable _CMD_Downloaded:ICommand 
  member this.CMD_Downloaded
    with get ()=
      if this._CMD_Downloaded=null then
        this._CMD_Downloaded<-new RelayCommand(fun _ ->
          match _Workspaces|>Seq.tryFind (fun a->a :? FVM_FinishedTaskes) with
          | Some x ->this.Workspace<-x
          | _ ->
              match new FVM_FinishedTaskes() with
              | x ->
                  x.D_DownloadedFiles<-_DownloadedFileInfos
                  _Workspaces.Add x
                  this.Workspace<-x
          )  
      this._CMD_Downloaded
