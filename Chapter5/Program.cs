using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic
{
    class Program
    {
        static void Main(string[] args)
        {
            Int32 n = 52;
            Object o = n;
            n = 1333;
            Console.WriteLine(n + ", " + (Int32)o);
        }
    }
    class DynamicDemo
    {
        public static void Main()
        {
            dynamic value;
            Int32 demo = 25;
            value = (demo == 2) ? (dynamic)5 : (dynamic)"A";
            value = value + value;
            Console.WriteLine(value);
        }
    }
}
