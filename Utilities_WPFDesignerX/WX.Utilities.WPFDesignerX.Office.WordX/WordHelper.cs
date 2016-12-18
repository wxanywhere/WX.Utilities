using System;
using System.IO;
using System.Windows.Media.Imaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
//using System.Windows.Media;


namespace WX.Utilities.WPFDesignerX.Office.Word
{
    public class WordHelper
    {
        private static readonly XNamespace ns = @"http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        public static void InsertImage(WordprocessingDocument wordDoc, byte[] buffer, double height, double width, ImagePartType imagePartType)
        {
            using (var imageStream = new MemoryStream(buffer))
            {
                MainDocumentPart mainPart = wordDoc.MainDocumentPart;

                ImagePart imagePart = mainPart.AddImagePart(imagePartType);
                
                imagePart.FeedData(imageStream);
                const decimal emusPerCm = 360000.0m; //const float emusPerInch = 914400.0f;
                var maxWidthCm = 21.0m - 3.17m * 2.0m; //A4 21*29.7 cm
                var ratio = height / width;
                var maxWidthEmus = (Int64)(maxWidthCm * emusPerCm);
                var maxHeightEmus = (Int64)(maxWidthEmus * ratio);
                AddImageToBody(wordDoc, mainPart.GetIdOfPart(imagePart), maxWidthEmus, maxHeightEmus);

            }
        }

