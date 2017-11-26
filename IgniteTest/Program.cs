using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Apache.Ignite.Linq;
using IgniteTestCommon;

namespace IgniteTest
{
    class Program
    {
        private static List<Person> _personsList = new List<Person>();
        static void Main()
        {
            Environment.SetEnvironmentVariable("IGNITE_H2_DEBUG_CONSOLE", "true");

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
                ClientMode = true,
            }))
            {
                var cacheConfiguration = new CacheConfiguration("persons", typeof(Person));
                cacheConfiguration.CacheMode = CacheMode.Partitioned;
                cacheConfiguration.WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync;

                var cache = ignite.GetOrCreateCache<int, Person>(cacheConfiguration);
                cache.Clear();

                if (!cache.Any())
                    UploadData(cache);

                //return;
                Console.WriteLine("Init complete. 'Enter' to start tests.");
                Console.ReadLine();

                TestPerformance(cache);
                Console.ReadLine();
            }
        }

        private static void TestPerformance(ICache<int, Person> cache)
        {
            var persons = cache.AsCacheQueryable();
            var igniteQuery = CompiledQuery.Compile(() => persons.Where(x => x.Value.Name.Contains("ey"))
                .GroupBy(x => x.Value.Name)
                .Select(g => new Tuple<string, double>(g.Key, g.Min(x => x.Value.SalaryReceived))));

            for (var j = 0; j < 10; j++)
            {
                var sw = Stopwatch.StartNew();
                
                if (_personsList.Any())
                {
                    var avgSalaryLinq = _personsList.Where(x => x.Name.Contains("ey"))
                        .GroupBy(x => x.Name)
                        .Select(g => new Tuple<string, double>(g.Key, g.Min(x => x.SalaryReceived)))
                        .ToList();
                    Console.WriteLine($"LINQ calculated in {sw.Elapsed}");
                }
                
                sw.Restart();
                var avgSalaryIgnite = igniteQuery().GetAll();
                Console.WriteLine($"Ignite calculated in {sw.Elapsed}");
                Console.WriteLine($"persons.Count == {persons.Count().ToString("#,#", CultureInfo.InvariantCulture)}");

                Thread.Sleep(5000);
            }
        }

        private static void UploadData(ICache<int, Person> cache)
        {
            using (var streamer = cache.Ignite.GetDataStreamer<int, Person>("persons"))
            {
                Console.WriteLine("Uploading data...");
                var i = 0;
                while (i < 10000000)
                {
                    var person = new Person { Name = "Andrey K", SalaryReceived = new Random().Next(10000) };
                    //cache.PutIfAbsent(i, person);
                    streamer.AddData(i, person);
                    _personsList.Add(person);
                    i++;

                    person = new Person { Name = "Sergey R", SalaryReceived = new Random().Next(1000) };
                    //cache.PutIfAbsent(i, person);
                    streamer.AddData(i, person);
                    _personsList.Add(person);
                    i++;

                    person = new Person { Name = "Oleg K", SalaryReceived = new Random().Next(1000) };
                    //cache.PutIfAbsent(i, person);
                    streamer.AddData(i, person);
                    _personsList.Add(person);
                    i++;

                    person = new Person { Name = "Igor B", SalaryReceived = new Random().Next(1000) };
                    //cache.PutIfAbsent(i, person);
                    streamer.AddData(i, person);
                    _personsList.Add(person);
                    i++;

                    person = new Person { Name = "Alexey M", SalaryReceived = new Random().Next(1000) };
                    //cache.PutIfAbsent(i, person);
                    streamer.AddData(i, person);
                    _personsList.Add(person);
                    i++;

                    person = new Person { Name = "Igor P", SalaryReceived = new Random().Next(1000) };
                    //cache.PutIfAbsent(i, person);
                    streamer.AddData(i, person);
                    _personsList.Add(person);
                    i++;

                    person = new Person { Name = "Alexey Z", SalaryReceived = new Random().Next(100) };
                    //cache.PutIfAbsent(i, person);
                    streamer.AddData(i, person);
                    _personsList.Add(person);
                    i++;

                    person = new Person { Name = "Marianne B", SalaryReceived = new Random().Next(100000) };
                    //cache.PutIfAbsent(i, person);
                    streamer.AddData(i, person);
                    _personsList.Add(person);
                    i++;

                    person = new Person { Name = "Nikolaj D", SalaryReceived = new Random().Next(1000000) };
                    //cache.PutIfAbsent(i, person);
                    streamer.AddData(i, person);
                    _personsList.Add(person);
                    i++;

                    person = new Person { Name = "Tais C", SalaryReceived = new Random().Next(1000000) };
                    //cache.PutIfAbsent(i, person);
                    streamer.AddData(i, person);
                    _personsList.Add(person);
                    i++;

                    if (i % 1000000 == 0)
                        Console.WriteLine($"{i.ToString("#,#", CultureInfo.InvariantCulture)}...");
                }
            }
        }
    }
}
