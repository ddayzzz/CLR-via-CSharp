using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic
{
    class PropertyDemo
    {
        public static void Main()
        {
            var o1 = new { Name = "Shu", Date = new DateTime(1997, 12, 22) };
            var o2 = new { Name = "Shu", Date = new DateTime(1997, 12, 22) };
            Console.WriteLine(o1==o2);//o1、o2的类型是相同的。Equals方法被重载，==还是比较的对象实例是否相同
        }
    }
    class TupleDemo
    {
        public static Tuple<Int32, Int32> MinMax(Int32 a, Int32 b)
        {
            return new Tuple<int, int>(Math.Min(a, b), Math.Max(a, b));
        }
        public static void Main()
        {
            var minMax = MinMax(50, 33);
            Console.WriteLine("Min:{0}, Max:{1}", minMax.Item1, minMax.Item2);
        }
    }
    class ExpandoDemo
    {
        public static void Main()
        {
            dynamic e = new System.Dynamic.ExpandoObject();
            e.x = 6;
            e.y = "Jeff";
            e.z = null;
            foreach(var v in (IDictionary<String, Object>)e)
            {
                Console.WriteLine("Key={0}, V={1}", v.Key, v.Value);

            }
        }
    }
    sealed class BitArray
    {
        private Byte[] m_byteArray;
        private Int32 m_numBits;
        public BitArray(Int32 numBits)
        {
            if (numBits <= 0)
                throw new ArgumentOutOfRangeException("numBits must be > 0");
            this.m_numBits = numBits;
            m_byteArray = new Byte[numBits];
        }
        [System.Runtime.CompilerServices.IndexerNameAttribute("Bit")]
        public Boolean this[Int32 bitPos]
        {
            get
            {
                if ((bitPos < 0) || (bitPos >= m_numBits))
                    throw new ArgumentOutOfRangeException("bitPos");
                return (m_byteArray[bitPos / 8] & (1 << (bitPos % 8))) != 0;//每一位保存一个状态
            }
            set
            {
                if ((bitPos < 0) || (bitPos >= m_numBits))
                    throw new ArgumentOutOfRangeException("bitPos", bitPos.ToString());
                if(value)
                {
                    //将指定的索引设置为true
                    m_byteArray[bitPos / 8] = (Byte)(m_byteArray[bitPos / 8] | (1 << (bitPos % 8)));
                }
                else
                {
                    m_byteArray[bitPos / 8] = (Byte)(m_byteArray[bitPos / 8] & ~(1 << (bitPos % 8)));
                }
            }
        }
        public static void Main()
        {
            List<int> s = new List<int>();
            BitArray ba = new BitArray(14);
            for(Int32 x=0;x<14;++x)
            {
                ba[x] = (x % 2 == 0);
            }
            for(Int32 x=0;x<14;++x) 
            {
                
                Console.WriteLine("Bit " + x + " is " + (ba[x] ? "On" : "Off"));
            }
        }
    }
}
