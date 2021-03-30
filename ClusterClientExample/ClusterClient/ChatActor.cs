using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Tools.Client;
using Shared;
using System;

namespace MyClusterClient
{
    public class ChatActor : ReceiveActor
    {
        // Jos jedan nacin za dobit adresu sustava
        Address address = Cluster.Get(Context.System).SelfAddress;

        IActorRef _clusterClient;
        public ChatActor(Props clusterClientProps)
        {
            Console.WriteLine(address);
            //Deploy the cluster client into the local actor system
            _clusterClient = Context.ActorOf(clusterClientProps, "clusterClient");
            // u ovom trenutku sa klasterom možemo komunicirati na isti način kao i kod
            // distributed publish-subscribe


            Receive<Init>(x =>
                _clusterClient.Tell(new ClusterClient.Send("/user/service", new Msg($"Seeeeend from {Self}")))
            //_clusterClient.Tell(new ClusterClient.SendToAll("/user/service", new Msg($"SeeeeendToAllllllll from {Self}")))
            );
            Receive<InitPub>(x =>
                _clusterClient.Tell(new ClusterClient.Publish("ClientService", new Msg("This was published")))
            );
            Receive<Msg>(x => Console.WriteLine($"{Self} with {address} Got: {x.Text}"));
        }
    }
}
