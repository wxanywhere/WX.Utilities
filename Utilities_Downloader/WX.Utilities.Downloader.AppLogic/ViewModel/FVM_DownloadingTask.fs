namespace WX.Utilities.Downloader.ViewModel

open System
open System.Text
open System.Xml
open System.Xml.Linq
open System.Timers
open System.IO
open System.Net
open System.Collections
open System.Collections.ObjectModel
open System.Threading
open System.Windows
open System.Windows.Data
open System.Windows.Input
open System.ComponentModel
open System.Diagnostics
open WX.Utilities.Common
open WX.Utilities.Downloader.Model

#nowarn "40"


type FVM_DownloadingTask() as this=
  inherit ViewModelBase()

  let RunTasks ()=
    let downloadingNumber=
      this.D_DownloadingFiles
      |>Seq.filter (fun a->a.TaskStatus=TaskStatus.Downloading)
      |>Seq.length
    match Config.MaxDownloadingNumber,downloadingNumber with
    | xa,xb when xa>xb ->
        match 
          xa-xb,
          this.D_DownloadingFiles
          |>Seq.filter(fun a->a.TaskStatus=TaskStatus.LineUp)
          with
        | ya, yb when Seq.length yb>0->
            if ya>Seq.length yb then
              yb|>Seq.iter(fun a->this.Download a)
            else yb|>Seq.take ya|>Seq.iter(fun a->this.Download a)
        | _ ->()
    | _ ->()

  
  [<DV>]val mutable private _D_DownloadingFiles:ObservableCollection<DownloadFileInfo>
  member this.D_DownloadingFiles
    with get ():ObservableCollection<DownloadFileInfo>=this._D_DownloadingFiles
    and set v=this._D_DownloadingFiles<-v

  [<DV>]val mutable _D_DownloadingFile:DownloadFileInfo
  member this.D_DownloadingFile
    with get()= this._D_DownloadingFile
    and set v=
      if v<>this._D_DownloadingFile then
        this._D_DownloadingFile<-v
        this.OnPropertyChanged "D_DownloadingFile"

  [<DV>]val mutable _CMD_NewTaskes:ICommand 
  member this.CMD_NewTaskes
    with get ()=
      if this._CMD_NewTaskes=null then
        this._CMD_NewTaskes<-new RelayCommand(fun _ ->
          match new FVM_NewDownloadTask() with
          | x ->
              x.RequestClose.Add(fun e ->
                match e.Data with
                | :? (DownloadFileInfo[]) as y ->
                  y
                  |>Seq.iter(fun a->
                    this.D_DownloadingFiles.Add a
                    )
                  RunTasks ()
                | _ ->()
                )
              this.ShowDialog(x)|>ignore
          )
      this._CMD_NewTaskes

  [<DV>]val mutable _CMD_Run:ICommand 
  member this.CMD_Run
    with get ()=
      if this._CMD_Run=null then
        this._CMD_Run<-new RelayCommand((fun _ ->
          this.D_DownloadingFiles
          |>Seq.filter (fun a->a.IsSelected && a.TaskStatus<>TaskStatus.Downloading)
          |>Seq.iter (fun a->
             a.TaskStatus<-TaskStatus.LineUp
             a.Rate<-string TaskStatus.LineUp
             )
          RunTasks ()
          ),
          (fun _ ->
            this.D_DownloadingFiles|>Seq.exists(fun a->a.IsSelected && (a.TaskStatus=Paused || a.TaskStatus=Error)) &&
            this.D_DownloadingFiles|>Seq.exists(fun a->a.IsSelected && a.TaskStatus=Pausing)|>not 
            ))
      this._CMD_Run

  [<DV>]val mutable _CMD_Pause:ICommand 
  member this.CMD_Pause
    with get ()=
      if this._CMD_Pause=null then
        this._CMD_Pause<-new RelayCommand((fun _ ->
          this.D_DownloadingFiles
          |>Seq.filter (fun a->a.IsSelected && (a.TaskStatus=Downloading || a.TaskStatus=LineUp))
          |>Seq.iter (fun a->
             match a.TaskStatus with
             | Downloading ->
               a.TaskStatus<-TaskStatus.Pausing
               a.Rate<-string TaskStatus.Pausing
             | _ ->
               a.TaskStatus<-TaskStatus.Paused
               a.Rate<-string TaskStatus.Paused
             )
          ),
          (fun _ ->
            this.D_DownloadingFiles
            |>Seq.exists(fun a->a.IsSelected && (a.TaskStatus=Downloading || a.TaskStatus=LineUp))))
      this._CMD_Pause

  [<DV>]val mutable _CMD_Delete:ICommand 
  member this.CMD_Delete
    with get ()=
      if this._CMD_Delete=null then
        this._CMD_Delete<-new RelayCommand((fun _ ->
          match MessageBox.Show("选定的下载任务将被删除，是否继续？","操作提示",MessageBoxButton.YesNo,MessageBoxImage.Question) with
          | MessageBoxResult.Yes ->
              this.D_DownloadingFiles
              |>Seq.filter (fun a->a.IsSelected)
              |>Seq.toArray
              |>Array.iter (fun a->
                 a.TaskStatus<-TaskStatus.Deleting
                 a.Rate<-string TaskStatus.Deleting
                 this.D_DownloadingFiles.Remove a|>ignore
                 )
          | _ ->()
          ),
          (fun _ ->this.D_DownloadingFiles|>Seq.exists(fun a->a.IsSelected)))
      this._CMD_Delete

  [<DV>]val mutable _CMD_Folder:ICommand 
  member this.CMD_Folder
    with get ()=
      if this._CMD_Folder=null then
        this._CMD_Folder<-new RelayCommand((fun _ ->
          match this.D_DownloadingFile with
          | NotNull xa ->
              if File.Exists (xa.FilePath+Value.DldFileSuffix) then
                try
                  Process.Start("Explorer.exe", "/SELECT,"+xa.FilePath+Value.DldFileSuffix)|>ignore
                with _ ->()
          | _ ->()
          ),
          (fun a_ ->this.D_DownloadingFile<>Null()))
      this._CMD_Folder

  member this.Download(downloadFileInfo:DownloadFileInfo)=
    ServicePointManager.DefaultConnectionLimit <- 50;
    let handler:ElapsedEventHandler option ref=ref None
    match downloadFileInfo,downloadFileInfo.ElapsedMillisecond with
    | xf, xe ->
        let CreateDownloadPartialInfoCfg()=
          use xw= new XmlTextWriter(xf.FilePath+Value.CfgFileSuffix, Encoding.UTF8) //or use vx=XmlWriter.Create(XmlData.FilePath)
          xw.Formatting<-Formatting.Indented
          xw.WriteStartDocument()
          xw.WriteStartElement(XmlData.Prefix,"PartialInfos",XmlData.NS)
          for m in 0..xf.DownloadPartials.Length-1 do
            xw.WriteStartElement (XmlData.Prefix,"PartialInfo",XmlData.NS)
            xw.WriteAttributeString(XmlData.Prefix,"ID",XmlData.NS,string m)
            xw.WriteAttributeString(XmlData.Prefix,"PartialLength",XmlData.NS,string xf.DownloadPartials.[m].PartialLength)
            xw.WriteAttributeString(XmlData.Prefix,"PartialEndPosition",XmlData.NS,string xf.DownloadPartials.[m].PartialEndPosition)
            xw.WriteAttributeString(XmlData.Prefix,"DownloadedLength",XmlData.NS,string xf.DownloadPartials.[m].DownloadedLength)
            xw.WriteAttributeString(XmlData.Prefix,"DownloadPosition",XmlData.NS,string xf.DownloadPartials.[m].DownloadPosition)
            xw.WriteEndElement()
          xw.WriteEndElement()
          xw.WriteEndDocument()
        let UpdateDownloadInfoCfg()=
          match
            XmlData.XDownloadingFileInfos.Value.Elements(XName.Get("FileInfo",XmlData.NS))
            |>Seq.tryFind(fun a->
                match a.Attribute(XName.Get("Guid",XmlData.NS)) with
                | NotNull va ->
                    match Guid.TryParse va.Value with
                    | true, vb ->vb=xf.Guid
                    | _ ->false
                | _ ->false
                )
            with
          | Some u ->
              u.SetAttributeValue(XName.Get("ElapsedMillisecond",XmlData.NS),xf.ElapsedMillisecond)
              u.SetAttributeValue(XName.Get("ProgressValue",XmlData.NS),xf.ProgressValue)
              u.SetAttributeValue(XName.Get("ProgressPercent",XmlData.NS),xf.ProgressPercent)
              u.SetAttributeValue(XName.Get("PreDownloadedLength",XmlData.NS),xf.PreDownloadedLength)
              XmlData.SaveXDoc()|>ignore
          | _ ->()
        let AddDownloadInfoCfg()=
          match new XElement(XName.Get("FileInfo",XmlData.NS)) with
          | xa->
              xa.SetAttributeValue(XName.Get("Guid",XmlData.NS),xf.Guid)
              xa.SetAttributeValue(XName.Get("FileName",XmlData.NS),xf.FileName)
              xa.SetAttributeValue(XName.Get("FileExtension",XmlData.NS),xf.FileExtension)
              xa.SetAttributeValue(XName.Get("FilePath",XmlData.NS),xf.FilePath)
              xa.SetAttributeValue(XName.Get("FolderPath",XmlData.NS),xf.FolderPath)
              xa.SetAttributeValue(XName.Get("DownloadPath",XmlData.NS),xf.DownloadPath)
              xa.SetAttributeValue(XName.Get("FileSize",XmlData.NS),xf.FileSize)
              xa.SetAttributeValue(XName.Get("FileLength",XmlData.NS),xf.FileLength)
              xa.SetAttributeValue(XName.Get("ElapsedMillisecond",XmlData.NS),xf.ElapsedMillisecond)
              xa.SetAttributeValue(XName.Get("ProgressValue",XmlData.NS),xf.ProgressValue)
              xa.SetAttributeValue(XName.Get("ProgressPercent",XmlData.NS),xf.ProgressPercent)
              xa.SetAttributeValue(XName.Get("PreDownloadedLength",XmlData.NS),xf.PreDownloadedLength)
              XmlData.XDownloadingFileInfos.Value.AddFirst xa
              XmlData.SaveXDoc()|>ignore
        let DeleteDownloadInfoCfg()=
          match
            XmlData.XDownloadingFileInfos.Value.Elements(XName.Get("FileInfo",XmlData.NS))
            |>Seq.tryFind(fun a->
                match a.Attribute(XName.Get("Guid",XmlData.NS)) with
                | NotNull va ->
                    match Guid.TryParse va.Value with
                    | true, vb ->vb=xf.Guid
                    | _ ->false
                | _ ->false
                )
            with
          | Some u ->
              u.Remove()
              XmlData.SaveXDoc()|>ignore
          | _ ->()
        let (fs,temFileInfo),cfgFileInfo=
          match xf.FilePath+Value.DldFileSuffix,xf.FilePath+Value.CfgFileSuffix with
          | xa, xb ->
              match File.Exists xa, File.Exists xb with 
              | true,true when xf.IsNewDownload|>not-> 
                  new FileStream(xa,FileMode.Open),FileInfo xa
              | _ -> 
                  xf.IsNewDownload<-true
                  new FileStream(xa,FileMode.Create),FileInfo xa
              , FileInfo xb
        xf.TaskStatus<-TaskStatus.Downloading
        xf.Rate<-string TaskStatus.Downloading
        let startTickCount=Environment.TickCount
        try
          let UpdateDownloadInfo (downloadedLength:int64) =
            match downloadedLength with
            | y ->
                match int (float y/float xf.FileLength*1000.0) with
                | z ->
                    xf.ProgressValue<-z/10
                    xf.ProgressPercent<-String.Format("{0:P1}",float z/1000.0)
                match y-xf.PreDownloadedLength, xf.RateQueue with
                | za,zb ->
                    zb.Enqueue (float za)
                    if zb.Count>2 then zb.Dequeue()|>ignore  
                    match zb|>Seq.average|>int64 with
                    | 0L ->
                        xf.RemainTime<-"--:--:--"
                        xf.Rate<-string TaskStatus.Downloading
                    | w ->
                        xf.RemainTime<-(xf.FileLength-y)/w|>float|>TimeSpan.FromSeconds|>string
                        xf.Rate<-String.Format("{0}/s",w|>BusinessComm.GetSizeStr)
                        xf.RateNum<-w
                xf.ElapsedMillisecond<-xe+Environment.TickCount-startTickCount
                xf.ElapsedTime<-xf.ElapsedMillisecond/1000|>float|>TimeSpan.FromSeconds|>string
                xf.PreDownloadedLength<-y       
          let rec work=
            let isCanResumeDownload=ref false 
            let AsyncPreDownload()=
              async{
                try
                  let request=HttpWebRequest.Create(xf.DownloadPath):?>HttpWebRequest
                  use! rep=request.AsyncGetResponse()
                  isCanResumeDownload:=true //Temp
                  if !isCanResumeDownload|>not then xf.IsNewDownload<-true
                  xf.RateQueue<-new Generic.Queue<_>(2)
                  match xf.IsNewDownload with
                  | true ->
                      xf.IsNewDownload<-false
                      let headerInfo=rep.Headers.Get("Content-Length")
                      match Int64.TryParse(headerInfo) with
                      | (true, xa)->
                          xf.FileSize<-BusinessComm.GetSizeStr xa
                          xf.FileLength<-xa
                          fs.SetLength xa
                          xf.DownloadPartials<-
                            seq{
                              match !isCanResumeDownload, xa>1048576L*5L ,xa/int64 Config.FileTheadNumber, xa % int64 Config.FileTheadNumber with
                              | true,true,xb,xc ->
                                  let lastPartialLength=xb+xc
                                  for m in 0..Config.FileTheadNumber-1 do
                                    match new DownloadPartialInfo() with
                                    | y ->
                                        y.ID<-m
                                        if Config.FileTheadNumber-1=m then 
                                          y.PartialLength<-lastPartialLength
                                          y.PartialEndPosition<-xb*int64 m+lastPartialLength-1L
                                        else 
                                          y.PartialLength<-xb
                                          y.PartialEndPosition<-xb*int64(m+1)-1L
                                        y.DownloadPosition<-xb*int64 m
                                        yield y
                              | _ ->
                                  match new DownloadPartialInfo() with
                                  | y ->
                                      y.ID<-0
                                      y.PartialLength<-xf.FileLength
                                      y.PartialEndPosition<-xf.FileLength-1L
                                      yield y
                            }
                            |>Seq.toArray
                          AddDownloadInfoCfg()
                          CreateDownloadPartialInfoCfg()
                      | _ ->() //do:: 须增加错误处理
                  | _ ->
                      //do::从配置文件中获取下载信息
                      ()
                  rep.Close();request.Abort()
                with e ->
                  match !handler with
                  | Some h ->BusinessComm.Timer.Elapsed.RemoveHandler h
                  | _ ->()
                  raise e
              }
            let AsyncDownload()=
              handler:=new ElapsedEventHandler(fun _ _ ->
                if xf.TaskStatus=TaskStatus.Downloading then
                  task.ReportProgress 0
                  )|>Some
              BusinessComm.Timer.Elapsed.AddHandler (!handler|>Option.get)
              seq{
                for m in xf.DownloadPartials do
                  if m.DownloadPosition>m.PartialEndPosition|>not then
                    yield
                      async{
                        try 
                          let request=HttpWebRequest.Create(xf.DownloadPath):?>HttpWebRequest
                          if !isCanResumeDownload then request.AddRange(m.DownloadPosition,m.PartialEndPosition)
                          use! rep= request.AsyncGetResponse()
                          use resm=rep.GetResponseStream()
                          let buffer=Array.zeroCreate<byte> (Config.BufferSize*1024L|>int)
                          let dataLength=ref 1
                          while !dataLength>0 && xf.TaskStatus=Downloading do
                            if m.PartialLength>m.DownloadedLength then
                              if (m.PartialLength-m.DownloadedLength)/(Config.BufferSize*1024L)>0L then
                                let! temLength= resm.AsyncRead(buffer,0,buffer.Length)
                                dataLength:=temLength
                              else 
                                dataLength:=resm.Read(buffer,0,(m.PartialLength-m.DownloadedLength) % (Config.BufferSize*1024L)|>int)
                              lock fs (fun ()->
                                fs.Position<-m.PartialEndPosition+1L-m.PartialLength+ m.DownloadedLength
                                fs.Write(buffer,0,!dataLength)
                                m.DownloadedLength<-m.DownloadedLength+int64 !dataLength
                                )
                            else dataLength:=0
                          m.DownloadPosition<-m.PartialEndPosition+1L-m.PartialLength+ m.DownloadedLength
                          rep.Close();resm.Close();request.Abort()
                        with e ->raise e
                      }
              }
              |>Async.Parallel
            async{
              try
                do! AsyncPreDownload()
                let!_= AsyncDownload()
                do()
              with e ->
                match !handler with
                | Some h ->BusinessComm.Timer.Elapsed.RemoveHandler h
                | _ ->()
                fs.Close()
                UpdateDownloadInfoCfg()
                CreateDownloadPartialInfoCfg()
                raise e
              return xf.TaskStatus
            }           
          and task:AsyncWorker<_>=
            match new AsyncWorker<_>(work) with
            | x ->
                xf.Task<-x
                x
          task.Error.Add(fun _ ->
            match !handler with
            | Some h ->BusinessComm.Timer.Elapsed.RemoveHandler h
            | _ ->()
            fs.Close()
            UpdateDownloadInfoCfg()
            CreateDownloadPartialInfoCfg()
            xf.TaskStatus<-TaskStatus.Error
            xf.Rate<-string TaskStatus.Error
            xf.RemainTime<-"--:--:--"
            xf.PreDownloadedLength<-0L
            if xf.RateQueue<>null then xf.RateQueue.Clear()
            )
          task.Completed.Add(fun r->
            RunTasks()
            match !handler with
            | Some h ->BusinessComm.Timer.Elapsed.RemoveHandler h
            | _ ->()
            fs.Close()
            match r with
            | Downloading ->
                xf.TaskStatus<-TaskStatus.Downloaded
                xf.RemainTime<-String.Empty
                xf.FinishedTime<-DateTime.Now
                this.D_DownloadingFiles.Remove xf|>ignore
                async{
                  UpdateDownloadInfo (xf.FileLength)
                  let count=ref 1
                  let fileName=ref xf.FileName
                  while File.Exists xf.FilePath do
                    fileName:=xf.FileName.Insert(xf.FileName.LastIndexOf('.'),String.Format("({0})",!count))
                    xf.FilePath<-Path.Combine(xf.FolderPath,!fileName)
                    incr count
                  xf.FileName<- !fileName
                  temFileInfo.MoveTo(xf.FilePath)
                  cfgFileInfo.Delete()
                  DeleteDownloadInfoCfg()
                }
                |>Async.Start
            | Pausing ->
                xf.TaskStatus<-TaskStatus.Paused
                xf.Rate<-string TaskStatus.Paused
                xf.RemainTime<-"--:--:--"
                async{
                  UpdateDownloadInfoCfg()
                  CreateDownloadPartialInfoCfg()
                }
                |>Async.Start
            | Deleting ->
                async{
                  match xf.FilePath+Value.DldFileSuffix,xf.FilePath+Value.CfgFileSuffix with
                  | xa, xb ->
                      File.Delete xa; File.Delete xb
                  DeleteDownloadInfoCfg()
                }
                |>Async.Start
            | _ ->()
            )
          task.ProgressChanged.Add(fun _->
            try
              if xf.TaskStatus=TaskStatus.Downloading then
                match xf.DownloadPartials|>Seq.sumBy(fun a->a.DownloadedLength) with
                | x ->
                    UpdateDownloadInfo x 
              with _ ->
                match !handler with
                | Some h ->BusinessComm.Timer.Elapsed.RemoveHandler h
                | _ ->()
            )
          task.RunAsync()|>ignore
        with _ ->
          match !handler with
          | Some h ->BusinessComm.Timer.Elapsed.RemoveHandler h
          | _ ->()
          fs.Close()
          UpdateDownloadInfoCfg()
          CreateDownloadPartialInfoCfg()
