using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;

namespace BasicType.FeatureDemo
{
    namespace APIUse
    {
        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
        internal sealed class OSVERSIONINFO
        {
            public OSVERSIONINFO()
            {
                OSVersionInfoSize = (UInt32)Marshal.SizeOf(this);
            }
            public UInt32 OSVersionInfoSize = 0;
            public UInt32 MajorVersion = 0;
            public UInt32 MinorVersion = 0;
            public UInt32 BuildNumber = 0;
            public UInt32 PlatformId = 0;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public String CSDVersion = null;
        }
        internal sealed class MyClass
        {
            [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern Boolean GetVersionEx([In, Out] OSVERSIONINFO ver);
            public static void Main()
            {
                //OSVERSIONINFO os;
                //GetVersionEx(os);
            }
        }
    }
    namespace AttrsDemo
    {
        [Serializable]
        [DefaultMemberAttribute("Main")]
        [DebuggerDisplayAttribute("Richter", Name ="Jeff", Target =typeof(Program))]
        public sealed class Program
        {
            [Conditional("Debug")]
            [Conditional("Release")]
            public void DoSomething() { }
            public Program()
            {

            }

            [CLSCompliant(true)]
            [STAThread]
            public static void Main()
            {
                ShowAttributes(typeof(Program));
                var members = from m in typeof(Program).GetTypeInfo().DeclaredMembers.OfType<MethodBase>()
                              where m.IsPublic
                              select m;//我只选择了方法
                foreach(MemberInfo m in members)
                {
                    ShowAttributes(m);
                }
            }
            /// <summary>
            /// 输出特性的值
            /// </summary>
            /// <param name="attributeTarget">特性被应用的目标</param>
            private static void ShowAttributes(MemberInfo attributeTarget)
            {
                var attributes = attributeTarget.GetCustomAttributes<Attribute>();
                Console.WriteLine("Attributes applied to {0}:{1}",
                    attributeTarget.Name, (attributes.Count() == 0 ? "None" : String.Empty));
                foreach(Attribute attribute in attributes)
                {
                    Console.WriteLine("\t{0}", attribute.GetType().ToString());
                    if (attribute is DefaultMemberAttribute)
                        Console.WriteLine("\tMemberName={0}", ((DefaultMemberAttribute)attribute).MemberName);
                    else if (attribute is ConditionalAttribute)
                        Console.WriteLine("\tConditionString={0}", ((ConditionalAttribute)attribute).ConditionString);
                    else if (attribute is CLSCompliantAttribute)
                        Console.WriteLine("\tIsCompliant={0}", ((CLSCompliantAttribute)attribute).IsCompliant);
                    DebuggerDisplayAttribute dda = attribute as DebuggerDisplayAttribute;
                    if (dda != null)
                        Console.WriteLine("\tValue={0}, Name={1}, Target={2}", dda.Value, dda.Name, dda.Target);

                }
                Console.WriteLine();
            }
        }
    }
    namespace AttrsMatchAndEqualsDemo
    {
        internal enum Accounts
        {
            Savings = 0x0001,
            Checking = 0x002,
            Brokerage = 0x004
        }
        [AttributeUsage(AttributeTargets.Class)]
        internal sealed class AccountAttribute : Attribute
        {
            private Accounts m_accounts;
            public AccountAttribute(Accounts accounts)
            {
                m_accounts = accounts;
            }
            /// <summary>
            /// 检查与obj的账户是否匹配
            /// </summary>
            /// <param name="obj">一个特性的实例</param>
            /// <returns></returns>
            public override bool Match(object obj)
            {
                if (obj == null)
                    return false;
                if (obj.GetType() != this.GetType())
                    return false;
                AccountAttribute other = (AccountAttribute)obj;
                if ((other.m_accounts & m_accounts) != m_accounts)
                    return false;
                return true;
            }
            /// <summary>
            /// 检查与obj的账户特性的字段是否等价
            /// </summary>
            /// <param name="obj">特性实例</param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (obj.GetType() != this.GetType())
                    return false;
                AccountAttribute other = (AccountAttribute)obj;
                if (other.m_accounts != m_accounts)
                    return false;
                return true;
            }
            /// <summary>
            /// 与Equals保持一致
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return (Int32)m_accounts;
            }
        }

