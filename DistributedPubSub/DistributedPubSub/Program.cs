using System;
using System.Linq;
using Akka.Actor;
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
            }

            Console.WriteLine("Hello World!");
        }
    }
}