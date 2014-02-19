namespace WX.Utilities.Excel.ViewModel

open System
open System.Collections.ObjectModel
open System.Windows
open System.IO
open System.Text
open System.Text.RegularExpressions
open System.Xml
open System.Collections
open System.Windows.Controls
open System.Windows.Data
open System.Windows.Input
open System.ComponentModel
open Microsoft.Office.Interop.Excel
open Microsoft.Win32
open WX.Utilities.Excel.Model
open WX.Utilities.Common

type FVM_ExcelDataExport()=
  inherit ViewModelBase()
  let mutable _ExcelApp:ApplicationClass=null
  let mutable _SourceSheets:Sheets option=None
  let mutable _SourceExcelFileInfo:FileInfo option=None
  let _SourceSheetInfos=new Generic.List<SheetInfo>()
  let _IsCheckedForceLastLevel=ref false
  let GetBindingColumnLevelItems itemCount initialNum=
    [|
      for n in initialNum..(initialNum+itemCount-1) do
        yield new KeyValue<_,_>(Key=n,Value=string n+"级")
    |]

  let GetBindingHeaderRowItems itemCount initialNum=
    [|
      for n in initialNum..(initialNum+itemCount-1) do
        yield new KeyValue<_,_>(Key=n,Value="第"+string n+"行")
    |]

  (*
  http://www.cnblogs.com/shanyou/archive/2009/07/13/1522367.html
  转全角的函数(SBC case)
  全角空格为12288，半角空格为32
  其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
  *)
  let ToSBC (input:string)=
    match input.ToCharArray() with
    | x ->
        for i in 0..x.Length-1 do
          match x.[i] with
          | y when int y=32 ->x.[i]<-char 12288
          | y when int y<127 ->x.[i]<-char (int y+65248)
          | _ ->()
        String x

  (*
   转半角的函数(DBC case)
  全角空格为12288，半角空格为32
  其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
  *)
  let ToDBC (input:string)=
    match input.ToCharArray() with
    | x ->
        for i in 0..x.Length-1 do
          match x.[i] with
          | y when int y=12288 ->x.[i]<-char 32
          | y when int y>65280 && int y<65375 ->x.[i]<-char (int y-65248)
          | _ ->()
        String x

  (*返回值：单元格的值*合并单元格的行数，合并单元格的列数 *)
  let GetCellValue (sheet:_Worksheet) (rowNum:int,colNum:int)= 
    let cell = sheet.Cells.[rowNum, colNum]:?>Range
    match cell.MergeCells with
    | :? bool as x when x ->
        ((sheet.Cells.[cell.MergeArea.Row, cell.MergeArea.Column]):?>Range).Text.ToString(), (cell.MergeArea.Rows.Count,cell.MergeArea.Columns.Count)
    | _ ->cell.Text.ToString(),(1,1) 

  let GetWorksheetsConfigControl (sheets:Sheets) (headerRowNum:int)=
    let tab=new TabControl()
    _SourceSheetInfos.Clear()
    sheets
    |>Seq.cast<_Worksheet>
    |>Seq.iteri  (fun i w ->
        match new TabItem(),new SheetInfo()   with
        | z, zd ->
            _SourceSheetInfos.Add zd
            tab.Items.Add z|>ignore
            //z.Header<-w.Name
            z.DataContext<-zd
            zd.SheetNum<-i+1
            zd.SheetHeader<-w.Name
            zd.NeedExportFlag<-true
            GetBindingHeaderRowItems 12 1|>Seq.iter (fun a->zd.HeaderRowItems.Add a)
            match zd.HeaderRowItems|>Seq.tryFind (fun a->a.Key=headerRowNum) with
            | Some v->
                zd.HeaderRowSelectedItem<-v
            | _ ->()
            z.Header<-
              match new DockPanel() with
              | v ->
                  //v.DataContext<-zd
                  v.LastChildFill<-true
                  match new Controls.TextBlock() with
                  | u ->
                      v.Children.Add u|>ignore
                      u.SetBinding (TextBlock.TextProperty,new Binding("SheetHeader"))|>ignore
                  match new Controls.CheckBox() with
                  | u ->
                      v.Children.Add u|>ignore
                      DockPanel.SetDock (u,Dock.Right)
                      u.SetBinding (CheckBox.IsCheckedProperty,new Binding("NeedExportFlag"))|>ignore
                  v
            z.Content<-
              match new Grid() with
              | v ->
                  match new RowDefinition() with
                  | u ->
                      u.Height<-new GridLength(45.0)
                      v.RowDefinitions.Add u
                  match new RowDefinition() with
                  | u ->
                      u.Height<-new GridLength(1.0,GridUnitType.Star)
                      v.RowDefinitions.Add u
                  match new StackPanel() with
                  | u ->
                      v.Children.Add u|>ignore
                      Grid.SetRow(u,0)
                      u.Orientation<-Orientation.Horizontal
                      match new Controls.Label() with
                      | r ->
                          r.Content<-"标题行"
                          u.Children.Add r|>ignore
                      match new ComboBox() with
                      | r ->
                          r.Width<-80.0
                          r.SetBinding (ComboBox.ItemsSourceProperty,"HeaderRowItems")|>ignore
                          r.SetBinding (ComboBox.SelectedItemProperty,"HeaderRowSelectedItem")|>ignore
                          r.SetValue (ComboBox.DisplayMemberPathProperty,"Value") 
                          u.Children.Add r|>ignore
                  match new ListView() with
                  | u ->
                      v.Children.Add u|>ignore
                      Grid.SetRow (u,1)
                      u.SetBinding (ListView.ItemsSourceProperty,"SheetColumnInfoView")|>ignore
                      match new GridView() with
                      | r->
                          u.View<-r
                          match new GridViewColumn() with
                          | s ->
                              r.Columns.Add s
                              s.Header<-"列标题"
                              //y.Width<-150.0
                              s.DisplayMemberBinding<-new Binding("ColumnHeaderName")
                          match new GridViewColumn() with
                          | s ->
                               r.Columns.Add s
                               s.Header<-"层级"
                               s.Width<-120.0
                               match new DataTemplate() with
                               | t ->
                                   match new FrameworkElementFactory(typeof<ComboBox>) with
                                   | p ->
                                       p.SetBinding(ComboBox.ItemsSourceProperty,new Binding("ColumnLevelItems"))
                                       p.SetBinding (ComboBox.SelectedItemProperty,new Binding("ColumnLevelSelectedItem")) 
                                       p.SetValue (ComboBox.DisplayMemberPathProperty,"Value") 
                                       p.SetValue(ComboBox.WidthProperty,80.0)
                                       t.VisualTree<-p
                                   s.CellTemplate <-t
                  v
            //----------------------------------------------------
            let rec GetColumnInfos (rowNum:int) (columnNum:int)=
              seq{
                match GetCellValue w (rowNum,columnNum) with  //(w.Cells.[rowNum,columnNum]:?>Range).Text.ToString()
                | u,(_,v)  when String.IsNullOrWhiteSpace u|>not ->
                    for n in 0..v-1 do
                       match new SheetColumnInfo() with
                       | r ->
                           r.ColumnHeaderName<- if v>1 then u+(string (n+1)) else u
                           r.ColumnNum<-columnNum+n
                           yield r
                    yield! GetColumnInfos rowNum (columnNum+v) 
                | _ ->()
              }
            let prepareSheetColumnInfo headerRowNum=
              match GetColumnInfos headerRowNum 1|>Seq.toArray with
              | u ->
                  u
                  |>Array.mapi (fun i a->
                     if i=0 then GetBindingColumnLevelItems 1 1 |>Array.iter(fun b->a.ColumnLevelItems.Add b)
                     else GetBindingColumnLevelItems 2 1 |>Array.iter(fun b->a.ColumnLevelItems.Add b)
                     if a.ColumnLevelItems.Count>0 then a.ColumnLevelSelectedItem<-a.ColumnLevelItems.[0]
                     let recFlag=ref false
                     (a:>INotifyPropertyChanged).PropertyChanged.AddHandler(fun o e->
                       match o with
                       | :? SheetColumnInfo as v ->
                           match e.PropertyName, !recFlag with
                           | EqualsIn ["ColumnLevel"] _, false->
                               match u|>Array.tryFindIndex (fun b->b=v) with
                               | Some r ->
                                   if r+1<u.Length  then 
                                     recFlag:=true
                                     for n in r+1..u.Length-1 do
                                       u.[n].ColumnLevelItems.Clear()
                                       GetBindingColumnLevelItems 2 v.ColumnLevel|>Array.iter(fun b->u.[n].ColumnLevelItems.Add b)
                                       if u.[n].ColumnLevelItems.Count>0 then u.[n].ColumnLevelSelectedItem<-u.[n].ColumnLevelItems.[0]
                                     recFlag:=false
                                   else ()
                               | _ ->()
                           | _ ->()
                       | _ ->()
                       )
                     a
                     )
            (zd:>INotifyPropertyChanged).PropertyChanged.AddHandler(fun o e->
              match o with
              | :? SheetInfo as v ->
                  match e.PropertyName with
                  | EqualsIn ["HeaderRowSelectedItem"] _->
                      match prepareSheetColumnInfo v.HeaderRowSelectedItem.Key with
                      | u ->
                          v.SheetColumnInfoView.Clear()
                          u|>Array.iter(fun a->v.SheetColumnInfoView.Add a)
                  | _ ->()
              | _ ->()
              )
            match prepareSheetColumnInfo headerRowNum with
            | u ->u|>Seq.iter(fun a->zd.SheetColumnInfoView.Add a)
        )
    tab

  [<DV>]val mutable _D_FilePath:string
  member this.D_FilePath
    with get()= this._D_FilePath
    and set v=
      if v<>this._D_FilePath then
        this._D_FilePath<-v
        this.OnPropertyChanged "D_FilePath"

  [<DV>]val mutable _IsCheckedForceLastLevel:bool
  member this.IsCheckedForceLastLevel
    with get()= this._IsCheckedForceLastLevel
    and set v=
      if v<>this._IsCheckedForceLastLevel then
        this._IsCheckedForceLastLevel<-v
        this.OnPropertyChanged "IsCheckedForceLastLevel"
        _IsCheckedForceLastLevel:=v

  [<DV>]val mutable private _D_HeaderRowItems:ObservableCollection<KeyValue<int,string>>
  member  this.D_HeaderRowItems
    with get ()=
      if this._D_HeaderRowItems=null then
        this._D_HeaderRowItems<-new ObservableCollection<KeyValue<int,string>>(GetBindingHeaderRowItems 12 1)
      this._D_HeaderRowItems

  [<DV>]val mutable private _D_HeaderRowSelectedItem:KeyValue<int,string>
  member this.D_HeaderRowSelectedItem
    with get():KeyValue<int,string>=
      if this._D_HeaderRowSelectedItem=Null() then this.D_HeaderRowSelectedItem<-this.D_HeaderRowItems.[0]
      this._D_HeaderRowSelectedItem
    and set v=
      if this._D_HeaderRowSelectedItem<>v then
        this._D_HeaderRowSelectedItem<-v
        this.OnPropertyChanged "D_HeaderRowSelectedItem"  
        match _SourceSheets, _ExcelApp, _SourceExcelFileInfo with
        | Some x, y, Some z when y.Visible && y.Workbooks|>Seq.cast<Workbook>|>Seq.exists(fun a->a.Name=z.Name) -> 
            this.D_ExcelDataInfo<-GetWorksheetsConfigControl x v.Key
        | _ ->()
        
  [<DV>]val mutable _CMD_Browse:ICommand 
  member this.CMD_Browse
    with get ()=
      if this._CMD_Browse=null then
        this._CMD_Browse<-new RelayCommand(fun _ ->
          match new OpenFileDialog() with
          | x ->
              x.Multiselect<-false
              x.FileName<-""
              x.DefaultExt<-".xlsx"
              x.Filter<-"Excle documents (*.xlsx, *.xls)|*.xlsx;*.xls"
              x.CheckFileExists<-true
              match x.ShowDialog() with
              | y when y.HasValue && y.Value ->
                  _SourceExcelFileInfo<-Some<| FileInfo x.FileName
                  this.D_FilePath<-x.FileName
                  _ExcelApp<-
                      match  _ExcelApp with
                      | null ->new ApplicationClass(Visible = true)
                      | z ->
                          _ExcelApp.Visible<-true
                          z
                  let workbooks = _ExcelApp.Workbooks
                  let workbook=workbooks.Open(x.FileName)
                  _SourceSheets<- Some workbook.Worksheets
                  this.D_ExcelDataInfo<-GetWorksheetsConfigControl _SourceSheets.Value this.D_HeaderRowSelectedItem.Key
              | _ ->()
          )  
      this._CMD_Browse

  let GetWorksheetData (sheet:_Worksheet) (sheetInfo:SheetInfo) =
    seq{
      let rec GetData (sheet:_Worksheet) (levelMax:int,level:int,sheetColumnInfos:SheetColumnInfo[]) (preLevelData:((int*int*Guid*Guid)*(string*(int[]*int))[])[]) (rowNum:int)=
        seq{
          match 
            match level with
            | 1 ->
                match GetCellValue sheet (rowNum,1) with //测试每一行的第一个单元格是否有值，性能损失较大！
                | xs, _ when String.IsNullOrWhiteSpace xs|>not ->true
                | _ ->false
            | _ ->
                match preLevelData.[preLevelData.Length-1] with
                | (_,u,_,_),_ ->rowNum<=u
            with
          | true ->
              match 
                match level with
                | 1 ->Guid.Empty,[||]
                | _ ->
                    match preLevelData|>Seq.tryFind (fun (_,a)->a|>Seq.exists(fun (_,(b,c))->c=sheetColumnInfos.[0].ColumnNum-1 && b|>Seq.exists (fun d->d=rowNum))) with
                    | Some ((_,_,u,_),v) ->u,v
                    | _ ->Guid.Empty,[||]
                    (*
                    match preLevelData|>Seq.collect(fun (_,a)->a|>Seq.filter(fun (_,(b,_))->b|>Seq.exists(fun c->c=rowNum)))|>Seq.distinct with //不一定是同一行的，所以不能确定行ID
                    | u ->...
                    *)
                with
              | ua,ub ->
                  match 
                    (
                    seq{
                      yield! ub
                      for n in sheetColumnInfos do
                        match GetCellValue sheet (rowNum,n.ColumnNum) with
                        | v, (r,_)  ->
                            yield v, ([|for m in rowNum..rowNum+r-1->m|],n.ColumnNum) //Range行的范围不能提到行级别，否则两个层级的记录组合时，Range行的范围信息将丢失
                    }
                    |>Seq.sortBy (fun (_,(_,a))->a) //按列排序，需要？
                    |>Seq.toArray)
                    with
                  |  u ->
                      match u|>Seq.find (fun (_,(_,a))->a=sheetColumnInfos.[0].ColumnNum) with //grouby后的元素必定有值(sheetColumnInfos)
                      | _,(r,_) ->
                          yield (level,rowNum+r.Length-1,Guid.NewGuid(),ua),u
                          yield! 
                            match levelMax=level && !_IsCheckedForceLastLevel with
                            | false  ->rowNum+r.Length
                            | _ ->rowNum+1
                            |>GetData sheet (levelMax,level,sheetColumnInfos) preLevelData 
          | _ ->()
        }
      
      let rec GetDataAtOneLevel(sheet:_Worksheet) (sheetColumnInfos:ObservableCollection<SheetColumnInfo>) (rowNum:int)=
        seq{
          match GetCellValue sheet (rowNum,1) with //测试每一行的第一个单元格是否有值，性能损失较大！
          | xs, _ when String.IsNullOrWhiteSpace xs|>not ->
              match
                ( 
                seq{
                  for n in sheetColumnInfos do
                    match GetCellValue sheet (rowNum,n.ColumnNum) with
                    | v, _ -> 
                        yield v,([|rowNum|],n.ColumnNum)
                }
                |>Seq.toArray)
                with
              | u ->
                  yield (1,0,Guid.NewGuid(),Guid.Empty),u
                  yield! GetDataAtOneLevel sheet sheetColumnInfos (rowNum+1)
          | _ ->()
        }

      (*(int*int*Guid*Guid)*(string*(int[]*int))[]=(level*实际Range所在的最大的行*IDX*FIDX)*(Range的值*(Range行的范围*列)) *)
      match sheetInfo.SheetColumnInfoView|>Seq.forall (fun a->a.ColumnLevel=1) with
      | true-> //所有列按1个层级导出时，需做特殊处理
          yield GetDataAtOneLevel sheet sheetInfo.SheetColumnInfoView (sheetInfo.HeaderRowSelectedItem.Key+1)|>Seq.toArray
      | _ ->
          let preLevelData:((int*int*Guid*Guid)*(string*(int[]*int))[])[] ref=Null()|>ref
          match sheetInfo.SheetColumnInfoView|>Seq.groupBy (fun a->a.ColumnLevel)|>Seq.toArray with
          | v ->
              for (ma,mb) in v do
                match GetData sheet (v.Length,ma,mb|>Seq.toArray) !preLevelData (sheetInfo.HeaderRowSelectedItem.Key+1)|>Seq.toArray with
                | u ->
                   preLevelData:=u
                   yield u 
    }
    |>Seq.toArray

  [<DV>]val mutable _CMD_ExportToExcel:ICommand 
  member this.CMD_ExportToExcel
    with get ()=
      if this._CMD_ExportToExcel=null then
        this._CMD_ExportToExcel<-new RelayCommand(fun _ ->
          match _SourceExcelFileInfo, new SaveFileDialog() with
          | Some x, y ->
              y.FileName<-x.Name.Replace(x.Extension,String.Empty)+"(Processed)"
              y.DefaultExt<-x.Extension
              y.Filter<-"Excle documents (*.xlsx, *.xls)|*.xlsx;*.xls"
              match y.ShowDialog() with
              | z  when z.HasValue && z.Value  ->
                  let workbook = _ExcelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet) 
                  let sheets=workbook.Worksheets
                  _SourceSheetInfos
                  |>Seq.filter (fun a->a.NeedExportFlag)
                  |>Seq.iteri (fun i a ->
                      match _SourceSheets with
                      | Some y ->
                          match y.[a.SheetNum] with
                          | :? _Worksheet as w ->
                             if sheets.Count<i+1 then sheets.Add(After=sheets.[sheets.Count])|>ignore
                             match sheets.[i+1]  with
                             | :? _Worksheet as u->
                                 match GetWorksheetData w a with
                                 | v  ->
                                     u.Name<-w.Name
                                     match v|>Array.collect (fun a->a),a.HeaderRowSelectedItem.Key with
                                     | vs,vt when vs.Length>0 ->
                                         u.Cells.[vt,1]<-"层级"
                                         u.Cells.[vt,2]<-"IDX"
                                         u.Cells.[vt,3]<-"父IDX"
                                         for m in a.SheetColumnInfoView do
                                           u.Cells.[vt,m.ColumnNum+3]<-m.ColumnHeaderName
                                         vs
                                         |>Seq.iteri (fun i ((a1,_,a2,a3),a4)->
                                           u.Cells.[i+vt+1,1]<-a1.ToString()
                                           u.Cells.[i+vt+1,2]<-a2.ToString()
                                           u.Cells.[i+vt+1,3]<-a3.ToString()
                                           a4
                                           |>Seq.iteri (fun j (b,_)->
                                               u.Cells.[i+vt+1,j+4]<-b
                                               )
                                           )
                                     | _ ->()
                             | _ ->()
                          | _ ->()
                      | _ ->())
                  _ExcelApp.DisplayAlerts<-false //Overwrite directly
                  workbook.SaveAs (Filename=y.FileName,AccessMode=XlSaveAsAccessMode.xlShared)
                  _ExcelApp.DisplayAlerts<-true
                  workbook.Close(SaveChanges=true)
                  MessageBox.Show("导出Excel文件成功！","导出提示")|>ignore
              | _ ->()
          | _ ->()
          )  
      this._CMD_ExportToExcel

  [<DV>]val mutable _CMD_ExportToXml:ICommand 
  member this.CMD_ExportToXml
    with get ()=
      if this._CMD_ExportToXml=null then
        this._CMD_ExportToXml<-new RelayCommand(fun _ ->
          match new Forms.FolderBrowserDialog() with
          | x ->
              x.SelectedPath<-
                match _SourceExcelFileInfo with
                | Some y ->y.DirectoryName
                | _ ->String.Empty
              match x.ShowDialog() with
              | Forms.DialogResult.OK ->
                  _SourceSheetInfos
                  |>Seq.filter (fun a->a.NeedExportFlag)
                  |>Seq.iteri (fun _ a ->
                      match _SourceSheets with
                      | Some y ->
                          match y.[a.SheetNum] with
                          |  :? _Worksheet as w ->
                              match GetWorksheetData w a with
                              | v  ->
                                  match Path.Combine (x.SelectedPath,w.Name+".xml") with
                                  | uf ->
                                      use vx= new XmlTextWriter(uf , Encoding.UTF8) //or use vx=XmlWriter.Create(this.D_FilePath)
                                      vx.Formatting<-Formatting.Indented
                                      vx.WriteStartDocument()
                                      vx.WriteStartElement "View"
                                      match v|>Array.collect (fun a->a) with
                                      | vs when vs.Length>0 ->
                                          vs
                                          |>Seq.iteri (fun i ((a1,_,a2,a3),a4)->
                                              vx.WriteStartElement "Record"
                                              vx.WriteAttributeString ("层级",string a1)
                                              vx.WriteAttributeString ("IDX",string a2)
                                              vx.WriteAttributeString ("父IDX",string a3)
                                              for m in 0..a.SheetColumnInfoView.Count-1 do
                                                match  Regex.Replace(a.SheetColumnInfoView.[m].ColumnHeaderName, @"\W",String.Empty) , a4.Length>m with //替换所有非字符。ToDBC a.SheetColumnInfoView.[m].ColumnHeaderName(全角转半角)
                                                | va, true ->
                                                    match a4.[m] with
                                                    | vb,_ ->vx.WriteAttributeString (va,vb)   
                                                | va, _ ->vx.WriteAttributeString (va,String.Empty)
                                              vx.WriteEndElement()
                                              )
                                      | _ ->()
                                      vx.WriteEndElement()
                                      vx.WriteEndDocument()
                          | _ ->()
                      | _ ->())
                  MessageBox.Show("导出xml文件成功！","导出提示")|>ignore
              | _->()
          )
      this._CMD_ExportToXml

  [<DV>]val mutable _CMD_Export:ICommand 
  member this.CMD_Export
    with get ()=
      if this._CMD_Export=null then
        this._CMD_Export<-new RelayCommand(
          (fun _ ->
            ()),
          (fun _ ->
             match _ExcelApp, _SourceExcelFileInfo,_SourceSheetInfos with
             | x ,Some y, z ->
                 x.Visible &&
                 x.Workbooks|>Seq.cast<Workbook>|>Seq.exists (fun a->a.Name=y.Name) &&
                 z|>Seq.exists (fun a->a.NeedExportFlag)
             | _ ->false
          ))
      this._CMD_Export

  [<DV>] val mutable _D_ExcelDataInfo:TabControl
  member this.D_ExcelDataInfo
    with get ()=this._D_ExcelDataInfo
    and set v=
      if v<>this._D_ExcelDataInfo then
        this._D_ExcelDataInfo<-v
        this.OnPropertyChanged "D_ExcelDataInfo" 

  do 
      Application.Current.Exit.Add (fun a->
        match _ExcelApp with
        | x when x<>null ->
            _ExcelApp.Workbooks.Close()
            _ExcelApp.Quit()
        | _ ->()
        )


