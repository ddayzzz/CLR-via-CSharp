using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace BasicType
{
    class StringType
    {
        static void Main(string[] args)
        {
            String s1 = "Strasse";
            String s2 = "Straße";
            CultureInfo ci = new CultureInfo("de-DE");
            Int32 equals = String.Compare(s1, s2, StringComparison.Ordinal);//序号排列
            Console.WriteLine("{0} {1} {2}", s1, equals == 0 ? "==" : "!=", s2);
            equals = String.Compare(s1, s2, true, ci);//语言文化排列
            Console.WriteLine("{0} {1} {2}", s1, equals == 0 ? "==" : "!=", s2);
            
        }
    }
    class StringInfoDemo
    {
        public static void Main()
        {
            String s = "a\u0304\u0308bc\u0327";
            SubstringByTextElements(s);
        }
        private static void SubstringByTextElements(String s)
        {
            String output = String.Empty;
            StringInfo si = new StringInfo(s);//拆分为文本元素（多个字符显示）
            for(Int32 element=0;element<si.LengthInTextElements;++element)
            {
                output += String.Format("文本元素 {0} is ‘{1}’{2}",
                    element, si.SubstringByTextElements(element, 1), Environment.NewLine);
            }
            MessageBox.Show(output);
            
        }
    }
    class StringMethod
    {
        public static void Main()
        {
            String s1 = "xyz";
            String s2 = s1.Clone() as String;
            String s3 = String.Copy(s1);
            Console.WriteLine("s1==s2 : {0}", ReferenceEquals(s1, s2));
            Console.WriteLine("s1 equals s2 : {0}", s1.Equals(s2));
            Console.WriteLine("s1==s3 : {0}", ReferenceEquals(s1, s3));
            Console.WriteLine("s1 equals s3 : {0}", s1.Equals(s3));
        }
    }
    class CultureInfoDemo
    {
        public static void Main()
        {
            Decimal price = 123.45M;
            String s = price.ToString("C", CultureInfo.GetCultureInfo("en-US"));//显示朝鲜圆货币格式
            DateTime d = new DateTime(1997, 12, 11);
            MessageBox.Show(d.ToString("D", CultureInfo.GetCultureInfo("ko-KR")));
            MessageBox.Show(s);
        }
    }
    class UserDefinedFormatter
    {
        public static void Main()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(new BoldInt32s(), "{0} {1} {2:M}", "Jeff", 123, DateTime.Now);
            Console.WriteLine(sb);
        }
        internal class BoldInt32s : IFormatProvider, ICustomFormatter
        {
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                String s;
                IFormattable formattable = arg as IFormattable;
                if (formattable == null)
                    s = arg.ToString();
                else
                    s = formattable.ToString(format, formatProvider);
                if (arg.GetType() == typeof(Int32))
                    return "<B>" + s + "</B>";
                return s;
            }
            /// <summary>
            /// 获取指定自定义格式化器
            /// </summary>
            /// <param name="formatType">格式化器类型（实现了ICustomFormatter接口）</param>
            /// <returns></returns>
            public object GetFormat(Type formatType)
            {
                if (formatType == typeof(ICustomFormatter))
                    return this;
                return System.Threading.Thread.CurrentThread.CurrentCulture.GetFormat(formatType);
            }
        }
    }
    class ParseDemo
    {
        public static void Main()
        {
            Console.WriteLine("{0:##.##}", 23.1234);
        }
    }
    class EncodingDemo
    {
        public static void Main()
        {
            String shu = "0123";
            Decoder decoder = Encoding.ASCII.GetDecoder();
            Encoder encoder = Encoding.ASCII.GetEncoder();
            Byte[] bytes = new Byte[1024];
            Char[] cs = shu.ToCharArray();
            Int32 count = encoder.GetBytes(cs, 0, cs.Length, bytes, 0, false);
            Console.WriteLine("ascii编码字节数:{0}, 字节为：{1}", count, BitConverter.ToString(bytes, 0, count));
            
        }
    }
    class Base64EncodingDemo
    {
        public static void Main()
        {
            //Byte[] bytes = new Byte[10];
            //new Random().NextBytes(bytes);
            //Console.WriteLine(BitConverter.ToString(bytes));//显示字节
            //Console.WriteLine(Convert.ToBase64String(bytes));
            using (System.Security.SecureString ss = new System.Security.SecureString())
            {
                Console.WriteLine("请输入密码：");
                while(true)
                {
                    ConsoleKeyInfo cki = Console.ReadKey(true);//无回显
                    if (cki.Key == ConsoleKey.Enter)
                        break;
                    ss.AppendChar(cki.KeyChar);
                    Console.Write("*");
                }
                Console.WriteLine();
                DisplaySecureString(ss);
                Console.WriteLine();
            }
        }
        private unsafe static void DisplaySecureString(System.Security.SecureString ss)
        {
            Char* pc = null;
            try
            {
                pc = (Char*)Marshal.SecureStringToCoTaskMemUnicode(ss);//复制到非托管的内存缓冲区
                Console.WriteLine("0x{0:x}", (Int32)pc);
                Console.ReadKey();
                for (Int32 index = 0; pc[index] != 0; index++)
                {
                    Console.Write(pc[index]);
                }
                
            }
            finally
            {
                if (pc != null)
                    Marshal.ZeroFreeCoTaskMemUnicode((IntPtr)pc);
            }
        }
    }
}
