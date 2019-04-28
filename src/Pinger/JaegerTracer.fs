module JaegerTracer

open System
open System.Net.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Jaeger
open OpenTracing
open OpenTracing.Util
open OpenTracing.Contrib.NetCore.CoreFx

let ignoreJaeger = fun (options: HttpHandlerDiagnosticOptions) ->
    let endpoint = Environment.GetEnvironmentVariable("JAEGER_ENDPOINT")

    if String.length endpoint > 0 then
        options.IgnorePatterns.Add(fun (request: HttpRequestMessage) -> Uri(endpoint).IsBaseOf request.RequestUri)
    ()

let startup = fun (provider : IServiceProvider) ->
    let loggerFactory =  provider.GetRequiredService<ILoggerFactory>()
    let tracer = Configuration.FromEnv(loggerFactory).GetTracer()

    GlobalTracer.Register(tracer)
    tracer

type IServiceCollection with
    member __.AddJaeger() =
        __.AddSingleton<ITracer>(startup) |> ignore
        __.Configure<HttpHandlerDiagnosticOptions>(ignoreJaeger) |> ignore
        __
