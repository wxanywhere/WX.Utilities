namespace WX.Utilities.WPFDesignerX.BusinessEditor
open System
open System.IO
open System.Windows
open System.Windows.Data
open System.Windows.Controls
open System.Windows.Input
open Microsoft.Win32
open WX

type FVM_DataFilePathEditor()=
  inherit ViewModelBase()
  let mutable _DialogResult=false

  member this.Initialize()=
    this.Title<-"数据文件路径编辑"

  member this.DialogResult  
    with get ()=_DialogResult

  [<DV>] val mutable private _D_FilePath :string
  member this.D_FilePath  
    with get ()=this._D_FilePath 
    and set v=
      if this._D_FilePath <>v then
        this._D_FilePath <-v
        this.OnPropertyChanged "D_FilePath"

  [<DV>]val mutable _CMD_FilePath:ICommand
  member this.CMD_FilePath
    with get ()=
      if this. _CMD_FilePath=null then
        this. _CMD_FilePath<-new RelayCommand(
          (fun _->
            match new OpenFileDialog() with
            | x ->
                x.Multiselect<-false
                x.FileName<-""
                x.DefaultExt<-".xml"
                x.Filter<-"xml document (*.xml)|*.xml"
                x.CheckFileExists<-true
                match x.ShowDialog() with
                | y when y.HasValue && y.Value ->
                    this.D_FilePath<-x.FileName
                | _ ->()
            ),
          (fun _ ->true))
      this. _CMD_FilePath  

  [<DV>]val mutable _CMD_OK:ICommand
  member this.CMD_OK
    with get ()=
      if this. _CMD_OK=null then
        this. _CMD_OK<-new RelayCommand(
          (fun _->
            _DialogResult<-true
            this.CloseCommand.Execute(this.D_FilePath)
            ),
          (fun _ ->File.Exists this.D_FilePath))
      this. _CMD_OK 