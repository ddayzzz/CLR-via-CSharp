using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic
{
    public struct Point
    {
        private int m_x;
        private int m_y;
        public Point(int x,int y)
        {
            m_x = x;
            m_y = y;
            Console.WriteLine($"Point.ctor{x},{y}");
        }
        static Point()
        {
            
        }
    }
    class ClassDemo
    {
        public static void Main()
        {
            Point point1 = new Point();
            Point point2 = new Point(1,3);
        }
    }
}
