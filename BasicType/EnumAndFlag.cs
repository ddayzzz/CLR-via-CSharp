using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace BasicType.EnumAndFlag
{
    class EnumAndFlag
    {
        enum Color:byte
        {
            White = 0,
            Red = 1,
            Green = 2,
            Blue = 3,
            Orange = 4
        }
        public static void Main()
        {
            Console.WriteLine(Enum.Format(typeof(Color), (Byte)30, "G"));
            Color[] colors = (Color[])Enum.GetValues(typeof(Color));
            Console.WriteLine("Color枚举定义了{0}个符号", colors.Length);
            Console.WriteLine("Value\tSymbol\n-----\t-----");
            foreach(var c in colors)
            {
                Console.WriteLine("{0,5:D}\t{0:G}", c);
            }
            Console.WriteLine(Enum.GetUnderlyingType(typeof(Color)));
        }
    }
    internal class FlagDemo
    {
        //[Flags]
        internal enum Action
        {
            None = 0,
            Read = 0x0001,
            Write = 0x002,
            ReadWrite = Read | Write,
            Delete = 0x0004,
            Query = 0x0008,
            Sync = 0x0010
        }
        public static void Main()
        {
            String file = Assembly.GetEntryAssembly().Location;
            FileAttributes attributes = File.GetAttributes(file);
            Console.WriteLine("Is {0} hidden? {1}", file, (attributes & FileAttributes.Hidden) != 0);
            Action action = Action.Read | Action.Delete;
            Console.WriteLine(action);
            Console.WriteLine(action.ToString("F"));//F格式化对象 https://msdn.microsoft.com/zh-cn/library/26etazsy.aspx#standardStrings
            //从字符串返回标志类型实例
            Action a = (Action)Enum.Parse(typeof(Action), "5", true);
            Console.WriteLine(a.ToString("F"));
            Console.WriteLine("扩展方法的使用：");
            action.ForEach(f => Console.WriteLine(f.ToString("F")));
        }
    }
    /// <summary>
    /// 定义标志的扩展
    /// </summary>
    static class FlagExtension
    {
        public static void ForEach(this FlagDemo.Action action, Action<FlagDemo.Action> process)
        {
            if (process == null)
                throw new ArgumentNullException("必须有处理");
            for(UInt32 bit=1; bit !=0;bit<<=1)
            {
                UInt32 temp = bit & ((UInt32)action);
                if (temp != 0)
                    process((FlagDemo.Action)temp);//显示每一个置位
            }
        }
    }
}
