namespace SharpApi.RestApi.Serialization.Json

open Newtonsoft.Json
open Newtonsoft.Json.Linq
open SharpApi.Core.Models

type DocumentObjectConverter() =
  inherit JsonConverter<DocumentObject>()
  override this.WriteJson(writer: JsonWriter, value: DocumentObject, serializer: JsonSerializer): Unit =
    let DocumentPropertyToJToken (documentProperty: DocumentProperty) : JToken = 
      match documentProperty with 
      | String s -> JToken.FromObject s
      | Integer i -> JToken.FromObject i
      | Boolean b -> JToken.FromObject b
      | Float f -> JToken.FromObject f           

    let obj = JObject()
    value.Properties
    |> Map.iter (fun key property -> obj.Add(key, (DocumentPropertyToJToken property)))
    serializer.Serialize(writer, obj)

  override this.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer) =
    let rec JTokenToDocumentProperty (jToken:JToken): DocumentProperty =
      match jToken.Type with
      | JTokenType.String -> DocumentProperty.String (jToken.Value<string>())
      | JTokenType.Integer -> DocumentProperty.Integer (jToken.Value<int>())
      | JTokenType.Boolean -> DocumentProperty.Boolean (jToken.Value<bool>())
      | JTokenType.Float -> DocumentProperty.Float(jToken.Value<float>())
      | JTokenType.Property -> JTokenToDocumentProperty (Seq.exactlyOne (jToken.Children() :> seq<JToken>))
      | x -> failwith (sprintf "not supported type %s" (x.ToString()))

    let y = (serializer.Deserialize(reader) :?> JObject)
    let p = y.Properties()
            |> Seq.map(fun j -> (j.Name, (JTokenToDocumentProperty j)))
            |> Map
            
    { Properties = p }