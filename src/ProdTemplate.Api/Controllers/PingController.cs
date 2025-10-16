using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using ProdTemplate;
using ProdTemplate.Api.Services;
using ProdTemplate.Library;

namespace ProdTemplate.Api.Controllers;

[Controller]
[Route("ping")]
public class PingController(ProdTemplateActors actors) : ControllerBase
{
    private readonly IActorRef _pingActor = actors.PingActor;
    
    [HttpGet]
    public async Task<IActionResult> Ping([FromQuery] string message = "hello")
    {
        var result = await _pingActor.Ask<PongResponse>(PingMessage.NewPing(message));
        
        if (result is PongResponse.Pong pong)
            return Ok(pong.msg);
        
        return BadRequest("Unexpected response");
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var result = await _pingActor.Ask<PongResponse>(PingMessage.GetStats);
        
        if (result is PongResponse.Statistics stats)
            return Ok(new {
                totalPings = stats.stats.TotalPings,
                lastMessage = stats.stats.LastMessage,
                lastPingTime = stats.stats.LastPingTime
            });
        
        return BadRequest("Could not get stats");
    }
}