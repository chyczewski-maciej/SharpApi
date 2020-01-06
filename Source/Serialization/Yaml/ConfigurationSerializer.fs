namespace SharpApi.RestApi.Serialization.Yaml

  module ConfigurationSerializer =
    open System
    open YamlDotNet.Serialization
    open SharpApi.Core

    let (Serialize: Configuration -> String) =
      let serializer = 
        SerializerBuilder()
          .WithTypeConverter(SimpleDiscriminatedUnionConverter<Database>())
          .WithTypeConverter(SimpleDiscriminatedUnionConverter<FieldType>())
          .WithTypeConverter(SimpleDiscriminatedUnionConverter<HttpAction>())
          .Build()
      fun configuration -> serializer.Serialize (configuration :> Object)

    let (Deserialize: String -> Configuration) = 
      let deserializer = 
        DeserializerBuilder()
          .WithTypeConverter(SimpleDiscriminatedUnionConverter<Database>())
          .WithTypeConverter(SimpleDiscriminatedUnionConverter<FieldType>())
          .WithTypeConverter(SimpleDiscriminatedUnionConverter<HttpAction>())          
          .Build()
      fun ``string`` -> deserializer.Deserialize<Configuration> ``string``