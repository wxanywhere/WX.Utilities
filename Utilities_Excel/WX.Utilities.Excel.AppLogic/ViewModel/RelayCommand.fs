namespace WX.Utilities.Excel.ViewModel

open System
open System.Diagnostics
open System.Windows
open System.Windows.Input
open System.ComponentModel

type RelayCommand (action:(obj -> unit),canExecute:(obj -> bool)) =
  new execute =RelayCommand(execute,(fun _ ->true))
  interface ICommand with
    [<CLIEvent>]
    member this.CanExecuteChanged= CommandManager.RequerySuggested
    [<DebuggerStepThrough>]
    member x.CanExecute arg = canExecute(arg)
    member x.Execute arg = action(arg)

(*
    let event = new DelegateEvent<EventHandler>()
    interface ICommand with
        [<CLIEvent>]
        member x.CanExecuteChanged = event.Publish
        member x.CanExecute arg = canExecute(arg)
        member x.Execute arg = action(arg)
*)