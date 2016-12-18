
#r "PresentationCore"
#r "PresentationFramework"
#r "System.Xaml"
#r "System.Xml.Linq"
open System.IO
open System.Windows.Media
open System.Xml.Linq

#I @"D:\Workspace\WX\Dev\WPFDesignerX\ReferenceDll"
#r "DocumentFormat.OpenXml"
#r "WindowsBase"
open DocumentFormat.OpenXml
open DocumentFormat.OpenXml.Wordprocessing
open DocumentFormat.OpenXml.Packaging
//open DocumentFormat.OpenXml.Office2010.Word
//open A=DocumentFormat.OpenXml.Drawing //直接使用该命名空间时文档不能打开，估计成员有同名情况
//open DW=DocumentFormat.OpenXml.Drawing.Wordprocessing
//open PIC=DocumentFormat.OpenXml.Drawing.Pictures

#I @"D:\Workspace\WX\Dev\WPFDesignerX\WX.Utilities.WPFDesignerX.Assemblies\Debug"
//#r "WX.Utilities.WPFDesignerX.Office.Word"
#r "WX.Utilities.WPFDesignerX.Office.WordX"
open WX.Utilities.WPFDesignerX.Office.Word

#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper"
open WX.Data.Helper


//--------------------------------------------------------------
let templatePath= @"D:\Workspace\WX\Dev\WPFDesignerX\WX.Utilities.WPFDesignerX.Office.Word\Template.docx"
let byteArray = File.ReadAllBytes templatePath 
let mem = new MemoryStream() 
mem.Write(byteArray, 0, (int)byteArray.Length) 
let doc=WordprocessingDocument.Open(mem,true)
let imagePath= @"D:\Temp\Testing\x.png"

doc.Close()
let targetPath= @"D:\Workspace\WX\Dev\WPFDesignerX\WX.Utilities.WPFDesignerX.Office.Word\Test2.docx"
let fs = new FileStream(targetPath, FileMode.Create) 
mem.WriteTo(fs) 
mem.Close()
fs.Close()

let imageStream=new FileStream(@"D:\Temp\Testing\x.png", FileMode.Open)
WordHelper.InsertImag(doc,imageStream,ImagePartType.Png)
imageStream.Close()

WordHelper.AddHeadingTwoParagraph "系统支撑"
|>doc.MainDocumentPart.Document.Body.AppendChild

WordHelper.AddBlankParagraph 3
|>Seq.iter (fun a->doc.MainDocumentPart.Document.Body.AppendChild a|>ignore)

[|
  for n in 1..3 ->
    match new WordTableEntity() with
    | x ->
        x.SerialNumber<-n
        x.UIElementName<-"Button"
        x.RequirementDescription<-"需求描述"+n.ToString()
        x.ServicesDescription<-"服务描述"+n.ToString()
        x.BehaviorDescription<-"行为描述"+n.ToString()
        x
|]
|>WordHelper.AddTable 
|>doc.MainDocumentPart.Document.Body.AppendChild

doc
|>WordHelper.UpdateTOC

let settingsPart = 
    doc.MainDocumentPart.GetPartsOfType<DocumentSettingsPart>()|>Seq.head
// Create object to update fields on open
let updateFields = new UpdateFieldsOnOpen();
updateFields.Val <- new DocumentFormat.OpenXml.OnOffValue(true)
// Insert object into settings part.
settingsPart.Settings.PrependChild<UpdateFieldsOnOpen>(updateFields)
settingsPart.Settings.Save();

let  ns = XNamespace.Get @"http://schemas.openxmlformats.org/wordprocessingml/2006/main"
let xmlXdocument = XDocument.Parse(doc.MainDocumentPart.Document.InnerXml)
let xmlelement = xmlXdocument.Descendants(ns + "p"]);
//let TOCRefNode = xmlelement|>Seq.find (fun a->a.Attribute(ns+"rsidR").Value="00C8480A" && a.Attribute(ns+"rsidRDefault").Value="007E2267")
xmlelement|>Seq.nth 3