        [Account(Accounts.Savings)]
        internal sealed class ChildAccount { }

        [Account(Accounts.Savings | Accounts.Checking | Accounts.Brokerage)]
        internal sealed class AdultAccount { }

        public class Test
        {
            public static void Main()
            {
                CanWriteCheck(new ChildAccount());
                CanWriteCheck(new AdultAccount());
                CanWriteCheck(new Test());
            }
            private static void CanWriteCheck(Object obj)
            {
                //显式查找的实例。检查时需要构造一个Attribute的派生类实例
                Attribute checking = new AccountAttribute(Accounts.Checking);
                Attribute validAccount = Attribute.GetCustomAttribute(obj.GetType(), typeof(AccountAttribute), false);
                if((validAccount != null) && checking.Match(validAccount))
                {
                    Console.WriteLine("{0} 类型可以写支票", obj.GetType());
               
                }
                else
                {
                    Console.WriteLine("{0} 类型不可以写支票", obj.GetType());
                }
            }
        }
    }
    /// <summary>
    /// 安全地检测特性，不会构造新的特性实例
    /// </summary>
    namespace AttrsSafelyDetectDemo
    {
        [Serializable]
        [DefaultMemberAttribute("Main")]
        [DebuggerDisplayAttribute("Richter", Name = "Jeff", Target = typeof(Program))]
        public sealed class Program
        {
            [Conditional("Debug")]
            [Conditional("Release")]
            public void DoSomething() { }
            public Program()
            {

            }

            [CLSCompliant(true)]
            [STAThread]
            public static void Main()
            {
                //Int32? bb = 5;
                //int b = ((IComparable)(Int32)bb).CompareTo(6);//转型为IComparable需要装箱
                ShowAttributes(typeof(Program));
                var members = from m in typeof(Program).GetTypeInfo().DeclaredMembers.OfType<MethodBase>()
                              where m.IsPublic
                              select m;//我只选择了方法
                foreach (MemberInfo m in members)
                {
                    ShowAttributes(m);
                }
            }
            /// <summary>
            /// 输出特性的值
            /// </summary>
            /// <param name="attributeTarget">特性被应用的目标</param>
            private static void ShowAttributes(MemberInfo attributeTarget)
            {
                IList<CustomAttributeData> attributes = CustomAttributeData.GetCustomAttributes(attributeTarget);
                Console.WriteLine("Attributes applied to {0}:{1}",
                    attributeTarget.Name, (attributes.Count() == 0 ? "None" : String.Empty));
                foreach (CustomAttributeData attribute in attributes)
                {
                    Type t = attribute.Constructor.DeclaringType;//声明构造函数的类
                    Console.WriteLine("\t{0}", t.ToString());
                    Console.WriteLine("\tConstructor called={0}", attribute.Constructor);
                    IList<CustomAttributeTypedArgument> posArgs = attribute.ConstructorArguments;
                    Console.WriteLine("\tPositional arguments passed to Constructor:" +
                        ((posArgs.Count == 0) ? "None" : String.Empty));
                    foreach(CustomAttributeTypedArgument pa in posArgs)
                    {
                        Console.WriteLine("\t\tType={0}, Value={1}", pa.ArgumentType, pa.Value);
                    }
                    IList<CustomAttributeNamedArgument> nameArgs = attribute.NamedArguments;
                    Console.WriteLine("\tNamed arguments passed to Constructor:" +
                        ((nameArgs.Count == 0) ? "None" : String.Empty));
                    foreach (CustomAttributeNamedArgument na in nameArgs)
                    {
                        Console.WriteLine("\t\tType={0}, Value={1}", na.TypedValue.ArgumentType, na.TypedValue.Value);
                    }
                    Console.WriteLine();

                }
                Console.WriteLine();
            }
        }
    }
}
