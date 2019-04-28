module Handlers

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe

let pingHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            return! text "pong" next ctx
        }

let webApp: HttpHandler =
    choose [
        route "/ping" >=> pingHandler
    ]
