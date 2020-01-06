namespace SharpApi.RestApi.Extensions

module Result = 
  let IfError (f: 'TError -> 'T) (result:Result<'T, 'TError>)  =
    match result with
    | Error e -> f e 
    | Ok o -> o
  let Match (f: 'T -> 'TResult) (g: 'TError -> 'TResult) (result: Result<'T, 'TError>) = 
    match result with 
    | Ok o -> f o
    | Error e -> g e
  let Join (result:Result<'T, 'T>) =
    match result with
    | Ok o -> o
    | Error e -> e