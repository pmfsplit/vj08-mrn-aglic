using Akka.Actor;
using Akka.Configuration;
using Akka.Configuration.Hocon;
using Akka.Cluster.Tools.Client;
using Shared;
using System;
using System.Configuration;
using Akka.Cluster;
using Akka.Cluster.Tools.PublishSubscribe;

namespace MyCluster
{
    class WorkerActor : ReceiveActor
    {
        // Jos jedan nacin za dobit adresu sustava
        Address address = Cluster.Get(Context.System).SelfAddress;

        IActorRef mediator = DistributedPubSub.Get(Context.System).Mediator;
        public WorkerActor()
        {
            Console.WriteLine(address);

            mediator.Tell(new Subscribe("ClientService", Self));
            
            Receive<SubscribeAck>(x =>
            {
                Console.WriteLine($"Subscribed ack for topic {x.Subscribe.Topic}");
                Become(Subscribed);
            });
        }

        private void Subscribed()
        {
            Receive<Msg>(x =>
            {
                Console.WriteLine($"Got {x.Text} from {Sender}");
                Sender.Tell(new Msg($"Hello, my name is {Sender}"));
            });
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var port = args.Length > 0 ? int.Parse(args[0]) : 0;
            var section = (AkkaConfigurationSection)ConfigurationManager.GetSection("akka");

            var config = ConfigurationFactory
                .ParseString($"akka.remote.dot-netty.tcp.port={port}")
                .WithFallback(section.AkkaConfig);

            using (var system = ActorSystem.Create("Cluster", config))
            {
                Props props = Props.Create(() => new WorkerActor());
                // Uocite da je receptionist takodjer IActorRef
                var service = system.ActorOf(props, "service");
                //Retrieve the cluster receptionist extension
                var receptionist = ClusterClientReceptionist.Get(system);
                //Register the service with the cluster client
                receptionist.RegisterService(service);

                Console.ReadLine();
            }
        }
    }
}
