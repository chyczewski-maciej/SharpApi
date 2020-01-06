namespace SharpApi.RestApi.Databases

open System.Collections.Concurrent
open System.Collections.Generic
open SharpApi.Core.Models

type InMemoryDatabase(comparer:IEqualityComparer<DocumentProperty>) =
  let db = ConcurrentDictionary<DocumentProperty, DocumentObject>(comparer)
  public new() = InMemoryDatabase(EqualityComparer.Default)

  member __.Get() =
    db 
    |> Seq.map (fun kvp -> kvp.Value)
    |> Seq.toList

  member __.Get key: DocumentObject option =
    match db.TryGetValue(key) with
    | true, value -> Some value
    | _ -> None

  member __.Insert key value: DocumentObject option =
    match db.TryAdd(key, value) with
    | true -> Some value 
    | _ -> None

  member __.Remove key: DocumentObject option =
    match db.TryRemove(key) with
    | true, value -> Some value
    | _ -> None

  member __.Update key value: DocumentObject option =
    __.Get key
    |> Option.bind (fun currentValue -> if db.TryUpdate(key, value, currentValue) then Some value else None)

  member __.Exists key = db.ContainsKey key

  interface IDatabase with
    member this.Insert key value = this.Insert key value
    member this.Get() = this.Get()
    member this.Get key = this.Get key
    member this.Remove key = this.Remove key
    member this.Update key value = this.Update key value
    member this.Exists key = this.Exists key