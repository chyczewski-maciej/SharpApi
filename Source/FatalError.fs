namespace SharpApi.RestApi

type FatalError = 
  | Invalid_Argument = 0
  | Config_File_Not_Found = 1
  | Cant_Read_Config_File = 2
  | Config_File_Invalid = 3
  | Unknown_Error = 999

module FatalError =
  let TerminateApp fatalError = 
    let exitMessage = match fatalError with
                      | FatalError.Invalid_Argument -> "Invalid arguments"
                      | FatalError.Config_File_Not_Found -> "Configuration file not found" 
                      | FatalError.Cant_Read_Config_File -> "Can't read configuration file"
                      | FatalError.Config_File_Invalid -> "Configuration file is invalid"
                      | _ -> "Unknown fatal error"
    printfn "Fatal error: %s" exitMessage
    exit (LanguagePrimitives.EnumToValue fatalError)