let block = 
  doc.MainDocumentPart.Document.Descendants<DocPartGallery>()
  |>Seq.filter (fun a->a.Val.HasValue && a.Val.Value="TOC")
  |>Seq.head



let imageSourceConverter = new ImageSourceConverter()
let img = imageSourceConverter.ConvertFrom(imageStream):?>ImageSource
img.Width
img.Height

let img=new System.Windows.Media.Imaging.BitmapImage()
img.BeginInit()
img.StreamSource<-imageStream
img.EndInit()

let widthPx = img.PixelWidth
let heightPx = img.PixelHeight
let emusPerInch = 914400.0
let emusPerCm = 360000.0
let maxWidthCm= 21.0-3.17*2.0 //A4 21*29.7 cm
let ratio= decimal heightPx / decimal widthPx
let maxWidthEmus = maxWidthCm * emusPerCm|>int64
let maxHeight= decimal maxWidthEmus * ratio|>int64

(*
let widthPx = img.PixelWidth
let heightPx = img.PixelHeight
let horzRezDpi = img.DpiX
let vertRezDpi = img.DpiY
let emusPerInch = 914400.0
let emusPerCm = 360000.0
let maxWidthCm= 21.0-3.17*2.0 //A4 21*29.7 cm
let mutable widthEmus = float widthPx / horzRezDpi * float emusPerInch |>int64
let mutable heightEmus = float heightPx / vertRezDpi * float emusPerInch|>int64
let maxWidthEmus = maxWidthCm * emusPerCm|>int64
if widthEmus > maxWidthEmus then
    let ratio = decimal heightEmus / decimal widthEmus
    widthEmus <- maxWidthEmus
    heightEmus <- decimal widthEmus * ratio|>int64
*)



let imagex=new System.Drawing.Bitmap(imageStream)
imagex.Width
imagex.Height


let bms=
  //doc.MainDocumentPart.RootElement.Descendants<BookmarkStart>() //Right
  doc.MainDocumentPart.Document.Body.Descendants<BookmarkStart>()
  |>Seq.cast<BookmarkStart>
  |>Seq.toArray 


let bm= bms.[0].Name
let x= bms.[0].NextSibling<Run>()
x.GetFirstChild<Text>().Text<-"界面设计"

let x6= bms.[6].NextSibling<Run>()
x6.GetFirstChild<Drawing>()




DocumentFormat.OpenXml.Wordprocessing.i


doc.Close()



let main= doc.AddMainDocumentPart()
main.Document<-new Document()
let body = main.Document.AppendChild(new Body())

//let paragraph = body.AppendChild(new Paragraph())
let paragraph=doc.MainDocumentPart.Document.Body.AppendChild (new Paragraph())
let run = paragraph.AppendChild(new Run())
run.AppendChild(new Text("在 body 本文內容產生 text 文字"))




main.Document.Save()



//--------------------------------------------------------------

doc.MainDocumentPart.ImageParts
|>Seq.cast<ImagePart>
|>Seq.length

let image=doc.MainDocumentPart.AddImagePart(ImagePartType.Png)
let stream = new FileStream(@"D:\Temp\Testing\x.png", FileMode.Open)
image.FeedData stream
image
stream.Close()



