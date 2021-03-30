using Akka.Actor;
using Akka.Cluster.Tools.Client;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClusterClient
{
    public class Init { }
    public class InitPub { }
    class Program
    {
        static void Main(string[] args)
        {
            var initialContacts = ImmutableHashSet<ActorPath>.Empty
                .Add(ActorPath.Parse("akka.tcp://Cluster@localhost:12000/system/receptionist"));
            //.Add(ActorPath.Parse("akka.tcp://ClusterClientExample@localhost:12001/user/service"));

            using (var system = ActorSystem.Create("ClusterClient"))
            {
                var clusterClientSettings = ClusterClientSettings.Create(system)
                    .WithInitialContacts(initialContacts);
                // Creaste the Props used to deploy the cluster client on the local actor system

                var clusterClientProps = ClusterClient.Props(clusterClientSettings);
                //Deploy the cluster client into the local actor system
                //Ovaj dio je prebacen u actora kako bi vidjeli da mozemo iz bilo kojeg actora pristupiti nekom clusteru,
                // a ne samo iz actora stvorenih iz system.ActorOf
                //var clusterClient = system.ActorOf(clusterClientProps, "clusterClient");

                //clusterClient.Tell(new ClusterClient.Send("/user/service", new Msg($"Hello from cluster client")));

                var props = Props.Create(() => new ChatActor(clusterClientProps));
                var actor = system.ActorOf(props);

                Console.ReadLine();
                actor.Tell(new Init());

                Console.ReadLine();
                actor.Tell(new InitPub());

                Console.ReadLine();
            }
        }
    }
}
