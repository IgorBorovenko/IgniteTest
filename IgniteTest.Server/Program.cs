using System;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using IgniteTestCommon;

namespace IgniteTestServer
{
    class Program
    {
        static void Main()
        {
            using (var ignite = Ignition.Start(new IgniteConfiguration
            {
                BinaryConfiguration = new BinaryConfiguration(typeof(Person)),
                DiscoverySpi = new TcpDiscoverySpi
                {
                    IpFinder = new TcpDiscoveryStaticIpFinder
                    {
                        Endpoints = new[]
                        {
                            "ignite01.prod.stat.bi.3shape.local:47500..47509",
                            "ignite02.prod.stat.bi.3shape.local:47500..47509",
                            "ignite03.prod.stat.bi.3shape.local:47500..47509",
                            "ignite04.prod.stat.bi.3shape.local:47500..47509",
                        }
                    },
                    SocketTimeout = TimeSpan.FromSeconds(0.3)
                },
                IgniteInstanceName = "my cluster",
                ClientMode = false,
            }))
            {
                //Keep working...
                Console.ReadLine();
            }
        }
    }
}