(*
http://dldir1.qq.com/qqfile/qq/QQ2013/QQ2013/7690/QQ2013.exe
http://down.sandai.net/thunder7/Thunder_dl_7.9.8.4550.exe
http://mirrors.grandcloud.cn/centos/6.4/isos/x86_64/CentOS-6.4-x86_64-bin-DVD1.iso
*)

(*
use rep=
  try
    request.AddRange(1)
    match request.GetResponse() with
    | x ->
        isCanResumeDownload:=true
        x.Close()
        request<-HttpWebRequest.Create(xf.DownloadPath):?>HttpWebRequest
        request.GetResponse()
  with _->
    request.GetResponse()


[<DV>]val mutable _CMD_Pause:ICommand 
member this.CMD_Pause
  with get ()=
    if this._CMD_Pause=null then
      this._CMD_Pause<-new RelayCommand((fun _ ->
        this.D_DownloadingFiles
        |>Seq.filter (fun a->a.IsSelected && (a.TaskStatus=Downloading || a.TaskStatus=LineUp))
        |>Seq.toArray
        |>Array.iter (fun a->
           match a.TaskStatus with
           | Downloading ->a.Task.CancelAsync()
           | _ ->()
           a.TaskStatus<-TaskStatus.Pausing
           a.Rate<-string TaskStatus.Pausing
           )
        ),
        (fun _ ->
          this.D_DownloadingFiles
          |>Seq.exists(fun a->a.IsSelected && (a.TaskStatus=Downloading || a.TaskStatus=LineUp))))
    this._CMD_Pause

task.Canceled.Add(fun _ ->
  match !handler with
  | Some h ->BusinessComm.Timer.Elapsed.RemoveHandler h
  | _ ->()
  fs.Close();fsCfg.Close()
  xf.TaskStatus<-TaskStatus.Paused
  xf.Rate<-string TaskStatus.Paused
  xf.RemainTime<-"--:--:--"
  )
*)