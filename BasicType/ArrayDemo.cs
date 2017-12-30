using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicType.ArrayDemo
{
    public static class ArrayExtension
    {
        public static void Show(this System.Array array)
        {
            foreach (var item in array)
                Console.Write(item + " ");
            Console.WriteLine();
        }

    }
    class ArrayDemo
    {
        public static void Main()
        {
            //检测一下
            Object[] objs = { "13", "25" };
            String[] strs = new String[2];
            Array.Copy(objs, strs, objs.Length);
            foreach (var item in strs)
                Console.WriteLine(item);

        }
    }
    class CopyDemo
    {
        public static void Main()
        {
            Byte[] bytes = { 0x01, 0x02, 0x03 };
            Int32[] other = new Int32[5];
            System.Buffer.BlockCopy(bytes, 0, other, 0, 2);//other ‭001000000001‬
            other.Show();
        }
    }
    /// <summary>
    /// 用于演示数组传递的拷贝问题
    /// </summary>
    class Classroom
    {
        private String[] names;
        private Int32 sz = 0;
        public String[] GetNames()
        {
            String[] s= new String[sz];
            Array.Copy(names, s, 0);
            return s;//返回的是一个新的对象。用户的更改不会作用到原始对象上
        }
        public void Show() => names.Show();
        public void Add(String name)
        {
            if (sz < names.Length)
                names[sz++] = name;
        }
        public Classroom(int capacity)
        {
            names = new String[capacity];
        }
        public static void Main()
        {
            Classroom cs = new Classroom(5);
            cs.Add("Shu");
            cs.Add("Liu");
            cs.Show();
            var s = cs.names;
            Console.WriteLine("ReferenceEqals:{0}", ReferenceEquals(s, cs.names));
            Console.WriteLine("==:{0}", s == cs.names);
            Console.WriteLine("Eqals:{0}", s.Equals(cs.names));
            s[0] = "Qiang";
            cs.Show();
        }
    }
    class NoZeroBasedArray
    {
        public static void Main()
        {
            Int32[] lowerBounds = { 2005, 1 };
            Int32[] lengths = { 5, 4 };
            Decimal[,] quarterlyRevenue = (Decimal[,])Array.CreateInstance(typeof(Decimal), lengths, lowerBounds);
            Console.WriteLine("{0,4} {1,9} {2,9} {3,9} {4,9}", "Year", "Q1", "Q2", "Q3", "Q4");
            Int32 firstYear = quarterlyRevenue.GetLowerBound(0);
            Int32 lastYear = quarterlyRevenue.GetUpperBound(0);
            Int32 firstQuarter = quarterlyRevenue.GetLowerBound(1);
            Int32 lastQuarter = quarterlyRevenue.GetUpperBound(1);
            for(Int32 year = firstYear;year <= lastYear;++year)
            {
                Console.Write(year + " ");
                for (Int32 quarter = firstQuarter; quarter <= lastQuarter; ++quarter)
                    Console.Write("{0,9:C}", quarterlyRevenue[year, quarter]);
                Console.WriteLine();
            }
        }
    }
    class RStackallocDemo
    {
        public unsafe static void Main()
        {
            StackAllocDemo();
            InlineArrayDemo();
        }
        internal unsafe struct CharArray
        {
            public fixed Char Characters[20];
        }
        private static void StackAllocDemo()
        {
            unsafe
            {
                const Int32 width = 20;
                Char* pc = stackalloc Char[width];
                String s = "Shu Wang";
                for (Int32 index = 0; index < width; ++index)
                    pc[width - index - 1] = (index < s.Length) ? s[index] : '.';
                Console.WriteLine(new String(pc, 0, width));//由于不是FCL类型，所以必须
            }
        }
        private static void InlineArrayDemo()
        {
            unsafe
            {
                CharArray ca;
                Int32 widthInBytes = sizeof(CharArray);
                Int32 width = widthInBytes / 2;
                String s = "Shu Wang";
                for (Int32 index = 0; index < width; index++)
                    ca.Characters[width - index - 1] = (index < s.Length) ? s[index] : '.';
                Console.WriteLine(new String(ca.Characters, 0, width));
            }
        }
    }
}
