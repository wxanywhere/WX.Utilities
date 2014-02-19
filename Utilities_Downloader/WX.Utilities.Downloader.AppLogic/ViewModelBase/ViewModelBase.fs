namespace WX.Utilities.Downloader.ViewModel

open System
open System.Windows
open System.Windows.Data
open System.Windows.Input
open System.ComponentModel
open WX.Utilities.Common

type RequestCloseEventArgs (data)=
  inherit EventArgs()
  member this.Data=data

type RequestCloseEventHandler=delegate of obj*RequestCloseEventArgs->unit

type ViewModelBase() =
  let propertyChangedEvent = new DelegateEvent<PropertyChangedEventHandler>()
  let _RequestCloseEvent=new Event<RequestCloseEventHandler,RequestCloseEventArgs>()

  interface INotifyPropertyChanged with
      [<CLIEvent>]
      member this.PropertyChanged = propertyChangedEvent.Publish
  member this.OnPropertyChanged propertyName = 
      propertyChangedEvent.Trigger([| this; new PropertyChangedEventArgs(propertyName) |])
      

  member this.RequestClose:IEvent<RequestCloseEventHandler,RequestCloseEventArgs>=_RequestCloseEvent.Publish
  member private this.OnRequestClose (data:obj)=
    if this.RequestClose<>Null() then
      _RequestCloseEvent.Trigger(this,new RequestCloseEventArgs(data))   


  [<DV>]val mutable private _Title:string
  member this.Title 
    with get()=
      this._Title
    and set v=
      if this._Title<>v then
        this._Title<-v
        this.OnPropertyChanged "Title"

  [<DV>]val mutable private _MainWindow:Window
  member this.MainWindow 
    with get()=
      if this._MainWindow = null then
        this._MainWindow <- Application.Current.MainWindow
      this._MainWindow
    and set v=
      this._MainWindow<-v
      
  [<DV>]val mutable private _MainWindowModel:ViewModelBase
  member this.MainWindowModel
    with get ()=
      if this.MainWindow <> null && this.MainWindow.DataContext :? ViewModelBase then
        this._MainWindowModel<-this._MainWindow.DataContext :?> ViewModelBase
      this._MainWindowModel

  [<DV>]val mutable private _HostWindow:Window
  member this.HostWindow 
    with get()=
      if this._HostWindow = null then
        this._HostWindow <- this.MainWindow
      this._HostWindow
    and private set v=
      this._HostWindow<-v

  [<DV>]val mutable private _ParentModel:ViewModelBase
  member this.ParentModel 
    with get()=
      this._ParentModel
    and private set v=
      this._ParentModel<-v

  member this.ShowDialog (dialogModel:ViewModelBase)=
    let v = new Window()
    let vm = dialogModel
    v.DataContext <- vm
    vm.HostWindow <-v
    vm.ParentModel <- this
    vm.MainWindow <- this.MainWindow
    BindingOperations.SetBinding(v,Window.ContentProperty,new Binding())|>ignore
    BindingOperations.SetBinding(v,Window.TitleProperty,new Binding("Title"))|>ignore
    v.SizeToContent<-SizeToContent.WidthAndHeight
    v.ResizeMode<-ResizeMode.NoResize
    v.Icon<-this.MainWindow.Icon
    v.Owner<-this.HostWindow
    v.WindowStartupLocation<-WindowStartupLocation.CenterOwner
    v.ShowDialog()

  [<DV>]val mutable private _CloseCommand:ICommand
  member this.CloseCommand
    with get ()=
      if this._CloseCommand=null then
        this._CloseCommand<-new RelayCommand(fun e ->
          if this.HostWindow <> null && this.HostWindow.Equals(this.MainWindow)|>not then
            this.HostWindow.Close()
          this.OnRequestClose(e)
          )
      this._CloseCommand