(*
Microsoft.Office.Interop.Excel的用法 
http://www.cnblogs.com/yuteng/articles/1753767.html
*)

//-----------------------------------------------------------------

(* 使用模版，正确，但DataContext没有合适的设计位置
match new TabItem()  with
| z ->
    match new DataTemplate()  with
    | w ->
        match new FrameworkElementFactory(typeof<DockPanel>) with
        | s ->
            match new FrameworkElementFactory(typeof<TextBlock>) with
            | sx ->
                sx.SetBinding(TextBlock.TextProperty,new Binding("ColumnHeaderName"))
                s.AppendChild sx
            match new FrameworkElementFactory(typeof<Controls.CheckBox>) with
            | sx ->
                //DockPanel.SetDock(elementInstance,Dock.Right)
                sx.SetValue(DockPanel.DockProperty,Dock.Right)
                sx.SetBinding(CheckBox.IsCheckedProperty,new Binding("NeedExportFlag"))
                s.AppendChild sx
            w.VisualTree<-s
        z.HeaderTemplate<-w*)

//-----------------------------------------------------------------

(*
let GetCellValue (sheet:_Worksheet) (row:int,col:int)= //应该在这里获取行ID
  let cell = sheet.Cells.[row, col]:?>Range
  match cell.MergeCells with
  | :? bool as x when x ->
      ((sheet.Cells.[cell.MergeArea.Row, cell.MergeArea.Column]):?>Range).Text.ToString()
  | _ ->cell.Text.ToString()
  *)

(*
let rec GetData (sheet:_Worksheet) (rowNum:int)=
  seq{
    match GetCellValue sheet (rowNum,1) with //测试每一行的第一个单元格是否有值，性能损失较大！
    | xs when String.IsNullOrWhiteSpace xs|>not ->
        
        yield 
          seq{
            for (ma,mb) in z do
              yield ma, "IDX?","FIDX?",  //层级,层次ID,父层次ID
                seq{
                  for k in mb do
                    yield k.ColumnNum, GetCellValue sheet (rowNum,k.ColumnNum)
                }
                |>Seq.toArray
              parentID:=Guid.NewGuid()
          }
          |>Seq.toArray
        yield! GetData sheet (rowNum+1)
    | _ ->()
  }
GetData w 2|>ignore //从第二行开始，可配置
*)
