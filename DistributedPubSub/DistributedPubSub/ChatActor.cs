using System;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;

namespace DistribPubSub
{
    public class ChatActor : ReceiveActor
    {
        DistributedPubSub mediator = DistributedPubSub.Get(Context.System);

        public ChatActor()
        {
            // Receive<>
        }
        
        protected override void PreStart()
        {
            mediator.Mediator.Tell(new Subscribe("general", Self));

            var br = Program.Rnd.Next(0, 2);
            if (br == 1)
            {
                Console.WriteLine("HELLO................................");
                mediator.Mediator.Tell(new Subscribe("specific", Self));
            }
            base.PreStart();
        }
        
        
    }
}