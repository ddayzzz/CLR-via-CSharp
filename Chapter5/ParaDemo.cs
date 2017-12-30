using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic
{

    class ParaDemo
    {
        private static Int32 m_count = 0;
        public static void M(Int32 x=9, String s="A", DateTime date=default(DateTime), Guid guid=new Guid())
        {
            Console.WriteLine($"x={x}, s={s}, date={date}, guid={guid}");
        }
        public static void Main()
        {
            M(9, "B");
            M(guid:new Guid("{89781D01-3D44-4837-8787-327558095565}"));
            M();
        }
    }
}
