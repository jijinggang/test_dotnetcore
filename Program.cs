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

                var cmd = args[0];

                if (cmd == "udpclient" && args.Length >= 3)
                {
                    //udpclient 127.0.0.1 8888
                    test_udp_interact(args[1], int.Parse(args[2]));
                }
                else if (cmd == "udpserver" && args.Length > 1)
                {
                    //udpserver 8888
                    test_udp_server(int.Parse(args[1]));
                    System.Threading.Thread.Sleep(int.MaxValue);
                }
            }
        }

        static void test_udp_interact(string remoteIp, int remotePort)
        {
            Util.P("remote host is ", remoteIp, ":", remotePort);
            Util.P("input your message:");

            var client = new UDPSock();
            var remoteEP = new IPEndPoint(IPAddress.Parse(remoteIp), remotePort);
            client.OnReceive = delegate (EndPoint ep, byte[] data, int len)
            {
                Util.P("[RECV]", ep.ToString(), ":", System.Text.Encoding.UTF8.GetString(data, 0, len));
            };
            while (true)
            {
                var input = Console.ReadLine();
                if (input == "exit")
                {
                    break;
                }
                var buff = input.ToBytes();
                client.Send(buff, buff.Length, remoteEP);
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
            Util.P("udp server open on:", port);
            var serv = new UDPSock(port);
            serv.OnReceive = delegate (EndPoint ep, byte[] data, int len)
            {
                serv.Send(data, len, ep);
                Util.P("server recv from ", ep.ToString(), ":", System.Text.Encoding.UTF8.GetString(data, 0, len));
            };
        }
        static void test_udp_client(int port)
        {
            var client = new UDPSock();
            client.OnReceive = delegate (EndPoint ep, byte[] data, int len)
            {
                Util.P("client recv from ", ep.ToString(), ":", System.Text.Encoding.UTF8.GetString(data, 0, len));
            };
            var sep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            for (int i = 0; i < 1; i++)
            {
                string s = i.ToString("00000");
                var buff = s.ToBytes();
                client.Send(buff, buff.Length, sep);
            }
        }
        static void test_tcp()
        {
            var server = new TCPServ();
            server.OnAccept = delegate (TCPServ.Peer peer)
            {
                Util.P("accept:", peer);
                var sb = new System.Text.StringBuilder();
                peer.OnReceive = delegate (byte[] data, int len)
                {
                    sb.Append(System.Text.Encoding.UTF8.GetString(data, 0, len));
                    if (data[len - 1] == '\n')
                    {
                        Util.P("recv:", sb);
                        peer.Send(sb.ToString().ToBytes());
                        sb.Clear();
                    }
                };

                peer.OnClose = delegate ()
                {
                    Util.P("close:", peer);
                };
            };
            var port = 9999;
            Util.P("Listen on port:", port);
            server.Start(port);
        }
        static async System.Threading.Tasks.Task test_async()
        {
            using (var http = new System.Net.Http.HttpClient())
            {
                string str = await http.GetStringAsync("http://www.baidu.com");
                Util.P(str);
            }

        }

        static void test_sort()
        {
            var pts = getPoints();
            pts.Sort(delegate (Point pt1, Point pt2)
            {
                if (pt1.x < pt2.x)
                    return -1;
                if (pt1.x > pt2.x)
                    return 1;
                if (pt1.y <= pt2.y)
                    return -1;
                return 1;
            });
            printPoints(pts);
        }


        static List<Point> getPoints()
        {
            var pts = new List<Point>(10);
            var r = new Random();
            for (int i = 0; i < 20; i++)
            {
                var pt = new Point(r.Next(10), r.Next(10));
                pts.Add(pt);
            }
            return pts;
        }

        static void printPoints(List<Point> pts)
        {
            foreach (var pt in pts)
            {
                Util.P("x:", pt.x, ",y:", pt.y);
            }
        }
        static void test_list()
        {
            int[] nums = { 1, 2, 3, 4, 5 };
            var l = new List<int>(nums);
            l.Insert(3, 100);
            foreach (int num in l)
            {
                Util.P(num);
            }
        }
        static void test_rand()
        {
            int seed = (int)DateTime.Today.Ticks;
            var r = new Random(seed);
            var sum = 0;
            var N = 100;
            for (int i = 0; i < N; i++)
            {
                int v = r.Next(100);
                Util.P(v);
                sum += v;
            }
            Util.P("avg:", sum / N);
        }
        static void test_datetime()
        {
            var last = DateTime.Today;
            var now = DateTime.Now;
            Util.P("now:", now);
            Util.P("ticks:", now.Ticks);
            Util.P("diff:", now.Subtract(last));
        }
        static void test_str()
        {

            var s1 = "World";
            Util.P($"Hello {s1}");
            var s2 = "World";
            if (s1 == s2)
            {
                Util.P("==");
            }
            if (s1.Equals(s2))
            {
                Util.P("Equals");
            }

            var s3 = String.Format("Hello {0},{1}", s1, s2);
            Util.P(s3.Replace(s1, "China"));
            Util.P("s3=", s3);
            Util.P(s3.Reverse());

        }

        static void test_file()
        {
            using (var s = File.CreateText("1.txt"))
            {
                long v1 = 1;
                long v2 = 1;
                for (var i = 0; i < 80; i++)
                {
                    var v3 = v1 + v2;
                    s.WriteLineAsync(v3.ToString());
                    v1 = v2;
                    v2 = v3;
                }
            }
        }

    }
}
