using Akka.Actor;
using ProdTemplate.Library;

namespace ProdTemplate.Api.Services;

public class ProdTemplateActors(ActorSystem actorSystem, ILogger<ProdTemplateActors> logger)
{
    private readonly Lazy<IActorRef> _pingActorLazy = new(() => 
        Library.PingActor.create(actorSystem, "ping-actor", new ActorEnv(logger)));
    
    public IActorRef PingActor => _pingActorLazy.Value;
}