        private static void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId, Int64 cx, Int64 cy)
        {
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = cx, Cy = cy },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                       "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = cx, Cy = cy }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         ) { Preset = A.ShapeTypeValues.Rectangle }))
                             ) { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            // Append the reference to body, the element should be in a Run.
            wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        }

        // Creates an Paragraph instance and adds its children.
        public static Paragraph AddHeadingOneParagraph(string headingText)
        {
            Paragraph paragraph1 = new Paragraph() { RsidParagraphMarkRevision = "000F5D5C", RsidParagraphAddition = "007E2267", RsidParagraphProperties = "007E2267", RsidRunAdditionDefault = "00931963" };

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "Heading1" };

            NumberingProperties numberingProperties1 = new NumberingProperties();
            NumberingLevelReference numberingLevelReference1 = new NumberingLevelReference() { Val = 0 };
            NumberingId numberingId1 = new NumberingId() { Val = 1 };

            numberingProperties1.Append(numberingLevelReference1);
            numberingProperties1.Append(numberingId1);

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            FontSize fontSize1 = new FontSize() { Val = "28" };
            FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = "28" };

            paragraphMarkRunProperties1.Append(fontSize1);
            paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

            SectionProperties sectionProperties1 = new SectionProperties() { RsidRPr = "000F5D5C", RsidR = "007E2267", RsidSect = "007E2267" };
            SectionType sectionType1 = new SectionType() { Val = SectionMarkValues.Continuous };
            PageSize pageSize1 = new PageSize() { Width = (UInt32Value)11906U, Height = (UInt32Value)16838U };
            PageMargin pageMargin1 = new PageMargin() { Top = 1440, Right = (UInt32Value)1800U, Bottom = 1440, Left = (UInt32Value)1800U, Header = (UInt32Value)851U, Footer = (UInt32Value)992U, Gutter = (UInt32Value)0U };
            Columns columns1 = new Columns() { Space = "425" };
            DocGrid docGrid1 = new DocGrid() { Type = DocGridValues.Lines, LinePitch = 312 };

            sectionProperties1.Append(sectionType1);
            sectionProperties1.Append(pageSize1);
            sectionProperties1.Append(pageMargin1);
            sectionProperties1.Append(columns1);
            sectionProperties1.Append(docGrid1);

            paragraphProperties1.Append(paragraphStyleId1);
            paragraphProperties1.Append(numberingProperties1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);
            paragraphProperties1.Append(sectionProperties1);
            BookmarkStart bookmarkStart1 = new BookmarkStart() { Name = "_Toc350166364", Id = "2" };

            Run run1 = new Run() { RsidRunProperties = "00BA4497" };

            RunProperties runProperties1 = new RunProperties();
            RunFonts runFonts1 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSize fontSize2 = new FontSize() { Val = "28" };
            FontSizeComplexScript fontSizeComplexScript2 = new FontSizeComplexScript() { Val = "28" };

            runProperties1.Append(runFonts1);
            runProperties1.Append(fontSize2);
            runProperties1.Append(fontSizeComplexScript2);
            LastRenderedPageBreak lastRenderedPageBreak1 = new LastRenderedPageBreak();
            Text text1 = new Text();
            text1.Text = headingText;

            run1.Append(runProperties1);
            run1.Append(lastRenderedPageBreak1);
            run1.Append(text1);
            BookmarkEnd bookmarkEnd1 = new BookmarkEnd() { Id = "2" };

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(bookmarkStart1);
            paragraph1.Append(run1);
            paragraph1.Append(bookmarkEnd1);
            return paragraph1;
        }

        // Creates an Paragraph instance and adds its children.
        public static Paragraph AddHeadingTwoParagraph(string headingText)
        {
            Paragraph paragraph1 = new Paragraph() { RsidParagraphMarkRevision = "00FF4B9F", RsidParagraphAddition = "007E2267", RsidParagraphProperties = "00A34256", RsidRunAdditionDefault = "00B757C5" };

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "Heading2" };

            NumberingProperties numberingProperties1 = new NumberingProperties();
            NumberingLevelReference numberingLevelReference1 = new NumberingLevelReference() { Val = 1 };
            NumberingId numberingId1 = new NumberingId() { Val = 2 };

            numberingProperties1.Append(numberingLevelReference1);
            numberingProperties1.Append(numberingId1);

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            RunFonts runFonts1 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
            FontSize fontSize1 = new FontSize() { Val = "28" };
            FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = "28" };

            paragraphMarkRunProperties1.Append(runFonts1);
            paragraphMarkRunProperties1.Append(fontSize1);
            paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

            paragraphProperties1.Append(paragraphStyleId1);
            paragraphProperties1.Append(numberingProperties1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);
            //BookmarkStart bookmarkStart1 = new BookmarkStart() { Name = "_Toc350166365", Id = "3" };
            //ProofError proofError1 = new ProofError() { Type = ProofingErrorValues.SpellStart };

            Run run1 = new Run() { RsidRunProperties = "00FF4B9F" };

            RunProperties runProperties1 = new RunProperties();
            RunFonts runFonts2 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
            FontSize fontSize2 = new FontSize() { Val = "28" };
            FontSizeComplexScript fontSizeComplexScript2 = new FontSizeComplexScript() { Val = "28" };

            runProperties1.Append(runFonts2);
            runProperties1.Append(fontSize2);
            runProperties1.Append(fontSizeComplexScript2);
            LastRenderedPageBreak lastRenderedPageBreak1 = new LastRenderedPageBreak();
            Text text1 = new Text();
            text1.Text = headingText;

            run1.Append(runProperties1);
            run1.Append(lastRenderedPageBreak1);
            run1.Append(text1);
            //BookmarkEnd bookmarkEnd1 = new BookmarkEnd() { Id = "3" };
            //ProofError proofError2 = new ProofError() { Type = ProofingErrorValues.SpellEnd };

            paragraph1.Append(paragraphProperties1);
            //paragraph1.Append(bookmarkStart1);
            //paragraph1.Append(proofError1);
            paragraph1.Append(run1);
            //paragraph1.Append(bookmarkEnd1);
            //paragraph1.Append(proofError2);
            return paragraph1;
        }

        public static Paragraph AddHeadingThreeParagraph(string headingText)
        {
            Paragraph paragraph1 = new Paragraph() { RsidParagraphMarkRevision = "00274974", RsidParagraphAddition = "00787B85", RsidParagraphProperties = "00274974", RsidRunAdditionDefault = "00714564" };

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "Heading3" };

            NumberingProperties numberingProperties1 = new NumberingProperties();
            NumberingLevelReference numberingLevelReference1 = new NumberingLevelReference() { Val = 2 };
            NumberingId numberingId1 = new NumberingId() { Val = 2 };

            numberingProperties1.Append(numberingLevelReference1);
            numberingProperties1.Append(numberingId1);

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            RunFonts runFonts1 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
            FontSize fontSize1 = new FontSize() { Val = "28" };
            FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = "28" };

            paragraphMarkRunProperties1.Append(runFonts1);
            paragraphMarkRunProperties1.Append(fontSize1);
            paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

            paragraphProperties1.Append(paragraphStyleId1);
            paragraphProperties1.Append(numberingProperties1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);
            ProofError proofError1 = new ProofError() { Type = ProofingErrorValues.SpellStart };
            ProofError proofError2 = new ProofError() { Type = ProofingErrorValues.GrammarStart };

            Run run1 = new Run() { RsidRunProperties = "00274974" };

            RunProperties runProperties1 = new RunProperties();
            RunFonts runFonts2 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
            FontSize fontSize2 = new FontSize() { Val = "28" };
            FontSizeComplexScript fontSizeComplexScript2 = new FontSizeComplexScript() { Val = "28" };

            runProperties1.Append(runFonts2);
            runProperties1.Append(fontSize2);
            runProperties1.Append(fontSizeComplexScript2);
            Text text1 = new Text();
            text1.Text = headingText;

            run1.Append(runProperties1);
            run1.Append(text1);
            ProofError proofError3 = new ProofError() { Type = ProofingErrorValues.SpellEnd };
            ProofError proofError4 = new ProofError() { Type = ProofingErrorValues.GrammarEnd };

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(proofError1);
            paragraph1.Append(proofError2);
            paragraph1.Append(run1);
            paragraph1.Append(proofError3);
            paragraph1.Append(proofError4);
            return paragraph1;
        }

        // Creates an Table instance and adds its children.
        public static Table AddServiceInfoTable(WordServiceInfo[] data)
        {
            Table table1 = new Table();

            TableProperties tableProperties1 = new TableProperties();
            TableStyle tableStyle1 = new TableStyle() { Val = "TableGrid" };
            TableWidth tableWidth1 = new TableWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableLook tableLook1 = new TableLook() { Val = "04A0", FirstRow = true, LastRow = false, FirstColumn = true, LastColumn = false, NoHorizontalBand = false, NoVerticalBand = true };

            tableProperties1.Append(tableStyle1);
            tableProperties1.Append(tableWidth1);
            tableProperties1.Append(tableLook1);

            TableGrid tableGrid1 = new TableGrid();
            GridColumn gridColumn1 = new GridColumn() { Width = "638" };
            GridColumn gridColumn2 = new GridColumn() { Width = "1171" };
            GridColumn gridColumn3 = new GridColumn() { Width = "4395" };
            GridColumn gridColumn4 = new GridColumn() { Width = "708" };
            GridColumn gridColumn5 = new GridColumn() { Width = "1610" };

            tableGrid1.Append(gridColumn1);
            tableGrid1.Append(gridColumn2);
            tableGrid1.Append(gridColumn3);
            tableGrid1.Append(gridColumn4);
            tableGrid1.Append(gridColumn5);

            TableRow tableRow1 = new TableRow() { RsidTableRowAddition = "00880173", RsidTableRowProperties = "004D67BD" };

            TableCell tableCell1 = new TableCell();

            TableCellProperties tableCellProperties1 = new TableCellProperties();
            TableCellWidth tableCellWidth1 = new TableCellWidth() { Width = "638", Type = TableWidthUnitValues.Dxa };
            Shading shading1 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties1.Append(tableCellWidth1);
            tableCellProperties1.Append(shading1);

            Paragraph paragraph1 = new Paragraph() { RsidParagraphMarkRevision = "00FA7B50", RsidParagraphAddition = "00880173", RsidParagraphProperties = "00485314", RsidRunAdditionDefault = "00880173" };

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            Justification justification1 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

            paragraphProperties1.Append(justification1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);

            Run run1 = new Run() { RsidRunProperties = "00FA7B50" };

            RunProperties runProperties1 = new RunProperties();
            RunFonts runFonts1 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript2 = new FontSizeComplexScript() { Val = "21" };

            runProperties1.Append(runFonts1);
            runProperties1.Append(fontSizeComplexScript2);
            Text text1 = new Text();
            text1.Text = "序号";

            run1.Append(runProperties1);
            run1.Append(text1);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);

            tableCell1.Append(tableCellProperties1);
            tableCell1.Append(paragraph1);

            TableCell tableCell2 = new TableCell();

            TableCellProperties tableCellProperties2 = new TableCellProperties();
            TableCellWidth tableCellWidth2 = new TableCellWidth() { Width = "1171", Type = TableWidthUnitValues.Dxa };
            Shading shading2 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties2.Append(tableCellWidth2);
            tableCellProperties2.Append(shading2);

            Paragraph paragraph2 = new Paragraph() { RsidParagraphMarkRevision = "00FA7B50", RsidParagraphAddition = "00880173", RsidParagraphProperties = "00485314", RsidRunAdditionDefault = "00880173" };

            ParagraphProperties paragraphProperties2 = new ParagraphProperties();
            Justification justification2 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties2 = new ParagraphMarkRunProperties();
            FontSizeComplexScript fontSizeComplexScript3 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties2.Append(fontSizeComplexScript3);

            paragraphProperties2.Append(justification2);
            paragraphProperties2.Append(paragraphMarkRunProperties2);

            Run run2 = new Run();

            RunProperties runProperties2 = new RunProperties();
            RunFonts runFonts2 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript4 = new FontSizeComplexScript() { Val = "21" };

            runProperties2.Append(runFonts2);
            runProperties2.Append(fontSizeComplexScript4);
            Text text2 = new Text();
            text2.Text = "服务名称";

            run2.Append(runProperties2);
            run2.Append(text2);

            paragraph2.Append(paragraphProperties2);
            paragraph2.Append(run2);

            tableCell2.Append(tableCellProperties2);
            tableCell2.Append(paragraph2);

            TableCell tableCell3 = new TableCell();

            TableCellProperties tableCellProperties3 = new TableCellProperties();
            TableCellWidth tableCellWidth3 = new TableCellWidth() { Width = "4395", Type = TableWidthUnitValues.Dxa };
            Shading shading3 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties3.Append(tableCellWidth3);
            tableCellProperties3.Append(shading3);

            Paragraph paragraph3 = new Paragraph() { RsidParagraphMarkRevision = "00FA7B50", RsidParagraphAddition = "00880173", RsidParagraphProperties = "00485314", RsidRunAdditionDefault = "00880173" };

            ParagraphProperties paragraphProperties3 = new ParagraphProperties();
            Justification justification3 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties3 = new ParagraphMarkRunProperties();
            FontSizeComplexScript fontSizeComplexScript5 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties3.Append(fontSizeComplexScript5);

            paragraphProperties3.Append(justification3);
            paragraphProperties3.Append(paragraphMarkRunProperties3);

            Run run3 = new Run();

            RunProperties runProperties3 = new RunProperties();
            RunFonts runFonts3 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript6 = new FontSizeComplexScript() { Val = "21" };

            runProperties3.Append(runFonts3);
            runProperties3.Append(fontSizeComplexScript6);
            Text text3 = new Text();
            text3.Text = "服务描述";

            run3.Append(runProperties3);
            run3.Append(text3);

            paragraph3.Append(paragraphProperties3);
            paragraph3.Append(run3);

            tableCell3.Append(tableCellProperties3);
            tableCell3.Append(paragraph3);

            TableCell tableCell4 = new TableCell();

            TableCellProperties tableCellProperties4 = new TableCellProperties();
            TableCellWidth tableCellWidth4 = new TableCellWidth() { Width = "708", Type = TableWidthUnitValues.Dxa };
            Shading shading4 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties4.Append(tableCellWidth4);
            tableCellProperties4.Append(shading4);

            Paragraph paragraph4 = new Paragraph() { RsidParagraphMarkRevision = "00FA7B50", RsidParagraphAddition = "00880173", RsidParagraphProperties = "00485314", RsidRunAdditionDefault = "00880173" };

            ParagraphProperties paragraphProperties4 = new ParagraphProperties();
            Justification justification4 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties4 = new ParagraphMarkRunProperties();
            FontSizeComplexScript fontSizeComplexScript7 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties4.Append(fontSizeComplexScript7);

            paragraphProperties4.Append(justification4);
            paragraphProperties4.Append(paragraphMarkRunProperties4);

            Run run4 = new Run();

            RunProperties runProperties4 = new RunProperties();
            RunFonts runFonts4 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript8 = new FontSizeComplexScript() { Val = "21" };

            runProperties4.Append(runFonts4);
            runProperties4.Append(fontSizeComplexScript8);
            Text text4 = new Text();
            text4.Text = "引用";

            run4.Append(runProperties4);
            run4.Append(text4);

            paragraph4.Append(paragraphProperties4);
            paragraph4.Append(run4);

            tableCell4.Append(tableCellProperties4);
            tableCell4.Append(paragraph4);

            TableCell tableCell5 = new TableCell();

            TableCellProperties tableCellProperties5 = new TableCellProperties();
            TableCellWidth tableCellWidth5 = new TableCellWidth() { Width = "1610", Type = TableWidthUnitValues.Dxa };
            Shading shading5 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties5.Append(tableCellWidth5);
            tableCellProperties5.Append(shading5);

            Paragraph paragraph5 = new Paragraph() { RsidParagraphMarkRevision = "00FA7B50", RsidParagraphAddition = "00880173", RsidParagraphProperties = "00485314", RsidRunAdditionDefault = "00880173" };

            ParagraphProperties paragraphProperties5 = new ParagraphProperties();
            Justification justification5 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties5 = new ParagraphMarkRunProperties();
            FontSizeComplexScript fontSizeComplexScript9 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties5.Append(fontSizeComplexScript9);

            paragraphProperties5.Append(justification5);
            paragraphProperties5.Append(paragraphMarkRunProperties5);

            Run run5 = new Run();

            RunProperties runProperties5 = new RunProperties();
            RunFonts runFonts5 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript10 = new FontSizeComplexScript() { Val = "21" };

            runProperties5.Append(runFonts5);
            runProperties5.Append(fontSizeComplexScript10);
            Text text5 = new Text();
            text5.Text = "代码";

            run5.Append(runProperties5);
            run5.Append(text5);

            paragraph5.Append(paragraphProperties5);
            paragraph5.Append(run5);

            tableCell5.Append(tableCellProperties5);
            tableCell5.Append(paragraph5);

            tableRow1.Append(tableCell1);
            tableRow1.Append(tableCell2);
            tableRow1.Append(tableCell3);
            tableRow1.Append(tableCell4);
            tableRow1.Append(tableCell5);

            table1.Append(tableProperties1);
            table1.Append(tableGrid1);
            table1.Append(tableRow1);

            for (int i = 0; i < data.Length; i++)
            {
                TableRow tableRow2 = new TableRow() { RsidTableRowAddition = "00880173", RsidTableRowProperties = "004D67BD" };

                TableRowProperties tableRowProperties1 = new TableRowProperties();
                TableRowHeight tableRowHeight1 = new TableRowHeight() { Val = (UInt32Value)261U };

                tableRowProperties1.Append(tableRowHeight1);

                TableCell tableCell6 = new TableCell();

                TableCellProperties tableCellProperties6 = new TableCellProperties();
                TableCellWidth tableCellWidth6 = new TableCellWidth() { Width = "638", Type = TableWidthUnitValues.Dxa };

                tableCellProperties6.Append(tableCellWidth6);

                Paragraph paragraph6 = new Paragraph() { RsidParagraphMarkRevision = "000A24C7", RsidParagraphAddition = "00880173", RsidParagraphProperties = "00485314", RsidRunAdditionDefault = "00880173" };

                ParagraphProperties paragraphProperties6 = new ParagraphProperties();
                Justification justification6 = new Justification() { Val = JustificationValues.Center };

                ParagraphMarkRunProperties paragraphMarkRunProperties6 = new ParagraphMarkRunProperties();
                RunFonts runFonts6 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize1 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript11 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties6.Append(runFonts6);
                paragraphMarkRunProperties6.Append(fontSize1);
                paragraphMarkRunProperties6.Append(fontSizeComplexScript11);

                paragraphProperties6.Append(justification6);
                paragraphProperties6.Append(paragraphMarkRunProperties6);

                Run run6 = new Run() { RsidRunProperties = "000A24C7" };

                RunProperties runProperties6 = new RunProperties();
                RunFonts runFonts7 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize2 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript12 = new FontSizeComplexScript() { Val = "15" };

                runProperties6.Append(runFonts7);
                runProperties6.Append(fontSize2);
                runProperties6.Append(fontSizeComplexScript12);
                Text text6 = new Text();
                text6.Text = (i+1).ToString();

                run6.Append(runProperties6);
                run6.Append(text6);

                paragraph6.Append(paragraphProperties6);
                paragraph6.Append(run6);

                tableCell6.Append(tableCellProperties6);
                tableCell6.Append(paragraph6);

                TableCell tableCell7 = new TableCell();

                TableCellProperties tableCellProperties7 = new TableCellProperties();
                TableCellWidth tableCellWidth7 = new TableCellWidth() { Width = "1171", Type = TableWidthUnitValues.Dxa };

                tableCellProperties7.Append(tableCellWidth7);

                Paragraph paragraph7 = new Paragraph() { RsidParagraphMarkRevision = "000A24C7", RsidParagraphAddition = "00880173", RsidParagraphProperties = "00485314", RsidRunAdditionDefault = "00880173" };

                ParagraphProperties paragraphProperties7 = new ParagraphProperties();

                ParagraphMarkRunProperties paragraphMarkRunProperties7 = new ParagraphMarkRunProperties();
                RunFonts runFonts8 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize3 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript13 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties7.Append(runFonts8);
                paragraphMarkRunProperties7.Append(fontSize3);
                paragraphMarkRunProperties7.Append(fontSizeComplexScript13);

                paragraphProperties7.Append(paragraphMarkRunProperties7);
                ProofError proofError1 = new ProofError() { Type = ProofingErrorValues.SpellStart };

                Run run7 = new Run();

                RunProperties runProperties7 = new RunProperties();
                RunFonts runFonts9 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize4 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript14 = new FontSizeComplexScript() { Val = "15" };

                runProperties7.Append(runFonts9);
                runProperties7.Append(fontSize4);
                runProperties7.Append(fontSizeComplexScript14);
                Text text7 = new Text();
                text7.Text = data[i].ServiceName;

                run7.Append(runProperties7);
                run7.Append(text7);
                ProofError proofError2 = new ProofError() { Type = ProofingErrorValues.SpellEnd };

                paragraph7.Append(paragraphProperties7);
                paragraph7.Append(proofError1);
                paragraph7.Append(run7);
                paragraph7.Append(proofError2);

                tableCell7.Append(tableCellProperties7);
                tableCell7.Append(paragraph7);

                TableCell tableCell8 = new TableCell();

                TableCellProperties tableCellProperties8 = new TableCellProperties();
                TableCellWidth tableCellWidth8 = new TableCellWidth() { Width = "4395", Type = TableWidthUnitValues.Dxa };

                tableCellProperties8.Append(tableCellWidth8);

                Paragraph paragraph8 = new Paragraph() { RsidParagraphMarkRevision = "000A24C7", RsidParagraphAddition = "00880173", RsidParagraphProperties = "00485314", RsidRunAdditionDefault = "00880173" };

                ParagraphProperties paragraphProperties8 = new ParagraphProperties();

                ParagraphMarkRunProperties paragraphMarkRunProperties8 = new ParagraphMarkRunProperties();
                RunFonts runFonts10 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize5 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript15 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties8.Append(runFonts10);
                paragraphMarkRunProperties8.Append(fontSize5);
                paragraphMarkRunProperties8.Append(fontSizeComplexScript15);

                paragraphProperties8.Append(paragraphMarkRunProperties8);
                ProofError proofError3 = new ProofError() { Type = ProofingErrorValues.SpellStart };

                Run run8 = new Run();

                RunProperties runProperties8 = new RunProperties();
                RunFonts runFonts11 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize6 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript16 = new FontSizeComplexScript() { Val = "15" };

                runProperties8.Append(runFonts11);
                runProperties8.Append(fontSize6);
                runProperties8.Append(fontSizeComplexScript16);
                Text text8 = new Text();
                text8.Text = data[i].ServiceDescription;

                run8.Append(runProperties8);
                run8.Append(text8);
                BookmarkStart bookmarkStart1 = new BookmarkStart() { Name = "_GoBack", Id = "3" };
                BookmarkEnd bookmarkEnd1 = new BookmarkEnd() { Id = "3" };
                ProofError proofError4 = new ProofError() { Type = ProofingErrorValues.SpellEnd };

                paragraph8.Append(paragraphProperties8);
                paragraph8.Append(proofError3);
                paragraph8.Append(run8);
                paragraph8.Append(bookmarkStart1);
                paragraph8.Append(bookmarkEnd1);
                paragraph8.Append(proofError4);

                tableCell8.Append(tableCellProperties8);
                tableCell8.Append(paragraph8);

                TableCell tableCell9 = new TableCell();

                TableCellProperties tableCellProperties9 = new TableCellProperties();
                TableCellWidth tableCellWidth9 = new TableCellWidth() { Width = "708", Type = TableWidthUnitValues.Dxa };

                tableCellProperties9.Append(tableCellWidth9);

                Paragraph paragraph9 = new Paragraph() { RsidParagraphMarkRevision = "000A24C7", RsidParagraphAddition = "00880173", RsidParagraphProperties = "00880173", RsidRunAdditionDefault = "00332D63" };

                ParagraphProperties paragraphProperties9 = new ParagraphProperties();
                Justification justification7 = new Justification() { Val = JustificationValues.Center };

                ParagraphMarkRunProperties paragraphMarkRunProperties9 = new ParagraphMarkRunProperties();
                RunFonts runFonts12 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize7 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript17 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties9.Append(runFonts12);
                paragraphMarkRunProperties9.Append(fontSize7);
                paragraphMarkRunProperties9.Append(fontSizeComplexScript17);

                paragraphProperties9.Append(justification7);
                paragraphProperties9.Append(paragraphMarkRunProperties9);

                Run run9 = new Run();

                RunProperties runProperties9 = new RunProperties();
                RunFonts runFonts13 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize8 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript18 = new FontSizeComplexScript() { Val = "15" };

                runProperties9.Append(runFonts13);
                runProperties9.Append(fontSize8);
                runProperties9.Append(fontSizeComplexScript18);
                Text text9 = new Text();
                text9.Text = data[i].ReferenceCount.ToString();

                run9.Append(runProperties9);
                run9.Append(text9);

                paragraph9.Append(paragraphProperties9);
                paragraph9.Append(run9);

                tableCell9.Append(tableCellProperties9);
                tableCell9.Append(paragraph9);

                TableCell tableCell10 = new TableCell();

                TableCellProperties tableCellProperties10 = new TableCellProperties();
                TableCellWidth tableCellWidth10 = new TableCellWidth() { Width = "1610", Type = TableWidthUnitValues.Dxa };

                tableCellProperties10.Append(tableCellWidth10);

                Paragraph paragraph10 = new Paragraph() { RsidParagraphMarkRevision = "000A24C7", RsidParagraphAddition = "00880173", RsidParagraphProperties = "00880173", RsidRunAdditionDefault = "00332D63" };

                ParagraphProperties paragraphProperties10 = new ParagraphProperties();
                Justification justification8 = new Justification() { Val = JustificationValues.Center };

                ParagraphMarkRunProperties paragraphMarkRunProperties10 = new ParagraphMarkRunProperties();
                RunFonts runFonts14 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize9 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript19 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties10.Append(runFonts14);
                paragraphMarkRunProperties10.Append(fontSize9);
                paragraphMarkRunProperties10.Append(fontSizeComplexScript19);

                paragraphProperties10.Append(justification8);
                paragraphProperties10.Append(paragraphMarkRunProperties10);

                Run run10 = new Run();

                RunProperties runProperties10 = new RunProperties();
                RunFonts runFonts15 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize10 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript20 = new FontSizeComplexScript() { Val = "15" };

                runProperties10.Append(runFonts15);
                runProperties10.Append(fontSize10);
                runProperties10.Append(fontSizeComplexScript20);
                Text text10 = new Text();
                text10.Text = "";
                switch (data[i].ServiceChangedType)
                {
                    case ChangedType.Added:
                        text10.Text = "新增";
                        break;
                    case ChangedType.Modified:
                        text10.Text = "修改";
                        break;
                    default:
                        text10.Text = "";
                        break;
                }

                run10.Append(runProperties10);
                run10.Append(text10);

                paragraph10.Append(paragraphProperties10);
                paragraph10.Append(run10);

                tableCell10.Append(tableCellProperties10);
                tableCell10.Append(paragraph10);

                tableRow2.Append(tableRowProperties1);
                tableRow2.Append(tableCell6);
                tableRow2.Append(tableCell7);
                tableRow2.Append(tableCell8);
                tableRow2.Append(tableCell9);
                tableRow2.Append(tableCell10);


                table1.Append(tableRow2);
            }
            return table1;
        }



        // Creates an Table instance and adds its children.
        public static Table AddUITable(WordUIInfo[] data)
        {
            Table table1 = new Table();

            TableProperties tableProperties1 = new TableProperties();
            TableStyle tableStyle1 = new TableStyle() { Val = "TableGrid" };
            TableWidth tableWidth1 = new TableWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableLayout tableLayout1 = new TableLayout() { Type = TableLayoutValues.Fixed };
            TableLook tableLook1 = new TableLook() { Val = "04A0", FirstRow = true, LastRow = false, FirstColumn = true, LastColumn = false, NoHorizontalBand = false, NoVerticalBand = true };

            tableProperties1.Append(tableStyle1);
            tableProperties1.Append(tableWidth1);
            tableProperties1.Append(tableLayout1);
            tableProperties1.Append(tableLook1);

            TableGrid tableGrid1 = new TableGrid();
            GridColumn gridColumn1 = new GridColumn() { Width = "675" };
            GridColumn gridColumn2 = new GridColumn() { Width = "1701" };
            GridColumn gridColumn3 = new GridColumn() { Width = "3119" };
            GridColumn gridColumn4 = new GridColumn() { Width = "992" };
            GridColumn gridColumn5 = new GridColumn() { Width = "1134" };
            GridColumn gridColumn6 = new GridColumn() { Width = "901" };

            tableGrid1.Append(gridColumn1);
            tableGrid1.Append(gridColumn2);
            tableGrid1.Append(gridColumn3);
            tableGrid1.Append(gridColumn4);
            tableGrid1.Append(gridColumn5);
            tableGrid1.Append(gridColumn6);

            TableRow tableRow1 = new TableRow() { RsidTableRowAddition = "00777FC5", RsidTableRowProperties = "00777FC5" };

            TableCell tableCell1 = new TableCell();

            TableCellProperties tableCellProperties1 = new TableCellProperties();
            TableCellWidth tableCellWidth1 = new TableCellWidth() { Width = "675", Type = TableWidthUnitValues.Dxa };
            Shading shading1 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties1.Append(tableCellWidth1);
            tableCellProperties1.Append(shading1);

            Paragraph paragraph1 = new Paragraph() { RsidParagraphMarkRevision = "00FA7B50", RsidParagraphAddition = "00777FC5", RsidParagraphProperties = "00E52D06", RsidRunAdditionDefault = "00777FC5" };

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            Justification justification1 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

            paragraphProperties1.Append(justification1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);

            Run run1 = new Run() { RsidRunProperties = "00FA7B50" };

            RunProperties runProperties1 = new RunProperties();
            RunFonts runFonts1 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript2 = new FontSizeComplexScript() { Val = "21" };

            runProperties1.Append(runFonts1);
            runProperties1.Append(fontSizeComplexScript2);
            Text text1 = new Text();
            text1.Text = "序号";

            run1.Append(runProperties1);
            run1.Append(text1);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);

            tableCell1.Append(tableCellProperties1);
            tableCell1.Append(paragraph1);

            TableCell tableCell2 = new TableCell();

            TableCellProperties tableCellProperties2 = new TableCellProperties();
            TableCellWidth tableCellWidth2 = new TableCellWidth() { Width = "1701", Type = TableWidthUnitValues.Dxa };
            Shading shading2 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties2.Append(tableCellWidth2);
            tableCellProperties2.Append(shading2);

            Paragraph paragraph2 = new Paragraph() { RsidParagraphMarkRevision = "00FA7B50", RsidParagraphAddition = "00777FC5", RsidParagraphProperties = "00777FC5", RsidRunAdditionDefault = "00777FC5" };

            ParagraphProperties paragraphProperties2 = new ParagraphProperties();
            Justification justification2 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties2 = new ParagraphMarkRunProperties();
            FontSizeComplexScript fontSizeComplexScript3 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties2.Append(fontSizeComplexScript3);

            paragraphProperties2.Append(justification2);
            paragraphProperties2.Append(paragraphMarkRunProperties2);

            Run run2 = new Run();

            RunProperties runProperties2 = new RunProperties();
            RunFonts runFonts2 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript4 = new FontSizeComplexScript() { Val = "21" };

            runProperties2.Append(runFonts2);
            runProperties2.Append(fontSizeComplexScript4);
            Text text2 = new Text();
            text2.Text = "界面名称";

            run2.Append(runProperties2);
            run2.Append(text2);

            paragraph2.Append(paragraphProperties2);
            paragraph2.Append(run2);

            tableCell2.Append(tableCellProperties2);
            tableCell2.Append(paragraph2);

            TableCell tableCell3 = new TableCell();

            TableCellProperties tableCellProperties3 = new TableCellProperties();
            TableCellWidth tableCellWidth3 = new TableCellWidth() { Width = "3119", Type = TableWidthUnitValues.Dxa };
            Shading shading3 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties3.Append(tableCellWidth3);
            tableCellProperties3.Append(shading3);

            Paragraph paragraph3 = new Paragraph() { RsidParagraphMarkRevision = "00FA7B50", RsidParagraphAddition = "00777FC5", RsidParagraphProperties = "00E52D06", RsidRunAdditionDefault = "00777FC5" };

            ParagraphProperties paragraphProperties3 = new ParagraphProperties();
            Justification justification3 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties3 = new ParagraphMarkRunProperties();
            FontSizeComplexScript fontSizeComplexScript5 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties3.Append(fontSizeComplexScript5);

            paragraphProperties3.Append(justification3);
            paragraphProperties3.Append(paragraphMarkRunProperties3);

            Run run3 = new Run();

            RunProperties runProperties3 = new RunProperties();
            RunFonts runFonts3 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript6 = new FontSizeComplexScript() { Val = "21" };

            runProperties3.Append(runFonts3);
            runProperties3.Append(fontSizeComplexScript6);
            Text text3 = new Text();
            text3.Text = "界面标题";

            run3.Append(runProperties3);
            run3.Append(text3);

            paragraph3.Append(paragraphProperties3);
            paragraph3.Append(run3);

            tableCell3.Append(tableCellProperties3);
            tableCell3.Append(paragraph3);

            TableCell tableCell4 = new TableCell();

            TableCellProperties tableCellProperties4 = new TableCellProperties();
            TableCellWidth tableCellWidth4 = new TableCellWidth() { Width = "992", Type = TableWidthUnitValues.Dxa };
            Shading shading4 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties4.Append(tableCellWidth4);
            tableCellProperties4.Append(shading4);

            Paragraph paragraph4 = new Paragraph() { RsidParagraphMarkRevision = "00FA7B50", RsidParagraphAddition = "00777FC5", RsidParagraphProperties = "00777FC5", RsidRunAdditionDefault = "00777FC5" };

            ParagraphProperties paragraphProperties4 = new ParagraphProperties();
            Justification justification4 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties4 = new ParagraphMarkRunProperties();
            FontSizeComplexScript fontSizeComplexScript7 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties4.Append(fontSizeComplexScript7);

            paragraphProperties4.Append(justification4);
            paragraphProperties4.Append(paragraphMarkRunProperties4);

            Run run4 = new Run();

            RunProperties runProperties4 = new RunProperties();
            RunFonts runFonts4 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript8 = new FontSizeComplexScript() { Val = "21" };

            runProperties4.Append(runFonts4);
            runProperties4.Append(fontSizeComplexScript8);
            Text text4 = new Text();
            text4.Text = "标注";

            run4.Append(runProperties4);
            run4.Append(text4);

            paragraph4.Append(paragraphProperties4);
            paragraph4.Append(run4);

            tableCell4.Append(tableCellProperties4);
            tableCell4.Append(paragraph4);

            TableCell tableCell5 = new TableCell();

            TableCellProperties tableCellProperties5 = new TableCellProperties();
            TableCellWidth tableCellWidth5 = new TableCellWidth() { Width = "935", Type = TableWidthUnitValues.Dxa };
            Shading shading5 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties5.Append(tableCellWidth5);
            tableCellProperties5.Append(shading5);

            Paragraph paragraph5 = new Paragraph() { RsidParagraphMarkRevision = "00FA7B50", RsidParagraphAddition = "00777FC5", RsidParagraphProperties = "00777FC5", RsidRunAdditionDefault = "00777FC5" };

            ParagraphProperties paragraphProperties5 = new ParagraphProperties();
            Justification justification5 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties5 = new ParagraphMarkRunProperties();
            FontSizeComplexScript fontSizeComplexScript9 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties5.Append(fontSizeComplexScript9);

            paragraphProperties5.Append(justification5);
            paragraphProperties5.Append(paragraphMarkRunProperties5);

            Run run5 = new Run();

            RunProperties runProperties5 = new RunProperties();
            RunFonts runFonts5 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript10 = new FontSizeComplexScript() { Val = "21" };

            runProperties5.Append(runFonts5);
            runProperties5.Append(fontSizeComplexScript10);
            Text text5 = new Text();
            text5.Text = "主窗口";

            run5.Append(runProperties5);
            run5.Append(text5);

            paragraph5.Append(paragraphProperties5);
            paragraph5.Append(run5);

            tableCell5.Append(tableCellProperties5);
            tableCell5.Append(paragraph5);

            TableCell tableCell6 = new TableCell();

            TableCellProperties tableCellProperties6 = new TableCellProperties();
            TableCellWidth tableCellWidth6 = new TableCellWidth() { Width = "1100", Type = TableWidthUnitValues.Dxa };
            Shading shading6 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties6.Append(tableCellWidth6);
            tableCellProperties6.Append(shading6);

            Paragraph paragraph6 = new Paragraph() { RsidParagraphAddition = "00777FC5", RsidParagraphProperties = "00E52D06", RsidRunAdditionDefault = "00777FC5" };

            ParagraphProperties paragraphProperties6 = new ParagraphProperties();
            Justification justification6 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties6 = new ParagraphMarkRunProperties();
            RunFonts runFonts6 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript11 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties6.Append(runFonts6);
            paragraphMarkRunProperties6.Append(fontSizeComplexScript11);

            paragraphProperties6.Append(justification6);
            paragraphProperties6.Append(paragraphMarkRunProperties6);

            Run run6 = new Run();

            RunProperties runProperties6 = new RunProperties();
            RunFonts runFonts7 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript12 = new FontSizeComplexScript() { Val = "21" };

            runProperties6.Append(runFonts7);
            runProperties6.Append(fontSizeComplexScript12);
            Text text6 = new Text();
            text6.Text = "公共窗口";

            run6.Append(runProperties6);
            run6.Append(text6);

            paragraph6.Append(paragraphProperties6);
            paragraph6.Append(run6);

            tableCell6.Append(tableCellProperties6);
            tableCell6.Append(paragraph6);

            tableRow1.Append(tableCell1);
            tableRow1.Append(tableCell2);
            tableRow1.Append(tableCell3);
            tableRow1.Append(tableCell4);
            tableRow1.Append(tableCell5);
            tableRow1.Append(tableCell6);
            table1.Append(tableProperties1); //提前
            table1.Append(tableGrid1);  //提前
            table1.Append(tableRow1); //提前
            for (int i = 0; i < data.Length;i++ ) //*
            {
                TableRow tableRow2 = new TableRow() { RsidTableRowAddition = "00777FC5", RsidTableRowProperties = "00777FC5" };

                TableRowProperties tableRowProperties1 = new TableRowProperties();
                TableRowHeight tableRowHeight1 = new TableRowHeight() { Val = (UInt32Value)261U };

                tableRowProperties1.Append(tableRowHeight1);

                TableCell tableCell7 = new TableCell();

                TableCellProperties tableCellProperties7 = new TableCellProperties();
                TableCellWidth tableCellWidth7 = new TableCellWidth() { Width = "675", Type = TableWidthUnitValues.Dxa };

                tableCellProperties7.Append(tableCellWidth7);

                Paragraph paragraph7 = new Paragraph() { RsidParagraphMarkRevision = "000A24C7", RsidParagraphAddition = "00777FC5", RsidParagraphProperties = "00E52D06", RsidRunAdditionDefault = "00777FC5" };

                ParagraphProperties paragraphProperties7 = new ParagraphProperties();
                Justification justification7 = new Justification() { Val = JustificationValues.Center };

                ParagraphMarkRunProperties paragraphMarkRunProperties7 = new ParagraphMarkRunProperties();
                RunFonts runFonts8 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize1 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript13 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties7.Append(runFonts8);
                paragraphMarkRunProperties7.Append(fontSize1);
                paragraphMarkRunProperties7.Append(fontSizeComplexScript13);

                paragraphProperties7.Append(justification7);
                paragraphProperties7.Append(paragraphMarkRunProperties7);

                Run run7 = new Run() { RsidRunProperties = "000A24C7" };

                RunProperties runProperties7 = new RunProperties();
                RunFonts runFonts9 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize2 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript14 = new FontSizeComplexScript() { Val = "15" };

                runProperties7.Append(runFonts9);
                runProperties7.Append(fontSize2);
                runProperties7.Append(fontSizeComplexScript14);
                Text text7 = new Text();
                text7.Text = (i+1).ToString();

                run7.Append(runProperties7);
                run7.Append(text7);

                paragraph7.Append(paragraphProperties7);
                paragraph7.Append(run7);

                tableCell7.Append(tableCellProperties7);
                tableCell7.Append(paragraph7);

                TableCell tableCell8 = new TableCell();

                TableCellProperties tableCellProperties8 = new TableCellProperties();
                TableCellWidth tableCellWidth8 = new TableCellWidth() { Width = "1701", Type = TableWidthUnitValues.Dxa };

                tableCellProperties8.Append(tableCellWidth8);

                Paragraph paragraph8 = new Paragraph() { RsidParagraphMarkRevision = "000A24C7", RsidParagraphAddition = "00777FC5", RsidParagraphProperties = "00E52D06", RsidRunAdditionDefault = "00777FC5" };

                ParagraphProperties paragraphProperties8 = new ParagraphProperties();

                ParagraphMarkRunProperties paragraphMarkRunProperties8 = new ParagraphMarkRunProperties();
                RunFonts runFonts10 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize3 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript15 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties8.Append(runFonts10);
                paragraphMarkRunProperties8.Append(fontSize3);
                paragraphMarkRunProperties8.Append(fontSizeComplexScript15);

                paragraphProperties8.Append(paragraphMarkRunProperties8);
                ProofError proofError1 = new ProofError() { Type = ProofingErrorValues.SpellStart };

                Run run8 = new Run();

                RunProperties runProperties8 = new RunProperties();
                RunFonts runFonts11 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize4 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript16 = new FontSizeComplexScript() { Val = "15" };

                runProperties8.Append(runFonts11);
                runProperties8.Append(fontSize4);
                runProperties8.Append(fontSizeComplexScript16);
                Text text8 = new Text();
                text8.Text = data[i].UITypeName;

                run8.Append(runProperties8);
                run8.Append(text8);
                ProofError proofError2 = new ProofError() { Type = ProofingErrorValues.SpellEnd };

                paragraph8.Append(paragraphProperties8);
                paragraph8.Append(proofError1);
                paragraph8.Append(run8);
                paragraph8.Append(proofError2);

                tableCell8.Append(tableCellProperties8);
                tableCell8.Append(paragraph8);

                TableCell tableCell9 = new TableCell();

                TableCellProperties tableCellProperties9 = new TableCellProperties();
                TableCellWidth tableCellWidth9 = new TableCellWidth() { Width = "3119", Type = TableWidthUnitValues.Dxa };

                tableCellProperties9.Append(tableCellWidth9);

                Paragraph paragraph9 = new Paragraph() { RsidParagraphMarkRevision = "000A24C7", RsidParagraphAddition = "00777FC5", RsidParagraphProperties = "00E52D06", RsidRunAdditionDefault = "00777FC5" };

                ParagraphProperties paragraphProperties9 = new ParagraphProperties();

                ParagraphMarkRunProperties paragraphMarkRunProperties9 = new ParagraphMarkRunProperties();
                RunFonts runFonts12 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize5 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript17 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties9.Append(runFonts12);
                paragraphMarkRunProperties9.Append(fontSize5);
                paragraphMarkRunProperties9.Append(fontSizeComplexScript17);

                paragraphProperties9.Append(paragraphMarkRunProperties9);
                ProofError proofError3 = new ProofError() { Type = ProofingErrorValues.SpellStart };

                Run run9 = new Run();

                RunProperties runProperties9 = new RunProperties();
                RunFonts runFonts13 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize6 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript18 = new FontSizeComplexScript() { Val = "15" };

                runProperties9.Append(runFonts13);
                runProperties9.Append(fontSize6);
                runProperties9.Append(fontSizeComplexScript18);
                Text text9 = new Text();
                text9.Text = data[i].UIName;

                run9.Append(runProperties9);
                run9.Append(text9);
                BookmarkStart bookmarkStart1 = new BookmarkStart() { Name = "_GoBack", Id = "3" };
                BookmarkEnd bookmarkEnd1 = new BookmarkEnd() { Id = "3" };
                ProofError proofError4 = new ProofError() { Type = ProofingErrorValues.SpellEnd };

                paragraph9.Append(paragraphProperties9);
                paragraph9.Append(proofError3);
                paragraph9.Append(run9);
                paragraph9.Append(bookmarkStart1);
                paragraph9.Append(bookmarkEnd1);
                paragraph9.Append(proofError4);

                tableCell9.Append(tableCellProperties9);
                tableCell9.Append(paragraph9);

                TableCell tableCell10 = new TableCell();

                TableCellProperties tableCellProperties10 = new TableCellProperties();
                TableCellWidth tableCellWidth10 = new TableCellWidth() { Width = "992", Type = TableWidthUnitValues.Dxa };

                tableCellProperties10.Append(tableCellWidth10);

                Paragraph paragraph10 = new Paragraph() { RsidParagraphMarkRevision = "007272EF", RsidParagraphAddition = "00777FC5", RsidParagraphProperties = "007272EF", RsidRunAdditionDefault = "007272EF" };

                ParagraphProperties paragraphProperties10 = new ParagraphProperties();
                ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "ListParagraph" };
                Indentation indentation1 = new Indentation() { Left = "420", FirstLine = "0", FirstLineChars = 0 };

                ParagraphMarkRunProperties paragraphMarkRunProperties10 = new ParagraphMarkRunProperties();
                RunFonts runFonts14 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize7 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript19 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties10.Append(runFonts14);
                paragraphMarkRunProperties10.Append(fontSize7);
                paragraphMarkRunProperties10.Append(fontSizeComplexScript19);

                paragraphProperties10.Append(paragraphStyleId1);
                paragraphProperties10.Append(indentation1);
                paragraphProperties10.Append(paragraphMarkRunProperties10);

                Run run10 = new Run();

                RunProperties runProperties10 = new RunProperties();
                RunFonts runFonts15 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize8 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript20 = new FontSizeComplexScript() { Val = "15" };

                runProperties10.Append(runFonts15);
                runProperties10.Append(fontSize8);
                runProperties10.Append(fontSizeComplexScript20);
                Text text10 = new Text();
                text10.Text = "";
                if (data[i].IsAnnotated)
                {
                    text10.Text = "√";
                }

                run10.Append(runProperties10);
                run10.Append(text10);

                paragraph10.Append(paragraphProperties10);
                paragraph10.Append(run10);

                tableCell10.Append(tableCellProperties10);
                tableCell10.Append(paragraph10);

                TableCell tableCell11 = new TableCell();

                TableCellProperties tableCellProperties11 = new TableCellProperties();
                TableCellWidth tableCellWidth11 = new TableCellWidth() { Width = "1134", Type = TableWidthUnitValues.Dxa };

                tableCellProperties11.Append(tableCellWidth11);

                Paragraph paragraph11 = new Paragraph() { RsidParagraphMarkRevision = "000A24C7", RsidParagraphAddition = "00777FC5", RsidParagraphProperties = "00777FC5", RsidRunAdditionDefault = "007272EF" };

                ParagraphProperties paragraphProperties11 = new ParagraphProperties();
                Justification justification8 = new Justification() { Val = JustificationValues.Center };

                ParagraphMarkRunProperties paragraphMarkRunProperties11 = new ParagraphMarkRunProperties();
                RunFonts runFonts16 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize9 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript21 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties11.Append(runFonts16);
                paragraphMarkRunProperties11.Append(fontSize9);
                paragraphMarkRunProperties11.Append(fontSizeComplexScript21);

                paragraphProperties11.Append(justification8);
                paragraphProperties11.Append(paragraphMarkRunProperties11);

                Run run11 = new Run();

                RunProperties runProperties11 = new RunProperties();
                RunFonts runFonts17 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize10 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript22 = new FontSizeComplexScript() { Val = "15" };

                runProperties11.Append(runFonts17);
                runProperties11.Append(fontSize10);
                runProperties11.Append(fontSizeComplexScript22);
                Text text11 = new Text();
                text11.Text = "";
                if (data[i].IsMainUI)
                {
                    text11.Text = "√";
                }

                run11.Append(runProperties11);
                run11.Append(text11);

                paragraph11.Append(paragraphProperties11);
                paragraph11.Append(run11);

                tableCell11.Append(tableCellProperties11);
                tableCell11.Append(paragraph11);

                TableCell tableCell12 = new TableCell();

                TableCellProperties tableCellProperties12 = new TableCellProperties();
                TableCellWidth tableCellWidth12 = new TableCellWidth() { Width = "901", Type = TableWidthUnitValues.Dxa };

                tableCellProperties12.Append(tableCellWidth12);

                Paragraph paragraph12 = new Paragraph() { RsidParagraphAddition = "00777FC5", RsidParagraphProperties = "00777FC5", RsidRunAdditionDefault = "007272EF" };

                ParagraphProperties paragraphProperties12 = new ParagraphProperties();
                Justification justification9 = new Justification() { Val = JustificationValues.Center };

                ParagraphMarkRunProperties paragraphMarkRunProperties12 = new ParagraphMarkRunProperties();
                RunFonts runFonts18 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize11 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript23 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties12.Append(runFonts18);
                paragraphMarkRunProperties12.Append(fontSize11);
                paragraphMarkRunProperties12.Append(fontSizeComplexScript23);

                paragraphProperties12.Append(justification9);
                paragraphProperties12.Append(paragraphMarkRunProperties12);

                Run run12 = new Run();

                RunProperties runProperties12 = new RunProperties();
                RunFonts runFonts19 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize12 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript24 = new FontSizeComplexScript() { Val = "15" };

                runProperties12.Append(runFonts19);
                runProperties12.Append(fontSize12);
                runProperties12.Append(fontSizeComplexScript24);
                Text text12 = new Text();
                text12.Text = "";
                if (data[i].IsCommonUI)
                {
                    text12.Text = "√";
                }

                run12.Append(runProperties12);
                run12.Append(text12);

                paragraph12.Append(paragraphProperties12);
                paragraph12.Append(run12);

                tableCell12.Append(tableCellProperties12);
                tableCell12.Append(paragraph12);

                tableRow2.Append(tableRowProperties1);
                tableRow2.Append(tableCell7);
                tableRow2.Append(tableCell8);
                tableRow2.Append(tableCell9);
                tableRow2.Append(tableCell10);
                tableRow2.Append(tableCell11);
                tableRow2.Append(tableCell12);

                table1.Append(tableRow2);
            }

            return table1;

        }

        private static Color GetRedColor(){
            return new Color() { Val = "FF0000" }; //红色
        }
        private static Color GetBlueColor()
        {
            return new Color() { Val = "0070C0" }; //蓝色
        }

        // Creates an Table instance and adds its children.
        //没有表格边框时，在模板中粘贴一个符号要求的表格后，删除即可
        public static Table AddAnnotationTable(WordAnnotation[] data)
        {
            Table table1 = new Table();

            TableProperties tableProperties1 = new TableProperties();
            TableStyle tableStyle1 = new TableStyle() { Val = "TableGrid" };
            TableWidth tableWidth1 = new TableWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableLook tableLook1 = new TableLook() { Val = "04A0", FirstRow = true, LastRow = false, FirstColumn = true, LastColumn = false, NoHorizontalBand = false, NoVerticalBand = true };

            tableProperties1.Append(tableStyle1);
            tableProperties1.Append(tableWidth1);
            tableProperties1.Append(tableLook1);

            TableGrid tableGrid1 = new TableGrid();
            GridColumn gridColumn1 = new GridColumn() { Width = "638" };
            GridColumn gridColumn2 = new GridColumn() { Width = "1171" };
            GridColumn gridColumn3 = new GridColumn() { Width = "2410" };
            GridColumn gridColumn4 = new GridColumn() { Width = "1559" };
            GridColumn gridColumn5 = new GridColumn() { Width = "2744" };

            tableGrid1.Append(gridColumn1);
            tableGrid1.Append(gridColumn2);
            tableGrid1.Append(gridColumn3);
            tableGrid1.Append(gridColumn4);
            tableGrid1.Append(gridColumn5);

            TableRow tableRow1 = new TableRow() { RsidTableRowAddition = "00FB4F84", RsidTableRowProperties = "00E06999" };

            TableCell tableCell1 = new TableCell();

            TableCellProperties tableCellProperties1 = new TableCellProperties();
            TableCellWidth tableCellWidth1 = new TableCellWidth() { Width = "638", Type = TableWidthUnitValues.Dxa };
            Shading shading1 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties1.Append(tableCellWidth1);
            tableCellProperties1.Append(shading1);

            Paragraph paragraph1 = new Paragraph() { RsidParagraphMarkRevision = "00FA7B50", RsidParagraphAddition = "00FB4F84", RsidParagraphProperties = "00FA7B50", RsidRunAdditionDefault = "00FB4F84" };

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            Justification justification1 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

            paragraphProperties1.Append(justification1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);

            Run run1 = new Run() { RsidRunProperties = "00FA7B50" };

            RunProperties runProperties1 = new RunProperties();
            RunFonts runFonts1 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript2 = new FontSizeComplexScript() { Val = "21" };

            runProperties1.Append(runFonts1);
            runProperties1.Append(fontSizeComplexScript2);
            Text text1 = new Text();
            text1.Text = "序号";

            run1.Append(runProperties1);
            run1.Append(text1);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);

            tableCell1.Append(tableCellProperties1);
            tableCell1.Append(paragraph1);

            TableCell tableCell2 = new TableCell();

            TableCellProperties tableCellProperties2 = new TableCellProperties();
            TableCellWidth tableCellWidth2 = new TableCellWidth() { Width = "1171", Type = TableWidthUnitValues.Dxa };
            Shading shading2 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties2.Append(tableCellWidth2);
            tableCellProperties2.Append(shading2);

            Paragraph paragraph2 = new Paragraph() { RsidParagraphMarkRevision = "00FA7B50", RsidParagraphAddition = "00FB4F84", RsidParagraphProperties = "00FA7B50", RsidRunAdditionDefault = "00FB4F84" };

            ParagraphProperties paragraphProperties2 = new ParagraphProperties();
            Justification justification2 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties2 = new ParagraphMarkRunProperties();
            FontSizeComplexScript fontSizeComplexScript3 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties2.Append(fontSizeComplexScript3);

            paragraphProperties2.Append(justification2);
            paragraphProperties2.Append(paragraphMarkRunProperties2);

            Run run2 = new Run();

            RunProperties runProperties2 = new RunProperties();
            RunFonts runFonts2 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript4 = new FontSizeComplexScript() { Val = "21" };

            runProperties2.Append(runFonts2);
            runProperties2.Append(fontSizeComplexScript4);
            Text text2 = new Text();
            text2.Text = "界面元素";

            run2.Append(runProperties2);
            run2.Append(text2);

            paragraph2.Append(paragraphProperties2);
            paragraph2.Append(run2);

            tableCell2.Append(tableCellProperties2);
            tableCell2.Append(paragraph2);

            TableCell tableCell3 = new TableCell();

            TableCellProperties tableCellProperties3 = new TableCellProperties();
            TableCellWidth tableCellWidth3 = new TableCellWidth() { Width = "2410", Type = TableWidthUnitValues.Dxa };
            Shading shading3 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties3.Append(tableCellWidth3);
            tableCellProperties3.Append(shading3);

            Paragraph paragraph3 = new Paragraph() { RsidParagraphMarkRevision = "00FA7B50", RsidParagraphAddition = "00FB4F84", RsidParagraphProperties = "00FA7B50", RsidRunAdditionDefault = "00FB4F84" };

            ParagraphProperties paragraphProperties3 = new ParagraphProperties();
            Justification justification3 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties3 = new ParagraphMarkRunProperties();
            RunFonts runFonts3 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript5 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties3.Append(runFonts3);
            paragraphMarkRunProperties3.Append(fontSizeComplexScript5);

            paragraphProperties3.Append(justification3);
            paragraphProperties3.Append(paragraphMarkRunProperties3);

            Run run3 = new Run() { RsidRunProperties = "00FA7B50" };

            RunProperties runProperties3 = new RunProperties();
            RunFonts runFonts4 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript6 = new FontSizeComplexScript() { Val = "21" };

            runProperties3.Append(runFonts4);
            runProperties3.Append(fontSizeComplexScript6);
            Text text3 = new Text();
            text3.Text = "需求描述";

            run3.Append(runProperties3);
            run3.Append(text3);

            paragraph3.Append(paragraphProperties3);
            paragraph3.Append(run3);

            tableCell3.Append(tableCellProperties3);
            tableCell3.Append(paragraph3);

            TableCell tableCell4 = new TableCell();

            TableCellProperties tableCellProperties4 = new TableCellProperties();
            TableCellWidth tableCellWidth4 = new TableCellWidth() { Width = "1559", Type = TableWidthUnitValues.Dxa };
            Shading shading4 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties4.Append(tableCellWidth4);
            tableCellProperties4.Append(shading4);

            Paragraph paragraph4 = new Paragraph() { RsidParagraphMarkRevision = "00FA7B50", RsidParagraphAddition = "00FB4F84", RsidParagraphProperties = "00FA7B50", RsidRunAdditionDefault = "00FB4F84" };

            ParagraphProperties paragraphProperties4 = new ParagraphProperties();
            Justification justification4 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties4 = new ParagraphMarkRunProperties();
            FontSizeComplexScript fontSizeComplexScript7 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties4.Append(fontSizeComplexScript7);

            paragraphProperties4.Append(justification4);
            paragraphProperties4.Append(paragraphMarkRunProperties4);

            Run run4 = new Run() { RsidRunProperties = "00FA7B50" };

            RunProperties runProperties4 = new RunProperties();
            RunFonts runFonts5 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript8 = new FontSizeComplexScript() { Val = "21" };

            runProperties4.Append(runFonts5);
            runProperties4.Append(fontSizeComplexScript8);
            Text text4 = new Text();
            text4.Text = "服务描述";

            run4.Append(runProperties4);
            run4.Append(text4);

            paragraph4.Append(paragraphProperties4);
            paragraph4.Append(run4);

            tableCell4.Append(tableCellProperties4);
            tableCell4.Append(paragraph4);

            TableCell tableCell5 = new TableCell();

            TableCellProperties tableCellProperties5 = new TableCellProperties();
            TableCellWidth tableCellWidth5 = new TableCellWidth() { Width = "2744", Type = TableWidthUnitValues.Dxa };
            Shading shading5 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "8DB3E2", ThemeFill = ThemeColorValues.Text2, ThemeFillTint = "66" };

            tableCellProperties5.Append(tableCellWidth5);
            tableCellProperties5.Append(shading5);

            Paragraph paragraph5 = new Paragraph() { RsidParagraphMarkRevision = "00FA7B50", RsidParagraphAddition = "00FB4F84", RsidParagraphProperties = "00FA7B50", RsidRunAdditionDefault = "00FB4F84" };

            ParagraphProperties paragraphProperties5 = new ParagraphProperties();
            Justification justification5 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties5 = new ParagraphMarkRunProperties();
            FontSizeComplexScript fontSizeComplexScript9 = new FontSizeComplexScript() { Val = "21" };

            paragraphMarkRunProperties5.Append(fontSizeComplexScript9);

            paragraphProperties5.Append(justification5);
            paragraphProperties5.Append(paragraphMarkRunProperties5);

            Run run5 = new Run() { RsidRunProperties = "00FA7B50" };

            RunProperties runProperties5 = new RunProperties();
            RunFonts runFonts6 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            FontSizeComplexScript fontSizeComplexScript10 = new FontSizeComplexScript() { Val = "21" };

            runProperties5.Append(runFonts6);
            runProperties5.Append(fontSizeComplexScript10);
            Text text5 = new Text();
            text5.Text = "交互行为描述";

            run5.Append(runProperties5);
            run5.Append(text5);

            paragraph5.Append(paragraphProperties5);
            paragraph5.Append(run5);

            tableCell5.Append(tableCellProperties5);
            tableCell5.Append(paragraph5);

            tableRow1.Append(tableCell1);
            tableRow1.Append(tableCell2);
            tableRow1.Append(tableCell3);
            tableRow1.Append(tableCell4);
            tableRow1.Append(tableCell5);
            table1.Append(tableProperties1);
            table1.Append(tableGrid1);
            table1.Append(tableRow1);

            for (int i = 0; i < data.Length;i++ )//*
            {
                TableRow tableRow2 = new TableRow() { RsidTableRowAddition = "00FB4F84", RsidTableRowProperties = "00E06999" };

                TableRowProperties tableRowProperties1 = new TableRowProperties();
                TableRowHeight tableRowHeight1 = new TableRowHeight() { Val = (UInt32Value)261U };

                tableRowProperties1.Append(tableRowHeight1);

                TableCell tableCell6 = new TableCell();

                TableCellProperties tableCellProperties6 = new TableCellProperties();
                TableCellWidth tableCellWidth6 = new TableCellWidth() { Width = "638", Type = TableWidthUnitValues.Dxa };

                tableCellProperties6.Append(tableCellWidth6);

                Paragraph paragraph6 = new Paragraph() { RsidParagraphMarkRevision = "000A24C7", RsidParagraphAddition = "00FB4F84", RsidParagraphProperties = "003971DB", RsidRunAdditionDefault = "00FB4F84" };

                ParagraphProperties paragraphProperties6 = new ParagraphProperties();
                Justification justification6 = new Justification() { Val = JustificationValues.Center };

                ParagraphMarkRunProperties paragraphMarkRunProperties6 = new ParagraphMarkRunProperties();
                RunFonts runFonts7 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize1 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript11 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties6.Append(runFonts7);
                paragraphMarkRunProperties6.Append(fontSize1);
                paragraphMarkRunProperties6.Append(fontSizeComplexScript11);

                paragraphProperties6.Append(justification6);
                paragraphProperties6.Append(paragraphMarkRunProperties6);

                Run run6 = new Run() { RsidRunProperties = "000A24C7" };

                RunProperties runProperties6 = new RunProperties();
                RunFonts runFonts8 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize2 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript12 = new FontSizeComplexScript() { Val = "15" };

                runProperties6.Append(runFonts8);
                runProperties6.Append(fontSize2);
                runProperties6.Append(fontSizeComplexScript12);
                Text text6 = new Text();
                text6.Text = (i + 1).ToString(); //*

                run6.Append(runProperties6);
                run6.Append(text6);

                paragraph6.Append(paragraphProperties6);
                paragraph6.Append(run6);

                tableCell6.Append(tableCellProperties6);
                tableCell6.Append(paragraph6);

                TableCell tableCell7 = new TableCell();

                TableCellProperties tableCellProperties7 = new TableCellProperties();
                TableCellWidth tableCellWidth7 = new TableCellWidth() { Width = "1171", Type = TableWidthUnitValues.Dxa };

                tableCellProperties7.Append(tableCellWidth7);

                Paragraph paragraph7 = new Paragraph() { RsidParagraphMarkRevision = "000A24C7", RsidParagraphAddition = "00FB4F84", RsidRunAdditionDefault = "003D7DCA" };

                ParagraphProperties paragraphProperties7 = new ParagraphProperties();

                ParagraphMarkRunProperties paragraphMarkRunProperties7 = new ParagraphMarkRunProperties();
                RunFonts runFonts9 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize3 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript13 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties7.Append(runFonts9);
                paragraphMarkRunProperties7.Append(fontSize3);
                paragraphMarkRunProperties7.Append(fontSizeComplexScript13);

                paragraphProperties7.Append(paragraphMarkRunProperties7);

                Run run7 = new Run();

                RunProperties runProperties7 = new RunProperties();
                RunFonts runFonts10 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };

                FontSize fontSize4 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript14 = new FontSizeComplexScript() { Val = "15" };

                runProperties7.Append(runFonts10);
                switch (data[i].UIElementNameChangedType)
                {
                    case ChangedType.Added:
                        runProperties7.Append(GetBlueColor());
                        break;
                    case ChangedType.Modified:
                        runProperties7.Append(GetRedColor());
                        break;
                    default:
                        break;
                }
                runProperties7.Append(fontSize4);
                runProperties7.Append(fontSizeComplexScript14);
                Text text7 = new Text();
                text7.Text = data[i].UIElementName;  //*

                run7.Append(runProperties7);
                run7.Append(text7);

                paragraph7.Append(paragraphProperties7);
                paragraph7.Append(run7);

                tableCell7.Append(tableCellProperties7);
                tableCell7.Append(paragraph7);

                TableCell tableCell8 = new TableCell();

                TableCellProperties tableCellProperties8 = new TableCellProperties();
                TableCellWidth tableCellWidth8 = new TableCellWidth() { Width = "2410", Type = TableWidthUnitValues.Dxa };

                tableCellProperties8.Append(tableCellWidth8);

                Paragraph paragraph8 = new Paragraph() { RsidParagraphMarkRevision = "000A24C7", RsidParagraphAddition = "00FB4F84", RsidRunAdditionDefault = "003D7DCA" };

                ParagraphProperties paragraphProperties8 = new ParagraphProperties();

                ParagraphMarkRunProperties paragraphMarkRunProperties8 = new ParagraphMarkRunProperties();
                RunFonts runFonts11 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize5 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript15 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties8.Append(runFonts11);
                paragraphMarkRunProperties8.Append(fontSize5);
                paragraphMarkRunProperties8.Append(fontSizeComplexScript15);

                paragraphProperties8.Append(paragraphMarkRunProperties8);

                Run run8 = new Run();

                RunProperties runProperties8 = new RunProperties();
                RunFonts runFonts12 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize6 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript16 = new FontSizeComplexScript() { Val = "15" };

                runProperties8.Append(runFonts12);
                switch (data[i].RequirementDescriptionChangedType)
                {
                    case ChangedType.Added:
                        runProperties8.Append(GetBlueColor());
                        break;
                    case ChangedType.Modified:
                        runProperties8.Append(GetRedColor());
                        break;
                    default:
                        break;
                }
                runProperties8.Append(fontSize6);
                runProperties8.Append(fontSizeComplexScript16);
                Text text8 = new Text();
                text8.Text = data[i].RequirementDescription;  //*

                run8.Append(runProperties8);
                run8.Append(text8);

                paragraph8.Append(paragraphProperties8);
                paragraph8.Append(run8);

                tableCell8.Append(tableCellProperties8);
                tableCell8.Append(paragraph8);

                TableCell tableCell9 = new TableCell();

                TableCellProperties tableCellProperties9 = new TableCellProperties();
                TableCellWidth tableCellWidth9 = new TableCellWidth() { Width = "1559", Type = TableWidthUnitValues.Dxa };

                tableCellProperties9.Append(tableCellWidth9);

                Paragraph paragraph9 = new Paragraph() { RsidParagraphMarkRevision = "000A24C7", RsidParagraphAddition = "00FB4F84", RsidRunAdditionDefault = "003D7DCA" };

                ParagraphProperties paragraphProperties9 = new ParagraphProperties();

                ParagraphMarkRunProperties paragraphMarkRunProperties9 = new ParagraphMarkRunProperties();
                RunFonts runFonts13 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize7 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript17 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties9.Append(runFonts13);
                paragraphMarkRunProperties9.Append(fontSize7);
                paragraphMarkRunProperties9.Append(fontSizeComplexScript17);

                paragraphProperties9.Append(paragraphMarkRunProperties9);

                Run run9 = new Run();

                RunProperties runProperties9 = new RunProperties();
                RunFonts runFonts14 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize8 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript18 = new FontSizeComplexScript() { Val = "15" };

                runProperties9.Append(runFonts14);
                switch (data[i].ServiceInfosChangedType)
                {
                    case ChangedType.Added:
                        runProperties9.Append(GetBlueColor());
                        break;
                    case ChangedType.Modified:
                        runProperties9.Append(GetRedColor());
                        break;
                    default:
                        break;
                }
                runProperties9.Append(fontSize8);
                runProperties9.Append(fontSizeComplexScript18);
                Text text9 = new Text();
                text9.Text = "";  //item.ServicesDescription;  //*

                run9.Append(runProperties9);
                run9.Append(text9);

                paragraph9.Append(paragraphProperties9);
                paragraph9.Append(run9);

                tableCell9.Append(tableCellProperties9);
                tableCell9.Append(paragraph9);

                TableCell tableCell10 = new TableCell();

                TableCellProperties tableCellProperties10 = new TableCellProperties();
                TableCellWidth tableCellWidth10 = new TableCellWidth() { Width = "2744", Type = TableWidthUnitValues.Dxa };

                tableCellProperties10.Append(tableCellWidth10);

                Paragraph paragraph10 = new Paragraph() { RsidParagraphMarkRevision = "000A24C7", RsidParagraphAddition = "00FB4F84", RsidRunAdditionDefault = "003D7DCA" };

                ParagraphProperties paragraphProperties10 = new ParagraphProperties();

                ParagraphMarkRunProperties paragraphMarkRunProperties10 = new ParagraphMarkRunProperties();
                RunFonts runFonts15 = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize9 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript19 = new FontSizeComplexScript() { Val = "15" };

                paragraphMarkRunProperties10.Append(runFonts15);
                paragraphMarkRunProperties10.Append(fontSize9);
                paragraphMarkRunProperties10.Append(fontSizeComplexScript19);

                paragraphProperties10.Append(paragraphMarkRunProperties10);

                Run run10 = new Run();

                RunProperties runProperties10 = new RunProperties();
                RunFonts runFonts16 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                FontSize fontSize10 = new FontSize() { Val = "15" };
                FontSizeComplexScript fontSizeComplexScript20 = new FontSizeComplexScript() { Val = "15" };

                runProperties10.Append(runFonts16);
                switch (data[i].BehaviorDescriptionChangedType)
                {
                    case ChangedType.Added:
                        runProperties10.Append(GetBlueColor());
                        break;
                    case ChangedType.Modified:
                        runProperties10.Append(GetRedColor());
                        break;
                    default:
                        break;
                }
                runProperties10.Append(fontSize10);
                runProperties10.Append(fontSizeComplexScript20);
                Text text10 = new Text();
                text10.Text = data[i].BehaviorDescription;  //*

                run10.Append(runProperties10);
                run10.Append(text10);
                //BookmarkStart bookmarkStart1 = new BookmarkStart() { Name = "_GoBack", Id = "4" };
                //BookmarkEnd bookmarkEnd1 = new BookmarkEnd() { Id = "4" };

                paragraph10.Append(paragraphProperties10);
                paragraph10.Append(run10);
                //paragraph10.Append(bookmarkStart1);
                //paragraph10.Append(bookmarkEnd1);

                tableCell10.Append(tableCellProperties10);
                tableCell10.Append(paragraph10);

                tableRow2.Append(tableRowProperties1);
                tableRow2.Append(tableCell6);
                tableRow2.Append(tableCell7);
                tableRow2.Append(tableCell8);
                tableRow2.Append(tableCell9);
                tableRow2.Append(tableCell10);

                table1.Append(tableRow2);
            }
            
            return table1;
        }

        public static IEnumerable<Paragraph> AddBlankParagraph(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                yield return new Paragraph() { RsidParagraphAddition = "004A448C", RsidRunAdditionDefault = "004A448C" };
            }
        }

        /// <summary>
        /// Just for testing
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <remarks>http://openxmldeveloper.org/discussions/formats/f/13/t/1859.aspx</remarks>
        private static void CreateDocWithTOC(string filePath)
        {
            using (WordprocessingDocument WorDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                // Add a new main document part. 
                MainDocumentPart mainPart = WorDocument.AddMainDocumentPart();
                //Create Document tree for simple document. 
                mainPart.Document = new Document();
                //Create Body (this element contains other elements that we want to include 
                Body body = new Body();


                //Save changes to the main document part. 
                mainPart.Document.Append(body);

                XDocument xmlXdocument = XDocument.Parse(mainPart.Document.InnerXml);
                IEnumerable<XElement> xmlelement = xmlXdocument.Descendants(ns + "p");
                XElement TOCRefNode = xmlelement.First();
                GenerateTOC(xmlXdocument, TOCRefNode);

                mainPart.Document.InnerXml = xmlXdocument.ToString();

                mainPart.Document.Save();
                WorDocument.Close();
            }
        }

        /// <summary>
        /// http://msdn.microsoft.com/zh-cn/library/gg308473.aspx
        /// </summary>
        /// <param name="wordDoc"></param>
        public static void UpdateTOC(WordprocessingDocument wordDoc)
        {
            XDocument xmlXdocument = XDocument.Parse(wordDoc.MainDocumentPart.Document.InnerXml);
            IEnumerable<XElement> xmlelement = xmlXdocument.Descendants(ns + "body");
            XElement TOCRefNode = xmlelement.First();
            GenerateTOC(xmlXdocument, TOCRefNode);

            wordDoc.MainDocumentPart.Document.InnerXml = xmlXdocument.ToString();
            wordDoc.MainDocumentPart.Document.Save();
        }

            /// <summary>
            /// returns title paragraphs of genereated doc fro creating toc hyperlink
            /// </summary>
        private static IEnumerable<XElement> TitleParagraphsElements(XDocument mainDocument)
        {
            IEnumerable<XElement> results = mainDocument.Descendants().Where
              (
                  tag =>
                      tag.Name == ns + "p" &&
                      tag.Descendants(ns + "t").Count() > 0 &&
                      tag.Descendants().Where
                      (
                        tag2 =>
                            tag2.Name == ns + "pStyle" &&
                            (
                                tag2.Attribute(ns + "val").Value == "Head1" ||
                                tag2.Attribute(ns + "val").Value == "Head2" ||
                                tag2.Attribute(ns + "val").Value == "Head3" ||
                                tag2.Attribute(ns + "val").Value == "Head4" ||
                                tag2.Attribute(ns + "val").Value == "Head5" ||
                                tag2.Attribute(ns + "val").Value == "Head6" 
                            )
                      ).Count() > 0
              );

            return results;
        }

        private static  void GenerateTOC(XDocument xmlMainDocument, XElement TOCRefNode)
        {
            int bookMarkIdCounter = 0;
            int maxHeading = 1;
            int tempheading = 1;

            //  sdtContent, will contain all the paragraphs used in the TOC
            XElement sdtContent = new XElement(ns + "sdtContent");
            String strContentHdr = "";
            XElement xContentHdr = TOCRefNode.Elements(ns + "sdtContent").First().Descendants().Where(
                tag =>
                    tag.Name == ns + "p" &&
                    tag.Descendants().Where
                    (
                        tag2 =>
                            tag2.Name == ns + "pStyle" &&
                            tag2.Attribute(ns + "val").Value == "TOCHeading"
                    ).Count() > 0
                ).FirstOrDefault();

            if (xContentHdr != null)
                strContentHdr = xContentHdr.Descendants(ns + "t").FirstOrDefault().Value;

            //  some information regarding the attributes of the TOC
            xContentHdr.Add(

                        new XElement(ns + "r",
                            new XElement(ns + "fldChar",
                                    new XAttribute(ns + "fldCharType", "begin"))),
                        new XElement(ns + "r",
                            new XElement(ns + "instrText",
                                new XAttribute(XNamespace.Xml + "space", "preserve"),
                                "TOCLIMIT")),
                        new XElement(ns + "r",
                            new XElement(ns + "fldChar",
                                new XAttribute(ns + "fldCharType", "separate"))));
            sdtContent.Add(new XElement(xContentHdr));


            //  for each title found it in the document, we have to wrap the run inside of it,
            //  with a bookmark, this bookmark will have an id which will work as an anchor,
            //  for link references in the TOC
            foreach (XElement titleParagraph in TitleParagraphsElements(xmlMainDocument))
            {
                string bookmarkName = "_TOC" + bookMarkIdCounter;
                XElement bookmarkStart =
                    new XElement(ns + "bookmarkStart",
                        new XAttribute(ns + "id", bookMarkIdCounter),
                        new XAttribute(ns + "name", bookmarkName));
                XElement bookmarkEnd =
                    new XElement(ns + "bookmarkEnd",
                        new XAttribute(ns + "id", bookMarkIdCounter));

                //  wrap the run with bookmarkStart and bookmarkEnd
                titleParagraph.AddFirst(bookmarkStart);
                titleParagraph.Add(bookmarkEnd);

                //  get the name of the style of the parapgraph of the title, and for each one,
                //  choose a style to add in the paragraph inside the TOC
                string referenceTitleStyle = "";
                switch (titleParagraph.Descendants(ns + "pStyle").First().Attribute(ns + "val").Value)
                {
                    case "Head1":
                        {
                            referenceTitleStyle = "TOC1";
                            tempheading = 1;
                            break;
                        }
                    case "Head2":
                        {
                            referenceTitleStyle = "TOC2";
                            tempheading = 2;
                            break;
                        }
                    case "Head3":
                        {
                            referenceTitleStyle = "TOC3";
                            tempheading = 3;
                            break;
                        }
                    case "Head4":
                        {
                            referenceTitleStyle = "TOC4";
                            tempheading = 4;
                            break;
                        }
                    case "Head5":
                        {
                            referenceTitleStyle = "TOC5";
                            tempheading = 5;
                            break;
                        }
                    case "Head6":
                        {
                            referenceTitleStyle = "TOC6";
                            tempheading = 6;
                            break;
                        }
                }


                string entryContent = "";
                IEnumerable<XElement> owTList = titleParagraph.Descendants(ns + "t");
                foreach (XElement entryElement in owTList)
                {
                    entryContent += (entryElement == null ? string.Empty : entryElement.Value);
                }

                XElement TOCElement = null;
                XElement tempTOCElement = TOCRefNode.Elements(ns + "sdtContent").First().Descendants().Where(
                tag =>
                    tag.Name == ns + "p" &&
                    tag.Descendants().Where
                    (
                        tag2 =>
                            tag2.Name == ns + "pStyle" &&
                            tag2.Attribute(ns + "val").Value == referenceTitleStyle
                    ).Count() > 0
                ).FirstOrDefault();

                if (tempTOCElement != null)
                {
                    if (maxHeading < tempheading)
                        maxHeading = tempheading;

                    TOCElement = new XElement(tempTOCElement);
                    //delete instrText which contains TOC 1-n
                    XElement instrTextTOC = TOCElement.Descendants().Where(
                            tag =>
                                tag.Name == ns + "r" &&
                                tag.Descendants().Where(
                                    tag2 =>
                                        tag2.Name == ns + "instrText" &&
                                        tag2.Value.Contains(@"TOC \o ")
                                ).Count() > 0

                        ).FirstOrDefault();
                    if (instrTextTOC != null)
                    {
                        instrTextTOC.ElementsAfterSelf(ns + "r").Remove();
                        instrTextTOC.ElementsBeforeSelf(ns + "r").Remove();
                        instrTextTOC.Remove();
                    }

                    //get hyperlink node
                    XElement hyperlink = TOCElement.Descendants().Where(
                            tag =>
                                tag.Name == ns + "hyperlink"

                        ).FirstOrDefault();
                    //update anchor attribute value
                    hyperlink.Attribute(ns + "anchor").Value = bookmarkName;

                    //get entry content node
                    XElement contentNode = hyperlink.Descendants().Where(
                            tag =>
                                tag.Name == ns + "r" &&
                                tag.Descendants().Where(
                                    tag2 =>
                                        tag2.Name == ns + "rStyle" &&
                                        tag2.Attribute(ns + "val").Value == "Hyperlink"
                                ).Count() > 0 &&
                                tag.Elements(ns + "t").Count() > 0

                        ).FirstOrDefault().Elements(ns + "t").FirstOrDefault();

                    contentNode.Value = entryContent;

                    //update PAGEREF value
                    XElement instrText = TOCElement.Descendants().Where(
                            tag =>
                                tag.Name == ns + "instrText" &&
                                tag.Value.Contains("PAGEREF ")
                        ).FirstOrDefault();
                    if (instrText != null)
                    {
                        instrText.Value = " PAGEREF " + bookmarkName + @" \h ";
                    }

                    sdtContent.Add(TOCElement);
                    bookMarkIdCounter++;
                }
            }

            sdtContent.Descendants().Where(
                tag =>
                    tag.Name == ns + "instrText" 
                    &&
                    tag.Value.Contains("TOCLIMIT")
                ).FirstOrDefault().Value = String.Format(@"TOC \o ""1-{0}"" \h \z \u ", maxHeading);

            sdtContent.Add(
                new XElement(ns + "p",
                    new XElement(ns + "r",
                        new XElement(ns + "fldChar",
                            new XAttribute(ns + "fldCharType", "end")))));

            //  Finish the xml construction of the TOC
            XElement TOC =
                new XElement(ns + "sdt",
                    new XElement(ns + "sdtPr",
                        new XElement(ns + "docPartObj",
                            new XElement(ns + "docPartGallery",
                                new XAttribute(ns + "val", "Table of Contents")),
                            new XElement(ns + "docPartUnique"))),
                    sdtContent);

            //  add it to the original document

            IEnumerable<XElement> tocNodes = xmlMainDocument.Descendants().Where
              (
                  tag =>
                      tag.Name == ns + "sdt" &&
                      tag.Descendants(ns + "sdtContent").Count() > 0 &&
                      tag.Descendants().Where
                      (
                        tag2 =>
                            tag2.Name == ns + "p" &&
                            tag2.Descendants().Where
                            (
                                tag3 =>
                                    tag3.Name == ns + "pStyle" &&
                                        (
                                            tag3.Attribute(ns + "val").Value == "TOCHeading"
                                        )
                            ).Count() > 0

                      ).Count() > 0
              );

            TOCRefNode.ReplaceWith(TOC);

        }


    }
}




