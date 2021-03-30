using System;
using AkkaConfigProvider;

namespace DistribPubSub
{
    class Program
    {
        static void Main(string[] args)
        {
            var configProvider = new ConfigProvider();
            var config = configProvider.GetAkkaConfig<MyAkkaConfig>();


            Console.WriteLine("Hello World!");
        }
    }
}