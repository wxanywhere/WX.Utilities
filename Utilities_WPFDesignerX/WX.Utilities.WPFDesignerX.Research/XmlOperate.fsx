
#r "System.Xml"
#r "System.Xml.Linq"
open System
open System.Text
open System.Xml
open System.Xml.Linq


let AnnotationDataXmlPath= @"D:\Temp\Testing\AnnotationData.xml"
let ns="http://schemas.ydtf.com/2013"
let prefix="m"

let xw= new XmlTextWriter(AnnotationDataXmlPath , Encoding.UTF8) //or use vx=XmlWriter.Create(this.D_FilePath)
xw.Formatting<-Formatting.Indented
xw.WriteStartDocument()
xw.WriteStartElement(prefix,"AnnotationData",ns)
xw.WriteStartElement(prefix,"Option",ns)
xw.WriteAttributeString(prefix,"IsDisplayAnnotation",ns,"False")
xw.WriteAttributeString(prefix,"IsDisplayController",ns,"False")
xw.WriteAttributeString(prefix,"IsDesignAudited",ns,"False")
xw.WriteAttributeString(prefix,"DocumentTitle",ns,String.Empty)
xw.WriteEndElement()
xw.WriteStartElement (prefix,"ServiceInfos",ns)
xw.WriteEndElement()
xw.WriteStartElement (prefix,"Annotations",ns)
xw.WriteEndElement()
xw.WriteEndElement()
xw.WriteEndDocument()
xw.Close()


let doc=XDocument.Load(AnnotationDataXmlPath)
let xa=doc.Descendants(XName.Get("Annotations",ns))
let xb=
  match xa|>Seq.tryFind (fun _ ->true) with
  | Some x ->x
  | _ ->null

xb.Attribute (XName.Get("www",ns))

xb.Elements(XName.Get("wx1111",ns))
|>Seq.length

doc.Descendants(XName.Get("Annotationsxxxxx",ns))

let xx:string []=[||]
xx|>Seq.tryFind (fun _ ->true)


for n in xa.Elements() do
  printf "%A" n.Name

//Add
let xe=new XElement(XName.Get("Annotation",ns))
xe.SetAttributeValue(XName.Get ("UIElementID",ns),String.Empty)
xe.SetAttributeValue(XName.Get ("UIElementName",ns),String.Empty)
xe.SetAttributeValue(XName.Get ("UITypeName",ns),String.Empty)
xe.SetAttributeValue(XName.Get ("UIName",ns),String.Empty)
xe.SetAttributeValue(XName.Get ("IsInTabItem",ns),"False")
xe.SetAttributeValue(XName.Get ("IsTabItem",ns),"False")
xe.SetAttributeValue(XName.Get ("TabControlNumber",ns),"0")
xe.SetAttributeValue(XName.Get ("TabItemNumber",ns),"0")
xe.SetAttributeValue(XName.Get ("TabItemHeader",ns),String.Empty)
xe.SetAttributeValue(XName.Get ("RequirementDescription",ns),String.Empty)
xe.SetAttributeValue(XName.Get ("BehaviorDescription",ns),String.Empty)
let xea=new XElement(XName.Get("Services",ns))
xe.AddFirst xea
for n in 1..3 do
  let xec=new XElement(XName.Get("Service",ns))
  xec.SetAttributeValue(XName.Get ("ServiceName",ns),String.Empty)
  xea.AddFirst xec
xb.AddFirst(xe)
doc.Save AnnotationDataXmlPath

//Query
xb.Attributes()|>Seq.length

xb.Attribute(XName.Get("UIElementID",ns)).Value
xb.Attribute(XName.Get("UIElementID")).Value
xb.Name
xb.GetNamespaceOfPrefix("m")
xb.GetPrefixOfNamespace(XNamespace.Get(ns))

//Query
xb.Elements(XName.Get("Annotations",ns))
|>Seq.filter (fun a->a.Attribute(XName.Get("UIElementID",ns)).Value="" )
|>Seq.length

//Delete
match xb.Elements(XName.Get("Annotations",ns))|>Seq.tryFind (fun _ ->true) with
| Some x ->
    x.Remove()
    //x.Save AnnotationDataXmlPath
|_ ->()
doc.Save AnnotationDataXmlPath

//Modify
match xb.Elements(XName.Get("Annotations",ns))|>Seq.tryFind (fun _ ->true) with
| Some x ->
    x.SetAttributeValue(XName.Get("UIElementID",ns),Guid.NewGuid())
    //x.SetElementValue(XName.Get("Service",ns),Guid.NewGuid())
    if x.HasElements then 
        x.RemoveNodes()
        //x.Elements().Remove()
        //x.AddFirst
    //x.Save AnnotationDataXmlPath
|_ ->()
doc.Save AnnotationDataXmlPath


