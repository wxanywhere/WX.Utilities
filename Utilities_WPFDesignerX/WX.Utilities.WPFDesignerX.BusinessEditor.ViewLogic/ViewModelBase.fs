namespace WX.Utilities.WPFDesignerX.BusinessEditor

open System
open System.Windows
open System.Windows.Data
open System.Windows.Input
open System.ComponentModel
open WX

type RequestCloseEventArgs (data)=
  inherit EventArgs()
  member this.Data=data

type RequestCloseEventHandler=delegate of obj*RequestCloseEventArgs->unit

[<AbstractClass>]
type ViewModelBase() =
  let _propertyChangedEvent = new DelegateEvent<PropertyChangedEventHandler>()
  let _RequestCloseEvent=new Event<RequestCloseEventHandler,RequestCloseEventArgs>()
  interface INotifyPropertyChanged with
    [<CLIEvent>]
    member this.PropertyChanged = _propertyChangedEvent.Publish
  member this.OnPropertyChanged propertyName = 
    _propertyChangedEvent.Trigger([| this; new PropertyChangedEventArgs(propertyName) |])

  member this.RequestClose:IEvent<RequestCloseEventHandler,RequestCloseEventArgs>=_RequestCloseEvent.Publish
  member private this.OnRequestClose (data:obj)=
    if this.RequestClose<>Null() then
      _RequestCloseEvent.Trigger(this,new RequestCloseEventArgs(data))   

  [<DV>]val mutable private _MainWindow:Window
  member this.MainWindow
    with get()=
      if this._MainWindow=null then
        this._MainWindow<-Application.Current.MainWindow 
      this._MainWindow
    and set v=this._MainWindow<-v

  member this.MainWindowModel
    with get()=
      match this.MainWindow.DataContext with
      | :? ViewModelBase as x ->x
      | _ ->Null()
     
  [<DV>]val mutable private _HostWindow:Window
  member this.HostWindow
    with get()=
      if this._HostWindow =null then
        this._HostWindow<-this.MainWindow 
      this._HostWindow
    and private set v=this._HostWindow<-v

  [<DV>] val mutable private _Title:string
  member this.Title 
    with get ()=this._Title
    and set v=
      if this._Title<>v then
        this._Title<-v
        this.OnPropertyChanged "Title"

  [<DV>]val mutable _CloseCommand:ICommand
  member this.CloseCommand
    with get ()=
      if this. _CloseCommand=null then
        this. _CloseCommand<-new RelayCommand(fun e->
          match this.HostWindow with
          | x when x = Application.Current.MainWindow -> x.Visibility<-Visibility.Hidden //不能是Visibility.Collapsed
          | x ->x.Close()
          this.OnRequestClose(e)
          )
      this. _CloseCommand  

  member this.ShowDialog(dialogModel:ViewModelBase)=
    match new Window(),dialogModel with
    | x,y ->
        //x.Resources<-this.HostWindow.Resources
        x.DataContext<-y
        y.HostWindow<-x
        y.MainWindow<-this.MainWindow
        BindingOperations.SetBinding(x,Window.ContentProperty,new Binding())|>ignore
        BindingOperations.SetBinding(x,Window.TitleProperty,new Binding("Title"))|>ignore
        x.SizeToContent<-SizeToContent.WidthAndHeight //Double.NaN设置'Auto'无效
        x.ResizeMode<-ResizeMode.NoResize
        x.Icon<-this.MainWindow.Icon
        x.Owner<-this.HostWindow
        x.WindowStartupLocation<-WindowStartupLocation.CenterOwner
        x.ShowDialog()|>ignore