/*
 * 
Creating a Word Document with the Open XML SDK 2.0
http://blog.stuartwhiteford.com/?p=33
public static void BuildDocument(string fileName)
{
    using (WordprocessingDocument w = WordprocessingDocument.Create(fileName, WordprocessingDocumentType.Document))
    {
        MainDocumentPart mp = w.AddMainDocumentPart();
        Document d = new Document();
        Body b = new Body();
        Paragraph p = new Paragraph();
        Run r = new Run();
        Text t = new Text();
        t.Text = "This is some body text.";
        r.Append(t);
        p.Append(r);
        b.Append(p);
        HeaderPart hp = mp.AddNewPart<HeaderPart>();
        string headerRelationshipID = mp.GetIdOfPart(hp);
        SectionProperties sectPr = new SectionProperties();
        HeaderReference headerReference = new HeaderReference();
        headerReference.Id = headerRelationshipID;
        headerReference.Type = HeaderFooterValues.Default;
        sectPr.Append(headerReference);
        b.Append(sectPr);
        d.Append(b);
        hp.Header = BuildHeader(hp, "This is some header text.");
        hp.Header.Save();
        mp.Document = d;
        mp.Document.Save();
        w.Close();
    }
}

private static Header BuildHeader(HeaderPart hp, string title)
{
    Header h = new Header();
    Paragraph p = new Paragraph();
    Run r = new Run();
    p.Append(r);
    r = new Run();
    RunProperties rPr = new RunProperties();
    TabChar tab = new TabChar();
    Bold b = new Bold();
    Color color = new Color { Val = "006699" };
    FontSize sz = new FontSize { Val = "40" };
    Text t = new Text { Text = title };
    rPr.Append(b);
    rPr.Append(color);
    rPr.Append(sz);
    r.Append(rPr);
    r.Append(tab);
    r.Append(t);
    p.Append(r);
    h.Append(p);
    return h;
}

 * 
http://msdn.microsoft.com/en-us/library/cc850833.aspx
 *
public static void InsertAPicture(string document, string fileName)
{
    using (WordprocessingDocument wordprocessingDocument =
        WordprocessingDocument.Open(document, true))
    {
        MainDocumentPart mainPart = wordprocessingDocument.MainDocumentPart;

        ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

        using (FileStream stream = new FileStream(fileName, FileMode.Open))
        {
            imagePart.FeedData(stream);
        }

        AddImageToBody(wordprocessingDocument, mainPart.GetIdOfPart(imagePart));
    }
}
 * 
 * 
//此处图片从文件中读入用以模拟内存中的图片
System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap("bg.jpg");
MemoryStream stream = new MemoryStream();
bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
ImageBrush imageBrush = new ImageBrush();
ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
imageBrush.ImageSource = (ImageSource)imageSourceConverter.ConvertFrom(stream);
button.Background = imageBrush;

 * 
var imageSourceConverter = new ImageSourceConverter();
var img = (ImageSource)imageSourceConverter.ConvertFrom(imageStream);
 * 
var imagex=new System.Drawing.Bitmap(imageStream)
 * 
var img = new BitmapImage(new Uri(fileName, UriKind.RelativeOrAbsolute));
var widthPx = img.PixelWidth;
var heightPx = img.PixelHeight;
var horzRezDpi = img.DpiX;
var vertRezDpi = img.DpiY;
const int emusPerInch = 914400;
const int emusPerCm = 360000;
var widthEmus = (long)(widthPx / horzRezDpi * emusPerInch);
var heightEmus = (long)(heightPx / vertRezDpi * emusPerInch);
var maxWidthEmus = (long)(maxWidthCm * emusPerCm);
if (widthEmus > maxWidthEmus) {
  var ratio = (heightEmus * 1.0m) / widthEmus;
  widthEmus = maxWidthEmus;
  heightEmus = (long)(widthEmus * ratio);
}
 * 
 * 
Uri iconUri = new Uri("pack://application:,,,/Icon1.ico", UriKind.RelativeOrAbsolute);
this.Icon = BitmapFrame.Create(iconUri);


var img = new BitmapImage();
img.BeginInit();
img.StreamSource = imageStream;
img.EndInit();

WPF BitmapImage Width/Height are always 1?
http://stackoverflow.com/questions/237959/wpf-bitmapimage-width-height-are-always-1 
 * 
*/