module Handlers

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive
open OpenTracing
open Giraffe

let traceHandler operation =
    fun (next: HttpFunc) (ctx: HttpContext) ->
    task {
        let tracer = ctx.GetService<ITracer>()
        use _ = tracer.BuildSpan(operation).StartActive(true)
        return! next ctx
    }

let pingHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            return! text "pong" next ctx
        }

let webApp: HttpHandler =
    choose [
        route "/ping" >=> traceHandler("pingHandler") >=> pingHandler
    ]