let AddImageToBody ( wordDoc:WordprocessingDocument, relationshipId:string)=
    // Define the reference of the image.
    let element =
         new DocumentFormat.OpenXml.Wordprocessing.Drawing(
             new DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline(
                 new DocumentFormat.OpenXml.Drawing.Wordprocessing.Extent(Cx = Int64Value 990000L, Cy = Int64Value 792000L),
                 new DocumentFormat.OpenXml.Drawing.Wordprocessing.EffectExtent(LeftEdge = Int64Value 0L,TopEdge = Int64Value 0L,
                     RightEdge = Int64Value 0L,BottomEdge = Int64Value 0L),
                 new DocumentFormat.OpenXml.Drawing.Wordprocessing.DocProperties(Id = UInt32Value 1u,Name = StringValue "Picture 1"),
                 new DocumentFormat.OpenXml.Drawing.Wordprocessing.NonVisualGraphicFrameDrawingProperties(
                     new DocumentFormat.OpenXml.Drawing.GraphicFrameLocks(NoChangeAspect = BooleanValue true)),
                 new DocumentFormat.OpenXml.Drawing.Graphic(
                     new DocumentFormat.OpenXml.Drawing.GraphicData(
                         new DocumentFormat.OpenXml.Drawing.Pictures.Picture(
                             new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties(
                                 new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties(Id = UInt32Value 0u,Name = StringValue "New Bitmap Image.jpg"),
                                 new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties()
                                 ),
                             new DocumentFormat.OpenXml.Drawing.Pictures.BlipFill(
                                 new DocumentFormat.OpenXml.Drawing.Blip(
                                     new DocumentFormat.OpenXml.Drawing.BlipExtensionList(
                                         new DocumentFormat.OpenXml.Drawing.BlipExtension(Uri =StringValue "{28A0092B-C50C-407E-A947-70E740481C1C}")
                                         ),
                                     Embed = StringValue relationshipId,CompressionState =EnumValue<_> DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print
                                 ),
                                 new DocumentFormat.OpenXml.Drawing.Stretch(new DocumentFormat.OpenXml.Drawing.FillRectangle())),
                             new DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties(
                                 new DocumentFormat.OpenXml.Drawing.Transform2D(
                                     new DocumentFormat.OpenXml.Drawing.Offset(X = Int64Value 0L, Y = Int64Value 0L),
                                     new DocumentFormat.OpenXml.Drawing.Extents(Cx = Int64Value 990000L, Cy = Int64Value 792000L )),
                                 new DocumentFormat.OpenXml.Drawing.PresetGeometry(
                                     new DocumentFormat.OpenXml.Drawing.AdjustValueList(),
                                     Preset = EnumValue<_> DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle))
                             ),
                         Uri = StringValue "http://schemas.openxmlformats.org/drawingml/2006/picture" 
                         )
                     ),
                 DistanceFromTop = UInt32Value 0u,DistanceFromBottom = UInt32Value 0u,
                 DistanceFromLeft = UInt32Value 0u,DistanceFromRight = UInt32Value 0u,EditId = HexBinaryValue "50D07946"
                 )
             )
    // Append the reference to body, the element should be in a Run.
    wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)))|>ignore



module A=
  type GraphicFrameLocks=DocumentFormat.OpenXml.Drawing.GraphicFrameLocks
  type Graphic=DocumentFormat.OpenXml.Drawing.Graphic
  type GraphicData=DocumentFormat.OpenXml.Drawing.GraphicData
  type Blip=DocumentFormat.OpenXml.Drawing.Blip
  type BlipExtensionList=DocumentFormat.OpenXml.Drawing.BlipExtensionList
  type BlipExtension=DocumentFormat.OpenXml.Drawing.BlipExtension
  type BlipCompressionValues=DocumentFormat.OpenXml.Drawing.BlipCompressionValues
  type Stretch=DocumentFormat.OpenXml.Drawing.Stretch
  type FillRectangle=DocumentFormat.OpenXml.Drawing.FillRectangle
  type Transform2D=DocumentFormat.OpenXml.Drawing.Transform2D
  type Offset=DocumentFormat.OpenXml.Drawing.Offset
  type Extents=DocumentFormat.OpenXml.Drawing.Extents
  type PresetGeometry=DocumentFormat.OpenXml.Drawing.PresetGeometry
  type AdjustValueList=DocumentFormat.OpenXml.Drawing.AdjustValueList
  type ShapeTypeValues=DocumentFormat.OpenXml.Drawing.ShapeTypeValues

