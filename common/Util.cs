using System;
namespace common
{
    public static class Util{
     
        public static void P(params object[] args)
        {
            foreach(var arg in args){
                Console.Write(arg.ToString());
            }
            Console.Write("\n");
        }
    }

}