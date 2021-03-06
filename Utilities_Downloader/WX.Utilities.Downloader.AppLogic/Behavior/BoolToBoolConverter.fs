﻿namespace WX.Utilities.Downloader.Behavior

open System
open System.Windows
open System.Windows.Data
open System.Windows.Media
open ConverterBase

type BoolToBoolConverter() =
    inherit ConverterBase()
    let convertFunc = fun (v:obj) _ _ _ ->         
        match v with
        | :? bool as x -> x|>not
        | _ -> false
        :> obj
    override this.Convert = convertFunc 