module DW=
  type Inline=DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline
  type Extent=DocumentFormat.OpenXml.Drawing.Wordprocessing.Extent
  type EffectExtent=DocumentFormat.OpenXml.Drawing.Wordprocessing.EffectExtent
  type DocProperties=DocumentFormat.OpenXml.Drawing.Wordprocessing.DocProperties
  type NonVisualGraphicFrameDrawingProperties=DocumentFormat.OpenXml.Drawing.Wordprocessing.NonVisualGraphicFrameDrawingProperties

module PIC=
  type Picture=DocumentFormat.OpenXml.Drawing.Pictures.Picture
  type NonVisualPictureProperties=DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties
  type NonVisualDrawingProperties=DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties
  type NonVisualPictureDrawingProperties=DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties
  type BlipFill=DocumentFormat.OpenXml.Drawing.Pictures.BlipFill
  type ShapeProperties=DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties


let AddImageToBody(wordDoc:WordprocessingDocument , relationshipId:string )=
  // Define the reference of the image.
  let element =
       new Drawing(
           new DW.Inline(
               new DW.Extent(Cx = Int64Value 990000L, Cy = Int64Value 792000L),
               new DW.EffectExtent(LeftEdge = Int64Value 0L,TopEdge = Int64Value 0L,RightEdge = Int64Value 0L,BottomEdge = Int64Value 0L),
               new DW.DocProperties(Id = UInt32Value 1u,Name = StringValue "Picture 1"),
               new DW.NonVisualGraphicFrameDrawingProperties(
                   new A.GraphicFrameLocks(NoChangeAspect = BooleanValue true)),
               new A.Graphic(
                   new A.GraphicData(
                       new PIC.Picture(
                           new PIC.NonVisualPictureProperties(
                               new PIC.NonVisualDrawingProperties(Id = UInt32Value 0u,Name = StringValue "New Bitmap Image.jpg"),
                               new PIC.NonVisualPictureDrawingProperties()),
                           new PIC.BlipFill(
                               new A.Blip(
                                   new A.BlipExtensionList(
                                       new A.BlipExtension(Uri = StringValue "{28A0092B-C50C-407E-A947-70E740481C1C}")
                                       ),
                                   Embed = StringValue relationshipId,
                                   CompressionState =EnumValue<_> A.BlipCompressionValues.Print
                                   ),
                               new A.Stretch(new A.FillRectangle())),
                           new PIC.ShapeProperties(
                               new A.Transform2D(
                                   new A.Offset(X = Int64Value 0L, Y = Int64Value 0L),
                                   new A.Extents(Cx = Int64Value 990000L, Cy = Int64Value 792000L)),
                               new A.PresetGeometry(
                                   new A.AdjustValueList(),
                                   Preset = EnumValue<_> A.ShapeTypeValues.Rectangle))),
                       Uri = StringValue "http://schemas.openxmlformats.org/drawingml/2006/picture" )
                   ),
               DistanceFromTop = UInt32Value 0u,DistanceFromBottom =UInt32Value 0u,
               DistanceFromLeft = UInt32Value 0u,DistanceFromRight = UInt32Value 0u,
               EditId = HexBinaryValue "50D07946"
               )
           )
  // Append the reference to body, the element should be in a Run.
  wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)))|>ignore

