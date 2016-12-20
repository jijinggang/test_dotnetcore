using System.IO;
using System;
using System.Collections.Generic;
using common;
namespace test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            test_udp();
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

        static void test_udp()
        {
            test_udp_server();
            test_udp_client();

            System.Threading.Thread.Sleep(1000);
        }
        static void test_udp_server()
        {
            var serv = new UDPSock(8888);
            serv.OnReceive = delegate (System.Net.EndPoint ep, byte[] data, int len)
            {
                Util.P("serv recv:", System.Text.Encoding.UTF8.GetString(data, 0, len));
                serv.Send(data, len, ep);
            };
        }
        static void test_udp_client()
        {
            var client = new UDPSock();
            var ep = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("localhost"), 8888);
            for (int i = 0; i < 10; i++)
            {
                string s = i.ToString();
                var buff = s.ToBytes();
                client.Send(buff, buff.Length, ep);
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
