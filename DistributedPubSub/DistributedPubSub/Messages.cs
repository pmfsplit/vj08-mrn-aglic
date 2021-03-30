namespace DistribPubSub
{
    public class Messages
    {
        public class Msg
        {
            public string Text { get; }

            public Msg(string text)
            {
                Text = text;
            }
        }
    }
}