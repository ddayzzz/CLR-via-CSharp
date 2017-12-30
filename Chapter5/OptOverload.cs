using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic
{
    class MyClass
    {
        private int m_int;
        public MyClass(int i)
        {
            m_int = i;
        }
        public static MyClass operator +(MyClass lhs, MyClass rhs)
        {
            return new MyClass(lhs.m_int + rhs.m_int);
        }
        /// <summary>
        /// 显式强制转换为直接返回Int32类型
        /// </summary>
        /// <param name="e">对象</param>
        public static explicit operator Int32(MyClass e)
        {
            return e.ToInt32();
        }
        public Int32 ToInt32()
        {
            return this.m_int;
        }
        //public static MyClass op_Addition(MyClass lhs, MyClass rhs)不能定义因为这就是operator+的友好名称
        //{
        //    return lhs + rhs;
        //}
        public override string ToString()
        {
            return m_int.ToString();
        }
    }
    class Rational
    {
        private Double m_double;
        public Rational(Double i)
        {
            m_double = i;
        }
        public static Rational operator +(Rational lhs, Rational rhs)
        {
            return new Rational(lhs.m_double + rhs.m_double);
        }
        /// <summary>
        /// 显式强制转换为Int32类型
        /// </summary>
        /// <param name="e">对象</param>
        public static explicit operator Int32(Rational e)
        {
            return e.ToInt32();
        }
        public Int32 ToInt32()
        {
            return (Int32)this.m_double;
        }
        /// <summary>
        /// 隐式从d转换为有理数类型
        /// </summary>
        /// <param name="d"></param>
        public static implicit operator Rational(Double d)
        {
            return new Rational(d);
        }
        public override string ToString()
        {
            return m_double.ToString();
        }
    }
    class OptOverload
    {
        public static void Main()
        {
            Decimal d = 55;
            Rational r1 = 55.0;
            Rational r2 = 52.2;
            Console.WriteLine(r1 + r2);
            Int32 i1 = (Int32)r1;
            Int32 i2 = (Int32)r2;
            Console.WriteLine(i1);
            Console.WriteLine(i2);
        }
    }
}
