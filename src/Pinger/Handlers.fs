module Handlers

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.Extensions.Logging
open Giraffe

let pingHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let logger = ctx.GetLogger("pingHandler")
            logger.LogWarning("WARNING: DANGER!")

            return! text "pong" next ctx
        }

let webApp: HttpHandler =
    choose [
        route "/ping" >=> pingHandler
    ]
