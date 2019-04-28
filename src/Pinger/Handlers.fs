module Handlers

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive
open Prometheus
open Giraffe

let pingCounter = Metrics.CreateCounter("pinger_ping_total", "The total number of pings", CounterConfiguration())

let pingHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            pingCounter.Inc() |> ignore

            return! text "pong" next ctx
        }

let webApp: HttpHandler =
    choose [
        route "/ping" >=> pingHandler
    ]
