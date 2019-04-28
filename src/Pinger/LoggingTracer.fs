module LoggingTracer

open System
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Builder
open Serilog.Context


type LoggingTracerMiddleware(next: RequestDelegate, loggerFactory: ILoggerFactory) =
    do if isNull next then raise (ArgumentNullException("next"))
    
    let getIdOrElse (header: IHeaderDictionary, key: string, orElse: string) =
        match header.TryGetValue key with
            | true, value -> (value.ToString())
            | _ -> orElse

    member __.Invoke(ctx: HttpContext) =
        task {
            let defaultRequestID = Guid.NewGuid().ToString()
            let initialRID = getIdOrElse(ctx.Request.Headers, "x-original-request-id", defaultRequestID)
            let parentRID = getIdOrElse(ctx.Request.Headers, "x-request-id", defaultRequestID)
            let requestID = defaultRequestID

            LogContext.PushProperty("original-request-id", initialRID) |> ignore
            LogContext.PushProperty("parent-request-id", parentRID) |> ignore
            LogContext.PushProperty("request-id", requestID) |> ignore
            return next.Invoke ctx
        }

type IApplicationBuilder with
    member __.UseLoggingTracer() = 
        __.UseMiddleware<LoggingTracerMiddleware>()
