using System.IO;
using System;
using System.Collections.Generic;
using common;
using System.Net;
namespace test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                test_udp_interact("127.0.0.1", 1234);
                //test_udp();
                //test_tcp();
                // test_list();
                // test_rand();
                // test_datetime();
                // test_str();
                // test_file();
                // test_sort();
                // test_async().Wait();
                // Util.P("ok");
            }
            else
            {

                var command = args[0];

                if (command == "udpclient" && args.Length >= 3)
                {
                    //udpclient 127.0.0.1 8888
                    test_udp_interact(args[1], int.Parse(args[2]));
                }
                else if (command == "udpserver" && args.Length > 1)
                {
                    //udpserver 8888
                    test_udp_server(int.Parse(args[1]));
                    System.Threading.Thread.Sleep(int.MaxValue);
                }
            }
        }

        static void test_udp_interact(string remoteIp, int remotePort)
        {
            Util.Print("remote host is ", remoteIp, ":", remotePort);
            Util.Print("input your message:");

            var client = new UDPSocket();
            var remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteIp), remotePort);
            client.OnReceive = delegate (EndPoint endPoint, byte[] data, int len)
            {
                Util.Print("[RECV]", endPoint.ToString(), ":", System.Text.Encoding.UTF8.GetString(data, 0, len));
            };
            while (true)
            {
                var input = Console.ReadLine();
                if (input == "exit")
                {
                    break;
                }
                var buffer = input.ToBytes();
                client.Send(buffer, buffer.Length, remoteEndPoint);
            }
        }

        static void test_udp()
        {
            int port = 8888;
            test_udp_server(port);
            test_udp_client(port);

            System.Threading.Thread.Sleep(int.MaxValue);
        }
        static void test_udp_server(int port)
        {
            Util.Print("udp server open on:", port);
            var server = new UDPSocket(port);
            server.OnReceive = delegate (EndPoint endPoint, byte[] data, int len)
            {
                server.Send(data, len, endPoint);
                Util.Print("server recv from ", endPoint.ToString(), ":", System.Text.Encoding.UTF8.GetString(data, 0, len));
            };
        }
        static void test_udp_client(int port)
        {
            var client = new UDPSocket();
            client.OnReceive = delegate (EndPoint endPoint, byte[] data, int len)
            {
                Util.Print("client recv from ", endPoint.ToString(), ":", System.Text.Encoding.UTF8.GetString(data, 0, len));
            };
            var sep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            for (int i = 0; i < 1; i++)
            {
                string formated = i.ToString("00000");
                var buffer = formated.ToBytes();
                client.Send(buffer, buffer.Length, sep);
            }
        }
        static void test_tcp()
        {
            var server = new TCPServer();
            server.OnAccept = delegate (TCPServer.Peer peer)
            {
                Util.Print("accept:", peer);
                var sb = new System.Text.StringBuilder();
                peer.OnReceive = delegate (byte[] data, int len)
                {
                    sb.Append(System.Text.Encoding.UTF8.GetString(data, 0, len));
                    if (data[len - 1] == '\n')
                    {
                        Util.Print("recv:", sb);
                        peer.Send(sb.ToString().ToBytes());
                        sb.Clear();
                    }
                };

                peer.OnClose = delegate ()
                {
                    Util.Print("close:", peer);
                };
            };
            var port = 9999;
            Util.Print("Listen on port:", port);
            server.Start(port);
        }
        static async System.Threading.Tasks.Task test_async()
        {
            using (var http = new System.Net.Http.HttpClient())
            {
                string str = await http.GetStringAsync("http://www.baidu.com");
                Util.Print(str);
            }

        }

        static void test_sort()
        {
            var points = getPoints();
            points.Sort(delegate (Point pt1, Point pt2)
            {
                if (pt1.x < pt2.x)
                    return -1;
                if (pt1.x > pt2.x)
                    return 1;
                if (pt1.y <= pt2.y)
                    return -1;
                return 1;
            });
            printPoints(points);
        }


        static List<Point> getPoints()
        {
            var points = new List<Point>(10);
            var rand = new Random();
            for (int i = 0; i < 20; i++)
            {
                var point = new Point(rand.Next(10), rand.Next(10));
                points.Add(point);
            }
            return points;
        }

        static void printPoints(List<Point> points)
        {
            foreach (var point in points)
            {
                Util.Print("x:", point.x, ",y:", point.y);
            }
        }
        static void test_list()
        {
            int[] nums = { 1, 2, 3, 4, 5 };
            var numList = new List<int>(nums);
            numList.Insert(3, 100);
            foreach (int num in numList)
            {
                Util.Print(num);
            }
        }
        static void test_rand()
        {
            int seed = (int)DateTime.Today.Ticks;
            var rand = new Random(seed);
            var sum = 0;
            var N = 100;
            for (int i = 0; i < N; i++)
            {
                int v = rand.Next(100);
                Util.Print(v);
                sum += v;
            }
            Util.Print("avg:", sum / N);
        }
        static void test_datetime()
        {
            var last = DateTime.Today;
            var now = DateTime.Now;
            Util.Print("now:", now);
            Util.Print("ticks:", now.Ticks);
            Util.Print("diff:", now.Subtract(last));
        }
        static void test_str()
        {

            var str1 = "World";
            Util.Print($"Hello {str1}");
            var str2 = "World";
            if (str1 == str2)
            {
                Util.Print("==");
            }
            if (str1.Equals(str2))
            {
                Util.Print("Equals");
            }

            var str3 = String.Format("Hello {0},{1}", str1, str2);
            Util.Print(str3.Replace(str1, "China"));
            Util.Print("str3=", str3);
            Util.Print(str3.Reverse());

        }

        static void test_file()
        {
            using (var file = File.CreateText("1.txt"))
            {
                long v1 = 1;
                long v2 = 1;
                for (var i = 0; i < 80; i++)
                {
                    var v3 = v1 + v2;
                    file.WriteLineAsync(v3.ToString());
                    v1 = v2;
                    v2 = v3;
                }
            }
        }

    }
}
