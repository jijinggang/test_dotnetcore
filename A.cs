namespace test{
    public class A{
        public int  Width{ get; private set; }
        public int  Heigth{ get; private set; }
        public A(int width = 1280, int height=720){
            Width = width;
            Heigth = height;
        }
//

        public static void test(){
            var a = new A(width: 10, height: 200);
            //var a1 = new A { Width = 100, Heigth=122};
        }
    }

    public class Point{
        public int x;
        public int y;
        public Point(int x, int y){
            this.x = x;
            this.y = y;
        }
    }

}