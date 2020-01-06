namespace SharpApi.RestApi

open Giraffe
open SharpApi.Core
open SharpApi.Core.Models
open SharpApi.Core.Validation
open SharpApi.RestApi.Databases
open System

module HttpHandlersGenerator = 

  let generateHttpHandlers (db:IDatabase) document = 
    let (idField: Field) = Array.find (fun field -> field.Name = document.Key) document.Fields

    let validateDocumentObject (nextHandlerFun: DocumentObject -> HttpHandler) documentObject : HttpHandler =
        match DocumentValidator(document).Validate documentObject with
        | true -> nextHandlerFun documentObject
        | false -> RequestErrors.BAD_REQUEST "Invalid object"

    let putActionHandler id (documentObject:DocumentObject): HttpHandler =
      let idFromTheBody = documentObject.Properties.[idField.Name]
      let idConflict = idFromTheBody <> id
      
      match (idConflict) with
      | true -> RequestErrors.BAD_REQUEST "Id from the url and id from body are different"
      | false -> match db.Update idFromTheBody documentObject with
                 | Some newValue -> negotiate newValue
                 | None -> setStatusCode 404

    let postActionHandler (documentObject: DocumentObject) : HttpHandler =
      let key = documentObject.Properties.[idField.Name]

      match db.Insert key documentObject with
      | Some newValue -> negotiate newValue
      | None -> RequestErrors.CONFLICT "Conflict"

    let getListActionHandler : HttpHandler = warbler(fun _ -> db.Get() |> negotiate)

    let getSingleHandler id =
        match db.Get id with
        | Some element -> negotiate element
        | None -> setStatusCode 404

    let deleteActionHandler id = 
      match db.Remove id with
      | Some element -> negotiate element
      | None -> setStatusCode 404

    let headActionHandler id =
      match db.Exists id with
      | true -> Successful.NO_CONTENT 
      | false -> setStatusCode 404

    let castIdToCorrectType (id:string) =
      match idField.Type with
      | FieldType.Boolean -> DocumentProperty.Boolean (bool.Parse(id))
      | FieldType.Integer -> DocumentProperty.Integer(Int32.Parse(id))
      | FieldType.Float -> DocumentProperty.Float (Double.Parse(id))
      | FieldType.String -> DocumentProperty.String id
    
    let generateActionHandler action =
      match action with
      | HttpAction.POST -> Giraffe.Core.POST >=> bindModel<DocumentObject> None (validateDocumentObject postActionHandler)
      | HttpAction.DELETE -> Giraffe.Core.DELETE >=> subRoutef "/%s" (castIdToCorrectType >> deleteActionHandler)
      | HttpAction.HEAD -> Giraffe.Core.HEAD >=> subRoutef "/%s" (castIdToCorrectType >> headActionHandler)
      | HttpAction.PUT -> Giraffe.Core.PUT >=> subRoutef "/%s" (castIdToCorrectType >> putActionHandler >> validateDocumentObject >> bindModel<DocumentObject> None)
      | HttpAction.GET -> 
        Giraffe.Core.GET >=> choose [ 
            subRoutef "/%s" (castIdToCorrectType >> getSingleHandler)
            getListActionHandler
          ]
      
    subRoute ("/" + document.Name) (choose [ yield! document.Actions |> Array.map generateActionHandler ])

  let Generate configuration =
    let database = 
      match configuration.Database with
      | Database.InMemory -> InMemoryDatabase()

    choose [
      yield! (configuration.Documents |> Array.map (fun document -> generateHttpHandlers database document))
    ]