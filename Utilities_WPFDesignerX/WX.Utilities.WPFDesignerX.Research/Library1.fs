namespace WX.Utilities.WPFDesignerX.Office.Word
open DocumentFormat.OpenXml
open DocumentFormat.OpenXml.Wordprocessing
open DocumentFormat.OpenXml.Packaging

type ExporterX=
  static member  AddImageToBody ( wordDoc:WordprocessingDocument, relationshipId:string)=
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
                           Uri = StringValue "http://schemas.openxmlformats.org/drawingml/2006/picture" )
                       ),
                   DistanceFromTop = UInt32Value 0u,DistanceFromBottom = UInt32Value 0u,
                   DistanceFromLeft = UInt32Value 0u,DistanceFromRight = UInt32Value 0u,EditId = HexBinaryValue "50D07946"
                   )
               )
      // Append the reference to body, the element should be in a Run.
      wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)))|>ignore