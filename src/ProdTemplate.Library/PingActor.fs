namespace ProdTemplate.Library

open Akka.Actor
open Akka.FSharp
open Microsoft.Extensions.Logging

type PingStats = {
    TotalPings: int
    LastMessage: string
    LastPingTime: System.DateTime
}

type PingMessage = 
    | Ping of msg: string
    | GetStats

type PongResponse =
    | Pong of msg: string
    | Statistics of stats: PingStats
    
module PingActor = 
    let pingActor (env: ActorEnv) (mailbox: Actor<_>) =
        let rec loop (stats: PingStats) = actor {
            let! message = mailbox.Receive()
            
            match message with
            | Ping msg ->
                env.Logger.LogInformation <| $"Received: {msg}"
                let newStats = {
                    TotalPings = stats.TotalPings + 1
                    LastMessage = msg
                    LastPingTime = System.DateTime.UtcNow
                }
                mailbox.Sender() <! Pong $"Pong: {msg}"
                return! loop newStats
                
            | GetStats ->
                mailbox.Sender() <! Statistics stats
                return! loop stats
        }
        
        let initialStats = {
            TotalPings = 0
            LastMessage = ""
            LastPingTime = System.DateTime.MinValue
        }
        
        loop initialStats
        
    let create (system: ActorSystem) (name: string) (env: ActorEnv)=
        spawn system name (pingActor env)