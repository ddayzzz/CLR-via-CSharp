using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace CoreFeature
{
    class GCDemo
    {
        public static void Main()
        {
            Timer timer = new Timer(new TimerCallback(Callback), null, 0, 2000);
            Console.ReadLine();
        }
        public static void Callback(Object obj)
        {
            Console.WriteLine("Callback called");
            GC.Collect();
        }
    }
    class GCNotification
    {
        //https://www.cnblogs.com/mq0036/p/3707257.html
        private static Action<Int32> s_gcDone = null;//事件字段
        public static event Action<Int32> GCDone
        {
            add
            {
                if (s_gcDone == null)
                {
                    new GenObject(0); new GenObject(2);//代为0或者2触发事件
                }
                s_gcDone += value;
            }
            remove
            {
                s_gcDone -= value;
            }
        }
        private sealed class GenObject
        {
            private Int32 m_generation;
            public GenObject(Int32 generation)
            {
                m_generation = generation;
            }
            ~GenObject()
            {
                //对象在我们期望的代或者更高的代中，通知委托时间GC刚刚完成
                if (GC.GetGeneration(this) >= m_generation)
                {
                    Volatile.Read(ref s_gcDone)?.Invoke(m_generation);//调用委托
                }
                if ((s_gcDone != null)//还有未委托的事件
                    && !AppDomain.CurrentDomain.IsFinalizingForUnload()
                    && !Environment.HasShutdownStarted)//是否在卸载AppDomain
                {
                    if (m_generation == 0)
                        new GenObject(0);//监控新的对象的代为0
                    else
                        GC.ReRegisterForFinalize(this);//请求终结
                }
                else
                {
                    //放过对象，让其被回收
                }

            }
        }
        public static void Main()
        {
            GCNotification.GCDone += (Int32 d) => Console.WriteLine(d);
            List<Int32> sp = new List<Int32>();
            for (int i = 0; i <500000; ++i)
            {
                
                Object o = new object();//分配对象
                if (i % 50 == 0)
                    GC.Collect();//强制垃圾回收
            }
        }


    }
    namespace FinalizeDemo
    {
       class DisposeDemo
        {
            public static void Main()
            {
                Byte[] bytes = new Byte[] { 1, 2, 3, 4, 5 };
                FileStream io = new FileStream("temp.dat", FileMode.Create);
                io.Write(bytes, 0, bytes.Length);
                io.Dispose();
                io.Dispose();
                using (io)
                {
                    //use io
                }
            }
        }
    }
    class HandlerCollector
    {
        private sealed class BigNativeResource
        {
            private Int32 m_size;
            public BigNativeResource(Int32 size)
            {
                m_size = size;
                if (m_size > 0)
                    GC.AddMemoryPressure(m_size);//将非托管资源的实际大小通知GC
                Console.WriteLine("BigNativeResource create.");
            }
            ~BigNativeResource()
            {
                if (m_size > 0)
                    GC.RemoveMemoryPressure(m_size);
                Console.WriteLine("BigNativeResource destory.");
            }
        }
        private static void HandleCollectorDemo()
        {
            Console.WriteLine();
            Console.WriteLine("HandleCollectorDemo");
            for (Int32 count = 0; count < 10; ++count)
                new LimitedResource();
            GC.Collect();
        }
        private sealed class LimitedResource
        {
            //创建一个HandleCollector，告诉它当两个或者更多的对象
            //存放于堆中，就执行回收
            private static readonly HandleCollector s_hc = new HandleCollector("LimitedResource", 2);
            public LimitedResource()
            {
                //告诉HandleCollecter堆中增加了一个LimitedResources对象
                s_hc.Add();
                Console.WriteLine("LimitedResource create, Count={0},", s_hc.Count);
            }
            ~LimitedResource()
            {
                s_hc.Remove();
                Console.WriteLine("LimitedResouce destory. Count:{0}", s_hc.Count);
            }
        }
        private static void MemoryPressureDemo(Int32 size)
        {
            Console.WriteLine();
            Console.WriteLine("MemoryPressureDemo, size={0}", size);
            for (Int32 d = 0; d < 15; ++d)
                new BigNativeResource(size);
            GC.Collect();
        }
        public static void Main()
        {
            MemoryPressureDemo(0);
            MemoryPressureDemo(10 * 1024 * 1024);
            HandleCollectorDemo();
        }
    }
    namespace GCHandleDemo
    {
        public delegate Boolean CallBack(Int32 handle, IntPtr param);
        public class LibWrap
        {
            [DllImport("user32.dll")]
            public static extern Boolean EnumWindows(CallBack cb, IntPtr param);//枚举顶层的窗口句柄

        }
        public class Test
        {
            //ref http://blog.csdn.net/robingaoxb/article/details/6199514
            public static void Main()
            {
                TextWriter tw = System.Console.Out;
                GCHandle gch = GCHandle.Alloc(tw);//固定GCHandle 是内存不发生移动
                CallBack cwep = new CallBack(CaptureEnumWindowsProc);
                LibWrap.EnumWindows(cwep, (IntPtr)gch);
                gch.Free();
                Console.ReadKey();
            }
            /// <summary>
            /// 获取窗口ID https://msdn.microsoft.com/en-us/library/windows/desktop/ms633497(v=vs.85).aspx
            /// </summary>
            /// <param name="handle">定义的回调函数</param>
            /// <param name="param">传递给回调函数的参数</param>
            /// <returns></returns>
            private static Boolean CaptureEnumWindowsProc(Int32 handle, IntPtr param)
            {
                GCHandle gch = (GCHandle)param;
                TextWriter tw = (TextWriter)gch.Target;
                tw.WriteLine(handle);
                return true;
            }
        }
    }
    class FixedDemo
    {
        unsafe public static void Main()
        {
            //分配大对象, 导致对象被释放
            for(Int32 x= 0;x < 1000000;++x)
            {
                new Object();
            }
            IntPtr originalMemoryAddress;
            Byte[] bytes = new Byte[1000];
            fixed(Byte* pBytes = bytes)
            {
                originalMemoryAddress = (IntPtr)pBytes;
            }
            //Byte可能被压缩
            GC.Collect();
            fixed(Byte* pbytes = bytes)
            {
                Console.WriteLine("The Byte[] did {0} move during GC",
                    (originalMemoryAddress == (IntPtr)pbytes ? "not" : null));
            }
        }
    }
    namespace ConditionalWeakTableDemo
    {
        internal static class ConditionalWeakTableDemo
        {
            public static void Main()
            {
                Object o = new Object().GCWatch("My Object created at " + DateTime.Now);
                GC.Collect();
                GC.KeepAlive(o);//确定o引用的对象现在还活着
                o = null;
                Thread.Sleep(1000);
                GC.Collect();
                Console.ReadLine();
            }


        }
        internal static class GCWatcher
        {
            private readonly static ConditionalWeakTable<Object, NotifyWhenGCd<String>> s_cwt
                =new ConditionalWeakTable<object, NotifyWhenGCd<string>>();
            public static T GCWatch<T>(this T @object, String tag) where T : class
            {
                s_cwt.Add(@object, new NotifyWhenGCd<string>(tag));
                return @object;
            }
            private sealed class NotifyWhenGCd<T>
            {
                private readonly T m_value;
                internal NotifyWhenGCd(T value)
                {
                    m_value = value;
                }
                public override string ToString()
                {
                    return m_value.ToString();
                }
                ~NotifyWhenGCd() { Console.WriteLine("GC'd:" + m_value); }

            }
        }
    }
    namespace ConditionalWeakTableDemo2
    {
        //来源 https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.conditionalweaktable-2?view=netframework-4.7
        //来源2 http://www.cnblogs.com/TianFang/p/3546556.html
        using System;
        using System.Runtime.CompilerServices;
        public class Example
       
        {
            public static void WeakPtrToData()
            {
                Console.WriteLine("使用弱引用的Key，垃圾回收后被释放：");
                Dictionary<WeakReference<ManagedClass>, ClassData> dictionary = new Dictionary<WeakReference<ManagedClass>, ClassData>();
                ManagedClass m1 = new ManagedClass();
                WeakReference<ManagedClass> wm1 = new WeakReference<ManagedClass>(m1);
                ClassData d1 = new ClassData();
                dictionary[wm1] = d1;
                m1 = null;
                GC.Collect();
                //foreach (var pair in dictionary)
                //{
                //    Console.WriteLine("{0}->{1}", pair.Key, pair.Value);
                //}
                Console.WriteLine("After GC");
                Console.ReadKey();
                Console.WriteLine(dictionary);
                /*输出
ManagedClass ctor called
ClassData ctor called
After GC
~ManagedClass called//提前释放了，Dictionary没用强引用Key实例
System.Collections.Generic.Dictionary`2[System.WeakReference`1[CoreFeature.ConditionalWeakTableDemo2.ManagedClass],CoreFeature.ConditionalWeakTableDemo2.ClassData]
~ClassData called
                 */
            }
            public static void KeyToData()
            {
                Console.WriteLine("GC后，不会立即释放：");
                Dictionary<ManagedClass, ClassData> dictionary = new Dictionary<ManagedClass, ClassData>();
                ManagedClass m1 = new ManagedClass();
                ClassData d1 = new ClassData();
                dictionary[m1] = d1;
                m1 = null;
                GC.Collect();
                Console.WriteLine("After GC");
                Console.ReadKey();
                Console.WriteLine(dictionary);
                /*输出
ManagedClass ctor called
ClassData ctor called
After GC
System.Collections.Generic.Dictionary`2[CoreFeature.ConditionalWeakTableDemo2.ManagedClass,CoreFeature.ConditionalWeakTableDemo2.ClassData]
~ClassData called
~ManagedClass called
                 */
            }
            public static void ConditionalWeakTable()
            {
                var mc1 = new ManagedClass();
                var cwt = new ConditionalWeakTable<ManagedClass, ClassData>();//弱引用字典
                cwt.Add(mc1, new ClassData());//建立各自的关联

                //var wr2 = new WeakReference(mc2);//wr2弱引用mc2
                //mc2 = null;
                mc1 = null;
                GC.Collect();
                Console.WriteLine("After GC");
                //ClassData data = null;
                //cwt.TryGetValue(mc1, out data);
                //if (wr2.Target == null)
                //    Console.WriteLine("No strong reference to mc2 exists.");
                //else if (cwt.TryGetValue(mc3, out data))//必须使用Release编译才不会报错
                //    Console.WriteLine("Data created at {0}", data.CreationTime);
                //else
                //    Console.WriteLine("mc2 not found in the table.");
                Console.ReadKey();
                /*
ManagedClass ctor called
ClassData ctor called
~ClassData called
~ManagedClass called
                 * */
            }
            public static void Main()
            {
                //KeyToData();
                //WeakPtrToData();
                ConditionalWeakTable();
            }
        }

        public class ManagedClass
        {
            public ManagedClass()
            {
                Console.WriteLine("ManagedClass ctor called");
            }
            ~ManagedClass()
            {
                Console.WriteLine("~ManagedClass called");
            }
        }

        public class ClassData
        {
            public DateTime CreationTime;
            public object Data;

            public ClassData()
            {
                CreationTime = DateTime.Now;
                this.Data = new object();
                Console.WriteLine("ClassData ctor called");
            }
            ~ClassData()
            {
                Console.WriteLine("~ClassData called");
            }
        }
        // The example displays the following output:
        //       No strong reference to mc2 exists.
    }
}
