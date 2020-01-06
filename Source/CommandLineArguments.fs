namespace SharpApi.RestApi

open CommandLine;

type CommandLineArguments = {
  [<OptionAttribute('c', "config", Required = true, HelpText = "Path to the config file")>] Config: string
  [<OptionAttribute('p', "http-port", HelpText = "Http port", Default = 5000 )>] HttpPort: int
}