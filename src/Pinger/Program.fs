module Pinger.App

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive
open Prometheus
open Giraffe

let configureApp (app : IApplicationBuilder) =
    // Add our metrics server and default http metrics handlers
    app.UseHttpMetrics() |> ignore
    app.UseMetricServer() |> ignore

    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe Handlers.webApp

let configureServices (services : IServiceCollection) =
    // Add Giraffe dependencies
    services.AddGiraffe() |> ignore

[<EntryPoint>]
let main _ =
    WebHostBuilder()
        .UseKestrel()
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .Build()
        .Run()
    0
