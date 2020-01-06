namespace SharpApi.RestApi

open Giraffe
open Giraffe.Serialization
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Newtonsoft.Json
open SharpApi.RestApi.Serialization.Json
open SharpApi.RestApi.Serialization.Yaml
open System
open System.IO
open CommandLine;
open Microsoft.AspNetCore.Http
open SharpApi.Core

module RestApi =
  let configureServices (configuration: Configuration) (services : IServiceCollection) =
    let jsonSerializerSettings = JsonSerializerSettings()
    jsonSerializerSettings.Converters.Add(DocumentObjectConverter())

    let negotiationConfig = 
      let defaultNegotiationConfig = (DefaultNegotiationConfig() :> INegotiationConfig)
      { 
        new INegotiationConfig with
          member __.Rules = 
              dict [
                  "*/*"             , json
                  "application/json", json
                  //"application/xml" , xml
                  //"text/xml"        , xml
                  "text/plain"      , fun x -> x.ToString() |> text
              ]
          member __.UnacceptableHandler = defaultNegotiationConfig.UnacceptableHandler
      }

    services
      .AddGiraffe()
      .AddSingleton<IJsonSerializer>(NewtonsoftJsonSerializer(jsonSerializerSettings))
      .AddSingleton<INegotiationConfig>(negotiationConfig)
    |> ignore

  let readConfigurationFile path = 
    match File.Exists path with
    | false -> FatalError.TerminateApp FatalError.Config_File_Not_Found
    | _ -> try File.ReadAllText path with _ -> FatalError.TerminateApp FatalError.Cant_Read_Config_File 
  
  let deserializeConfiguration (configFile: string) = 
    try 
      ConfigurationSerializer.Deserialize configFile |> Ok
    with _ -> FatalError.TerminateApp FatalError.Config_File_Invalid

  let runApp httpPort configuration =
    let webApp = SharpApi.RestApi.HttpHandlersGenerator.Generate configuration
    let configureApp (app : IApplicationBuilder) =
        // Add Giraffe to the ASP.NET Core pipeline
        app.UseGiraffe webApp

    WebHostBuilder()
        //.UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "True") // This allows to suppress startup console message
        .UseKestrel()
        .UseUrls(sprintf "http://localhost:%i" httpPort)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices configuration)
        .Build()
        .Run()
    0

  [<EntryPoint>]
  let main args =
    let arguments = match Parser.Default.ParseArguments<CommandLineArguments>(args) with
                    | :? Parsed<CommandLineArguments> as arguments -> arguments.Value
                    | _ -> FatalError.TerminateApp FatalError.Invalid_Argument

    readConfigurationFile arguments.Config
    |> deserializeConfiguration
    |> Result.map (runApp arguments.HttpPort)
    |> Extensions.Result.Join
