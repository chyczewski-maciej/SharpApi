namespace SharpApi.RestApi.Databases

open Newtonsoft.Json
open SharpApi.Core.Models
open System.IO

type FileSystemDatabase(path: string, keyToString: DocumentProperty -> string) =
  let prepend = path + "\\"
  let append = ".json"
  let getPath key = sprintf "%s%s%s" prepend (keyToString key) append
  do
    Directory.CreateDirectory path |> ignore

  member __.Get() =
    Directory.GetFiles path
    |> Array.map (File.ReadAllText >> JsonConvert.DeserializeObject<DocumentObject>)
    |> Array.toList

  member __.Get key: DocumentObject option =
    if File.Exists (getPath key) then
      File.ReadAllText (getPath key)
      |> JsonConvert.DeserializeObject<DocumentObject>
      |> Some
    else
      None

  member __.Insert (key:DocumentProperty) (value:DocumentObject) : DocumentObject option =
    match File.Exists (getPath key) with
    | true -> None
    | false ->    
      try
        File.WriteAllText((getPath key), (JsonConvert.SerializeObject value))
        Some value
      with
      | _ -> None

  member __.Remove key: DocumentObject option =
    match File.Exists (getPath key) with
    | false -> None 
    | true -> 
      let p = getPath key
      let v = (File.ReadAllText >> JsonConvert.DeserializeObject<DocumentObject> >> Some) p
      File.Delete p
      v
      
  member __.Exists key = File.Exists (getPath key)

  member __.Update key value: DocumentObject option =
    match File.Exists (getPath key) with
    | false -> None
    | true -> 
      File.WriteAllText((getPath key), (JsonConvert.SerializeObject value))
      Some value

  interface IDatabase with
    member this.Insert key value = this.Insert key value
    member this.Get() = this.Get()
    member this.Get key = this.Get key
    member this.Remove key = this.Remove key
    member this.Update key value = this.Update key value
    member this.Exists key = this.Exists key