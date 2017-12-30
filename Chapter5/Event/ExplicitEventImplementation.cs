using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Basic.Event
{
    
    /// <summary>
    /// 显式事件实现
    /// </summary>
    namespace ExplicitEventImplementation
    {
        public sealed class EventKey { }
        public sealed class EventSet
        {
            /// <summary>
            /// 维护一个EventKey->委托的映射
            /// </summary>
            private Dictionary<EventKey, Delegate> m_events = new Dictionary<EventKey, Delegate>();
            public void Add(EventKey eventKey, Delegate handler)
            {
                Monitor.Enter(m_events);//获取排他锁
                Delegate d;
                m_events.TryGetValue(eventKey, out d);
                m_events[eventKey] = Delegate.Combine(d, handler);//将委托于处理程序连接在一起
                Monitor.Exit(m_events);
            }
            public void Remove(EventKey eventKey, Delegate handler)
            {
                Monitor.Enter(m_events);
                Delegate d;
                if(m_events.TryGetValue(eventKey, out d))
                {
                    d = Delegate.Remove(d, handler);
                    if (d != null)
                        m_events[eventKey] = d;//后面的委托不需要删除
                    else
                        m_events.Remove(eventKey);//直接删除最后一个委托
                } 
            }
            public void Raise(EventKey eventKey, Object sender, EventArgs e)
            {
                //如果事件不在集合中，不抛出异常
                Delegate d;
                Monitor.Enter(m_events);
                m_events.TryGetValue(eventKey, out d);
                Monitor.Exit(m_events);
                if (d != null)
                    d.DynamicInvoke(new Object[] { sender, e });
            }
        }
        public class FooEventArgs:EventArgs { }
        public class TypeWithLotsOfEvents
        {
            private readonly EventSet m_eventSet = new EventSet();
            protected EventSet EventSet
            {
                get
                {
                    return m_eventSet;
                }
            }
            /// <summary>
            /// 每个对象有一个哈希码，以便在对象集合中查找这个事件
            /// </summary>
            protected static readonly EventKey s_fooEventKey = new EventKey();
            public event EventHandler<FooEventArgs> Foo
            {
                add { m_eventSet.Add(s_fooEventKey, value); }
                remove { m_eventSet.Remove(s_fooEventKey, value); }
            }
            protected virtual void OnFoo(FooEventArgs e)
            {
                m_eventSet.Raise(s_fooEventKey, this, e);
            }
            public void SimulateFoo()
            {
                //Thread.Sleep(2000);
                OnFoo(new FooEventArgs());
            }
        }
        public class Test
        {
            public static void Main()
            {
                TypeWithLotsOfEvents twle = new TypeWithLotsOfEvents();
                Thread[] ths = new Thread[5];
                for (int i = 0; i < 5; ++i)
                    ths[i] = new Thread(new ThreadStart(() =>
                    {
                        twle.Foo += HandleFoodEvent;
                        twle.SimulateFoo();
                    }));
                for (int i = 0; i < 5; ++i)
                {
                    ths[i].Name = "thread" + i;
                    ths[i].Start();
                }
            }
            private static void HandleFoodEvent(Object sender , EventArgs e)
            {
                Console.WriteLine("Hanlding FooEvent..." + Thread.CurrentThread.Name);
            }


        }
    }
}
