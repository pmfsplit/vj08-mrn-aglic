using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Configuration;
using AkkaConfigProvider;

namespace DistribPubSub
{
    class Program
    {
        static void Main(string[] args)
        {
            var configProvider = new ConfigProvider();
            var config = configProvider.GetAkkaConfig<MyAkkaConfig>();

            var ports = new[] {12000, 12001};

            List<DistributedPubSub> mediators = new List<DistributedPubSub>();
            
            // Pokreniti ćemo 4 čvora unutar ovog jednog projekta da vidimo da se i to može.
            // Važno je da kreiramo 4 actor sustava
            foreach (var el in Enumerable.Range(0, 4))
            {
                var port = el > ports.Length ? 0 : ports[el];
                var akkaConfig = ConfigurationFactory
                    .ParseString($"akka.remote.dot-netty.tcp.port={port}")
                    .WithFallback(config);
                var system = ActorSystem.Create("DisPubSubExample", config);
                var props = Props.Create(() => new ChatActor());
                system.ActorOf(props);
                
                DistributedPubSub mediator = DistributedPubSub.Get(system);
                
                mediators.Add(mediator);
            }

            string text = String.Empty;

            do
            {
                var cmd = Console.ReadLine();
                // cmd should be, e.g. general hello 1 -> 
                // which means that topic is "general", text is "hello",
                // 1 is the index that will be used to get a mediator from the list
                // called mediators
                var parsed = cmd.Split(' ');
                
                var topic = parsed[0];
                text = parsed[1];
                var index = int.Parse(parsed[2]);

                if(text.ToLower() != "q")
                {
                }
            } while (text.ToLower() != "q");
        }
    }
}