let AddImageToBodyX (wordDoc:WordprocessingDocument , relationshipId:string )=
  // Define the reference of the image.
  let element =
       new Drawing(
           match 
               new DW.Inline
               (
                   match new DW.Extent() with
                   | x ->
                       x.Cx <- Int64Value 990000L
                       x.Cy <- Int64Value 792000L
                       x
                   ,
                   match new DW.EffectExtent() with
                   | x ->
                       x.LeftEdge <- Int64Value 0L
                       x.TopEdge <- Int64Value 0L
                       x.RightEdge <- Int64Value 0L
                       x.BottomEdge <- Int64Value 0L
                       x
                   ,
                   match new DW.DocProperties() with
                   | x ->
                       x.Id <- UInt32Value 1u
                       x.Name <- StringValue "Picture 1"
                       x
                   ,
                   new DW.NonVisualGraphicFrameDrawingProperties(
                       match new A.GraphicFrameLocks() with
                       | x ->
                           x.NoChangeAspect <- BooleanValue true
                           x
                       ),
                   new A.Graphic(
                       match 
                           new A.GraphicData
                           (
                               new PIC.Picture(
                                   new PIC.NonVisualPictureProperties(
                                       match new PIC.NonVisualDrawingProperties() with
                                       | x ->
                                           x.Id <- UInt32Value 0u
                                           x.Name <- StringValue "New Bitmap Image.jpg"
                                           x
                                       ,
                                       new PIC.NonVisualPictureDrawingProperties()
                                       ),
                                   new PIC.BlipFill(
                                       match 
                                           new A.Blip
                                           (
                                               new A.BlipExtensionList(
                                                   match new A.BlipExtension() with
                                                   | x ->
                                                       x.Uri <- StringValue "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                       x
                                                   )
                                               )
                                           with
                                       | x ->
                                           x.Embed <- StringValue relationshipId
                                           x.CompressionState <- EnumValue<_> A.BlipCompressionValues.Print
                                           x
                                       ,
                                       new A.Stretch(new A.FillRectangle())
                                       ),
                                   new PIC.ShapeProperties(
                                       new A.Transform2D(
                                           match new A.Offset() with
                                           | x ->
                                               x.X <- Int64Value 0L
                                               x.Y <- Int64Value 0L
                                               x
                                           ,
                                           match new A.Extents() with
                                           | x ->
                                               x.Cx <- Int64Value 990000L
                                               x.Cy <- Int64Value 792000L
                                               x
                                           ),
                                       match 
                                           new A.PresetGeometry
                                           (
                                               new A.AdjustValueList()
                                               )
                                           with
                                       | x ->
                                           x.Preset <- EnumValue<_> A.ShapeTypeValues.Rectangle
                                           x
                                       )
                                   )
                               )
                           with
                       | x ->
                           x.Uri<-StringValue "http://schemas.openxmlformats.org/drawingml/2006/picture"
                           x
                       )
                   )
               with
           | x ->
               x.DistanceFromTop <- UInt32Value 0u
               x.DistanceFromBottom<- UInt32Value 0u
               x.DistanceFromLeft<-UInt32Value 0u
               x.DistanceFromRight<- UInt32Value 0u
               x.EditId<-HexBinaryValue "50D07946"
               x
           )

  // Append the reference to body, the element should be in a Run.
  wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)))|>ignore



let InsertAPicture(docPath:string, imagePath:string)=
    use  doc =WordprocessingDocument.Open(docPath, true)
    let mainPart = doc.MainDocumentPart
    let imagePart = mainPart.AddImagePart(ImagePartType.Png)
    use stream = new FileStream(imagePath, FileMode.Open)
    imagePart.FeedData stream
    //Exporter.AddImageToBody(doc, mainPart.GetIdOfPart(imagePart))
    AddImageToBodyX(doc, mainPart.GetIdOfPart(imagePart))

InsertAPicture(templatePath,imagePath)

try
  AddImageToBody(doc, doc.MainDocumentPart.GetIdOfPart(image))|>ignore
with e ->ObjectDumper.Write (e,3)




//let doc = WordprocessingDocument.Open((fileName:string), editable)

