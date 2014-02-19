namespace WX.Utilities.Downloader.Model
open System
open System.IO
open System.Windows.Input
open System.Collections
open System.Collections.ObjectModel
open WX.Utilities.Common
open WX.Utilities.Downloader.ViewModel

type DownloadPartialInfo()=
  inherit ModelBase()

  [<DV>]val mutable private _PartialLength:int64
  member this.PartialLength
    with get ()=this._PartialLength
    and set v=
      if this._PartialLength<>v then
        this._PartialLength<-v
        this.OnPropertyChanged "PartialLength"

  [<DV>]val mutable private _PartialEndPosition:int64
  member this.PartialEndPosition
    with get ()=this._PartialEndPosition
    and set v=
      if this._PartialEndPosition<>v then
        this._PartialEndPosition<-v
        this.OnPropertyChanged "PartialEndPosition"

  [<DV>]val mutable private _DownloadedLength:int64
  member this.DownloadedLength
    with get ()=this._DownloadedLength
    and set v=
      if this._DownloadedLength<>v then
        this._DownloadedLength<-v
        this.OnPropertyChanged "DownloadedLength"

  [<DV>]val mutable private _DownloadPosition:int64
  member this.DownloadPosition
    with get ()=this._DownloadPosition
    and set v=
      if this._DownloadPosition<>v then
        this._DownloadPosition<-v
        this.OnPropertyChanged "DownloadPosition"

  [<DV>]val mutable private _ID:int
  member this.ID
    with get ()=this._ID
    and set v=this._ID<-v



