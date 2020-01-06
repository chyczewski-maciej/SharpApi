namespace SharpApi.RestApi.Serialization.Xml

open System.Xml
open Giraffe.Serialization
open System.Text
open System.IO
open System.Xml.Serialization
open System
open SharpApi.Core.Models

type IXmlConverter =
  interface
    abstract member CanConvert : Type -> bool
    abstract member Serialize : obj -> byte array
    abstract member Deserialize : string -> obj
  end

[<AbstractClass>]
type XmlConverter<'T>() =
  abstract member CanConvert : Type -> bool
  abstract member Serialize : 'T -> byte array
  abstract member Deserialize<'T> : string -> 'T
  
  default this.CanConvert t = (t = typeof<'T>)
  
  interface IXmlConverter with
    member __.CanConvert t = (t = typeof<'T>)
    member __.Serialize o = __.Serialize(o :?> 'T)
    member __.Deserialize s = (__.Deserialize<'T> s) :> obj
  end


type DocumentObjectXmlConverter() =
  inherit XmlConverter<DocumentObject>()
  
  override this.Serialize documentObject = 
    use stream = new MemoryStream()
    use writer = XmlWriter.Create(stream)
    writer.WriteStartElement(typeof<DocumentObject>.Name)

    documentObject.Properties
    |> Map.iter(fun key value -> 
      writer.WriteStartElement(key)
      writer.WriteString(value.ToString())
      writer.WriteEndElement()
    )

    writer.WriteEndElement()
    writer.Flush()

    //let serializer = XmlSerializer(o.GetType())
    //serializer.Serialize(writer, o)
    stream.ToArray()

  override this.Deserialize str = { Properties = Map.empty }

type ExtendableXmlSerializer (settings : XmlWriterSettings, converters: IXmlConverter list) =
  static member DefaultSettings =
    XmlWriterSettings(
      Encoding           = Encoding.UTF8,
      Indent             = true,
      OmitXmlDeclaration = false
    )

  interface IXmlSerializer with
    member __.Serialize (o : obj) =
      converters 
      |> List.filter (fun c -> c.CanConvert (o.GetType()))
      |> List.head
      |> (fun c -> c.Serialize(o))

      //use stream = new MemoryStream()
      //use writer = XmlWriter.Create(stream, settings)
      //let serializer = XmlSerializer(o.GetType())
      //serializer.Serialize(writer, o)
      //stream.ToArray()

    member __.Deserialize<'T> (xml : string) =
      let serializer = XmlSerializer(typeof<'T>)
      use reader = new StringReader(xml)
      serializer.Deserialize reader :?> 'T
