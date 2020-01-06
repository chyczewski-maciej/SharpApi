namespace SharpApi.Core.Validation

open SharpApi.Core
open SharpApi.Core.Models
open System

type DocumentValidator (document: DocumentConfiguration) =
  let FieldTypeToDocumentProperty fieldType =
    match fieldType with
    | FieldType.Integer -> (DocumentProperty.Integer 0).GetType()
    | FieldType.String -> (DocumentProperty.String "").GetType()
    | FieldType.Boolean -> (DocumentProperty.Boolean true).GetType()
    | FieldType.Float -> (DocumentProperty.Float 0.0).GetType()

  let validProperties = new Map<string, Type> (document.Fields |> Array.map(fun field -> field.Name, FieldTypeToDocumentProperty field.Type))

  member __.Validate (documentObject:DocumentObject) =
    let ValidateField (field:Field) =
      let documentProperty = documentObject.Properties.[field.Name]
      documentProperty.GetType() = (FieldTypeToDocumentProperty field.Type)
      
    let validateAllRequiredFieldsArePresent() = Array.forall ValidateField document.Fields 

    let validateAllJTokenPropertiesAreLegal() =
      documentObject.Properties
      |> Map.forall (fun key _ -> validProperties.ContainsKey key)


    validateAllJTokenPropertiesAreLegal() && validateAllRequiredFieldsArePresent()