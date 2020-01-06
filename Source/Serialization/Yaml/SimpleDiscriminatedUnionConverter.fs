namespace SharpApi.RestApi.Serialization.Yaml

open System
open YamlDotNet.Core.Events
open YamlDotNet.Serialization
open Microsoft.FSharp.Reflection

  /// Discriminated Union Converter for YamlDotNet serializer library  
  ///
  /// WARNING: It supports only cases without any values
  ///
  /// Inspired by:
  /// https://stackoverflow.com/questions/21559497/create-discriminated-union-case-from-string
  /// https://github.com/aaubry/YamlDotNet/blob/5ec2dd75704d6dced41c0d49ff5528f3797d1068/YamlDotNet/Serialization/Converters/DateTimeConverter.cs
  type SimpleDiscriminatedUnionConverter<'T>() =
    member private this.values = 
        FSharpType.GetUnionCases typeof<'T>
        |> Array.map (fun case -> case.Name, (FSharpValue.MakeUnion (case, [||]) :?> 'T))
        |> dict 

    interface IYamlTypeConverter with 
      member this.Accepts ``type`` = 
        ``type`` = typeof<'T>
      member this.ReadYaml (parser, ``type``) =
        let (valueAsString: String) = (parser.Current :?> Scalar).Value        
        let (found, value) = this.values.TryGetValue(valueAsString)
        if(not found) then raise (InvalidValueException (typeof<'T>, valueAsString))

        parser.MoveNext() |> ignore
        (value :> obj)

      member this.WriteYaml(emitter, value, ``type``) = 
          match FSharpValue.GetUnionFields(value, typeof<'T>) with
          | case, _ -> emitter.Emit(Scalar(case.Name))