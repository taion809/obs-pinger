module Pinger.App

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Serilog
open Serilog.Events
open Serilog.Formatting.Compact
open LoggingTracer
open Giraffe

let configureApp (app : IApplicationBuilder) =
    // Add our middleware to push tracing information into our logging context
    app.UseLoggingTracer() |> ignore

    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe Handlers.webApp

let configureServices (services : IServiceCollection) =
    // Add Giraffe dependencies
    services.AddGiraffe() |> ignore

let initLogger (builder: ILoggingBuilder) =
    Log.Logger <- LoggerConfiguration()
                        .MinimumLevel.Warning()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .WriteTo.Console(CompactJsonFormatter())
                        .CreateLogger()

    builder.AddSerilog(Log.Logger, true) |> ignore

[<EntryPoint>]
let main _ =
    WebHostBuilder()
        .UseKestrel()
        .UseSerilog()
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(initLogger)
        .Build()
        .Run()

    Log.CloseAndFlush()
    0
