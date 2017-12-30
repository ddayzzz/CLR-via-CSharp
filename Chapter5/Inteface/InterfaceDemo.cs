using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Inteface
{
    namespace InterfaceDemo
    {
        class Base : IDisposable
        {
            public virtual void Dispose()
            {
                Console.WriteLine("Base's dispose()");
            }
        }
        class Derived:Base, IDisposable//Base已经实现了Dispose
        {
            public override void Dispose()//Base中的Dispose标记为virtual和sealed。可以显式指定virtual，可以继承
            {
                Console.WriteLine("Derived's Dispose()");
                base.Dispose();
            }
        }
        class Test
        {
            public static void Main()
            {
                Base b = new Base();
                b.Dispose();
                Console.WriteLine("----");
                Derived d = new Derived();
                d.Dispose();
                Console.WriteLine("----");
                ((IDisposable)b).Dispose();
                Console.WriteLine("----");
                ((IDisposable)d).Dispose();
            }
        }
    }
    namespace EIMIDemo
    {
        class SimpleType:IDisposable
        {
            public void Dispose()
            {
                Console.WriteLine("public Dispose");
            }
            void IDisposable.Dispose()
            {
                Console.WriteLine("IDisposable Dispose");
            }
        }
        class Test
        {
            public static void Main()
            {
                SimpleType s = new SimpleType();
                s.Dispose();
                ((IDisposable)s).Dispose();
                //EIMI的问题
                Int32 x = 5;
                Single s1 = ((IConvertible)x).ToSingle(null);
            }
        }
    }
}
