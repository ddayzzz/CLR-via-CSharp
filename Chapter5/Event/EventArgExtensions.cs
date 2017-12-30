using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Basic.Event
{
    public static class EventArgExtensions
    {
        public static void Raise<TEventArgs>(this TEventArgs e, Object sender, ref EventHandler<TEventArgs> eventDelegate)
        {
            //出于线程安全的考虑，把处理的委托字段复制到临时的字段
            var temp = Volatile.Read(ref eventDelegate);
            if (temp != null)
                temp(sender, e);
        }
    }
}
