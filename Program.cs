using System.IO;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using common;
using test;
namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // test_list();
            // test_rand();
            // test_datetime();
            // test_str();
            // test_file();
            // test_sort();
            test_async().Wait();
            Util.P("ok");
            
        }
        static async System.Threading.Tasks.Task test_async(){
            using (var http = new System.Net.Http.HttpClient())
            {
                string str = await http.GetStringAsync("http://www.baidu.com");
                Util.P(str);
            }

        }
        static void test_socket()
        {
            var ep = new IPEndPoint(0, 9999);
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(ep);
            sock.Listen(0);
            while (true)
            {
                for (int i = 0; i < 100; i++)
                {
                    Util.P(i);
                }
            }

        }

        static void test_sort(){
            var pts = getPoints();
            pts.Sort(delegate (Point pt1, Point pt2) {
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


        static List<Point> getPoints(){
            var pts = new List<Point>(10);
            var r = new Random();
            for(int i = 0; i < 20; i++){
                var pt = new Point(r.Next(10), r.Next(10));
                pts.Add(pt);
            }
            return pts;
        }

        static void printPoints(List<Point> pts){
            foreach(var pt in pts){
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
