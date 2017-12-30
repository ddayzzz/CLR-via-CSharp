using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//简化方式2，
using DataTimeList2 = System.Collections.Generic.List<System.DateTime>;
namespace Basic.Generics
{
    namespace ClassType
    {
        internal sealed class DictionaryStringKey<TValue> :
            Dictionary<String, TValue>
        {

        }
        class Test
        {
            public static void Main()
            {
                Object o = null;
                Type t = typeof(Dictionary<Int32,String>);
                Activator.CreateInstance(t);
            }
        }
    }
    namespace DifferentTypeTogether
    {
        
        internal class Node
        {
            protected Node m_next;
            public Node(Node next)
            {
                m_next = next;
            }
        }
        internal class TypedNode<T> : Node
        {
            public T m_data;
            public TypedNode(T data):this(data, null)
            {

            }
            public TypedNode(T data, Node next):base(next)
            {
                m_data = data;
            }
            public override string ToString()
            {
                return m_data.ToString() +
                    ((m_next !=null) ? m_next.ToString(): String.Empty);
            }
        }
        //简化方式1，同一性被破坏
        internal class DateTimeList:System.Collections.Generic.List<DateTime>
        {

        }
        
        public class Test
        {
            public static void Main()
            {
                //合并了不同的类型
                //Node head = new TypedNode<Char>('.');
                //head = new TypedNode<DateTime>(DateTime.Now, head);
                //head = new TypedNode<String>("Shu", head);
                //Console.WriteLine(head.ToString());
                //泛型同一性
                Console.WriteLine(typeof(DateTimeList) == typeof(List<DateTime>));
                Console.WriteLine(typeof(DataTimeList2) == typeof(List<DateTime>));
            }
        }
    }
    namespace CodeBang
    {
        internal class GT<T>
        {
            internal static T field = default(T);
            
        }
        internal class Test
        {
            public static void Main()
            {
                GT<Double>.field = 22;
                Console.WriteLine(GT<Int32>.field);
                Console.WriteLine(GT<Double>.field);
            }
        }
    }
    namespace DelegateGenerics
    {
        class Test
        {
            public delegate TResult MyFunc<in T, out TResult>(T arg);
            public static Int32 Func1(String s1)
            {
                return 1;
            }
            public static void Main()
            {
                MyFunc<String, Int32> ff1 = Func1;
                MyFunc<DateTime, Int32> ff2 = (DateTime d) => d.Day;
            }
        }
    }
    namespace Constraint
    {
        class Base
        {
            public delegate Int32 D1();
            public delegate D1 D2(Int32 ss);
            public virtual void M<T1, T2>()
                where T1:struct
                where T2:class
            {
                D2 dd = (Int32 d) => { return () => 1; };
                dd(25)();
            }
        }
        class Derived : Base
        {
            //不允许重复指定类型限定
            public override void M<T1, T2>()
            {
                base.M<T1, T2>();
            }
        }
        class Other
        {
            public static void CompareTwoType<T>(T o1, T o2)
            where T : class//T不能限定位struct，值类型未必定义了==。如果不约束，==运算符不一定适用于所有的对象。class或者约束为重载了==的类型，则可以
            {
                Boolean res = o1 == o2;
            }
            //public static T Sum<T>(IEnumerable<T> a)
            //    where T:class
            //{
            //    T sum = default(T);
            //    foreach(var i in a)
            //    {
            //        sum += i;//指定为class,struct或不指定会出错：值类型
            //    }
            //    return sum;
            //}
            //模拟了一个加法
            public static T Sum<T>(IEnumerable<T> a)
            {
                Double sum = 0.0;
                foreach (var i in a)
                {
                    sum += Convert.ToDouble(i);
                }
                return (dynamic)sum;
            }
           public static dynamic ReturnDynamic(int d)
            {
                return (d % 2 == 0 ? (dynamic)5 : (dynamic)10.3);
            }
            public static void Main()
            {
                var arr = new dynamic[5];
                for (int i = 0; i < 5; ++i)
                    arr[i] = ReturnDynamic(i);
                Console.WriteLine(Sum(arr));
                Console.ReadKey();
            }
        }
    }

}
