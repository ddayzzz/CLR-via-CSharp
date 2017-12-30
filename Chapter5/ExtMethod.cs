using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic
{
    public static class StringBuilderExtension
    {
        public static Int32 IndexOf(this StringBuilder sb, Char value)
        {
            for (var i = 0; i < sb.Length; ++i)
                if (sb[i] == value)
                    return i;
            return -1;
        }
    }
    /* 自动生成的元信息
     CustomAttribute #1 (0c000012)
	-------------------------------------------------------
		CustomAttribute Type: 0a000001
		CustomAttributeName: System.Runtime.CompilerServices.ExtensionAttribute :: instance void .ctor()
		Length: 4
		Value : 01 00 00 00                                      >                <
		ctor args: ()
    */
    public static class InvokeAndCatchExtension
    {
        public static void InvokeAndCatch<TException>(this Action<Object> d, Object o)
            where TException:Exception
        {
            try
            {
                d(o);
            }
            catch(TException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
    class ExtMethod
    {
        public static void Main()
        {
            StringBuilder sb = new StringBuilder("ABCD");
            Int32 i = StringBuilderExtension.IndexOf(sb.Replace('D', 'G'), 'G');
            Action<Object> action = (Object o) => Console.WriteLine(o.GetType());
            action.InvokeAndCatch<NullReferenceException>(null);
        }
    }
}
