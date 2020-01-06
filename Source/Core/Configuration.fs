namespace SharpApi.Core

open System

type Database = 
  | InMemory
  
type FieldType = 
  | String 
  | Integer 
  | Boolean 
  | Float 

type HttpAction = 
  | DELETE 
  | GET 
  | HEAD 
  | POST 
  | PUT 

[<CLIMutableAttribute>]
type Field = { Name: String; Type: FieldType;  }  

[<CLIMutableAttribute>]
type DocumentConfiguration = { Name: String; Actions: HttpAction[]; Fields: Field[]; Key: String }

[<CLIMutableAttribute>]
type Configuration = {
    Database: Database
    Documents: DocumentConfiguration[] 
  }