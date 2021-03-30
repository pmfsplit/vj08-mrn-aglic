using System;
using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Tools.PublishSubscribe;

namespace DistribPubSub
{
    public class ChatActor : ReceiveActor
    {
        DistributedPubSub mediator = DistributedPubSub.Get(Context.System);

        public ChatActor()
        {
            Receive<SubscribeAck>(x => {
                Console.WriteLine($"Subscribed ack for topic {x.Subscribe.Topic}");
                Become(Subscribed);
            });
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

        private int GetPort()
        {
            // DA imate nacin kako unutar lokalnog sustava dobiti port i ip
            Address address = ((ClusterActorRefProvider)((ExtendedActorSystem)Context.System).Provider).Transport.DefaultAddress;
            return address.Port.Value;
        }
        
        private void Subscribed()
        {
            Receive<Messages.Msg>(x => HandleMsg(x));
        }

        private void HandleMsg(Messages.Msg msg)
        {
            Console.WriteLine($"{Self} (with port: {GetPort()}) got {msg.Text} from {Sender}");
        }
    }
}