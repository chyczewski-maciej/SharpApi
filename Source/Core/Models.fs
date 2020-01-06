namespace SharpApi.Core.Models

type DocumentProperty =
  | String of string
  | Integer of int
  | Boolean of bool 
  | Float of float with
  
  override this.ToString() =
    match this with
    | String s -> s
    | Integer i -> i.ToString()
    | Boolean b -> b.ToString()
    | Float f -> f.ToString()
  
  member this.GetUnderlyingType() = 
    match this with
    | String _ -> typeof<string>
    | Integer _ -> typeof<int>
    | Boolean _ -> typeof<bool>
    | Float _ -> typeof<float>

[<CLIMutable>]
type DocumentObject = {
    Properties: Map<string, DocumentProperty> 
  }