(*正确参考，创建文件
let filePath= @"d:\Temp\Testing\TestDoc.docx"
let doc=WordprocessingDocument.Create(filePath,WordprocessingDocumentType.Document,true)

let main= doc.AddMainDocumentPart()
main.Document<-new Document()
let body = main.Document.AppendChild(new Body())

let paragraph = body.AppendChild(new Paragraph())
let run = paragraph.AppendChild(new Run())
run.AppendChild(new Text("在 body 本文內容產生 text 文字"))

main.Document.Save()

doc.Close()



//正确参考，使用Stream创建， 顺序不能变
let filePath= @"d:\Temp\Testing\TestDoc.docx"
let memory=new MemoryStream()
let doc=WordprocessingDocument.Create(memory,WordprocessingDocumentType.Document,true)

let main= doc.AddMainDocumentPart()
main.Document<-new Document()
let body = main.Document.AppendChild(new Body())
let paragraph = body.AppendChild(new Paragraph())
let run = paragraph.AppendChild(new Run())
run.AppendChild(new Text("在 body 本文內容產生 text 文字"))
main.Document.Save()

let fs= new FileStream(filePath, FileMode.Create)
doc.Close() //关闭之后，才将所有内存流写到自定义的"memory"中,main.Document.Save()只保存文档内容部分
memory.Position<- 0L
memory.WriteTo fs
memory.Close()
fs.Close()

//------------------------------------------------

let byteArray = File.ReadAllBytes filePath
let mem = new MemoryStream() 
mem.Write(byteArray, 0, (int)byteArray.Length) 
let doc = WordprocessingDocument.Open(mem, true) 
mem.Close
doc.Close()

(* 错误藏考，命名空间混淆(OpenXml.Wordprocessing.Drawing 及 OpenXml.Drawing混淆)
let AddImageToBody(doc:WordprocessingDocument, relationshipId:string)=
    // Define the reference of the image.
    let element =
         new Drawing(
             new Drawing.Wordprocessing.Inline(
                 new Drawing.Wordprocessing.Extent(Cx = Int64Value 990000L, Cy = Int64Value 792000L),
                 new Drawing.Wordprocessing.EffectExtent( LeftEdge = Int64Value 0L, TopEdge = Int64Value 0L, 
                     RightEdge = Int64Value 0L, BottomEdge = Int64Value 0L),
                 new Drawing.Wordprocessing.DocProperties(Id = UInt32Value 1u, Name = StringValue "Picture 1"), 
                 new Drawing.Wordprocessing.NonVisualGraphicFrameDrawingProperties(
                      new Drawing.GraphicFrameLocks(NoChangeAspect = BooleanValue true )), 
                 new Drawing.Graphic(
                     new Drawing.GraphicData(
                         new Drawing.Pictures.Picture(
                             new Drawing.Pictures.NonVisualPictureProperties(
                                 new Drawing.Pictures.NonVisualDrawingProperties( Id = UInt32Value 0u, Name = StringValue "New Bitmap Image.jpg" ),
                                 new Drawing.Pictures.NonVisualPictureDrawingProperties()),
                             new Drawing.Pictures.BlipFill(
                                 new Drawing.Blip(
                                     new Drawing.BlipExtensionList(
                                         new Drawing.BlipExtension( Uri = StringValue "{28A0092B-C50C-407E-A947-70E740481C1C}" )),
                                     Embed = StringValue relationshipId, CompressionState = EnumValue<_> Drawing.BlipCompressionValues.Print),
                                 new Drawing.Stretch(
                                     new Drawing.FillRectangle())),
                             new Drawing.Pictures.ShapeProperties(
                                 new Drawing.Transform2D(
                                     new Drawing.Offset( X = Int64Value 0L, Y = Int64Value 0L ),
                                     new Drawing.Extents( Cx = Int64Value 990000L, Cy = Int64Value 792000L )),
                                 new Drawing.PresetGeometry(
                                     new Drawing.AdjustValueList(),
                                     Preset = EnumValue<_> Drawing.ShapeTypeValues.Rectangle))),
                         Uri = StringValue "http://schemas.openxmlformats.org/drawingml/2006/picture" )),
                 DistanceFromTop = UInt32Value 0u, DistanceFromBottom = UInt32Value 0u, 
                 DistanceFromLeft = UInt32Value 0u, DistanceFromRight = UInt32Value 0u, EditId = HexBinaryValue "50D07946" ))
    // Append the reference to body, the element should be in a Run.
    doc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)))
    *)

*)