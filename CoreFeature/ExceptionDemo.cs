using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;

namespace CoreFeature
{
    class ExceptionDemo
    {
        public static void Func()
        {
            try
            {
                throw new Exception("exception in func try");
            }
            catch
            {
                //throw new Exception("exception in fun catch");
                throw;
            }
            finally
            {
                Console.WriteLine("Func finally");
            }
        }
        public static void Main()
        {
            try
            {
                Func();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                /*输出
                 func finally 还是会被调用
                 */
                Console.WriteLine("main finally");
            }
        }
    }
    namespace MyExceptionDemo
    {
        [Serializable]
        public sealed class Exception<TException>: Exception, ISerializable
            where TException : ExceptionArgs
        {
            private const String c_args = "Args";//用于反序列化
            private readonly TException m_args;//消息的对象
            public TException Args { get { return m_args; } }
            public Exception(TException args, String message=null,
                Exception innerException=null):base(message, innerException)
            {

            }
            public Exception(String message=null, Exception innerException=null):this(null, message, innerException)
            {

            }
            //用于反序列化
            [SecurityPermission(SecurityAction.LinkDemand,
                Flags =SecurityPermissionFlag.SerializationFormatter)]
            private Exception(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                m_args = (TException)info.GetValue(c_args, typeof(ExceptionArgs));
            }
            //用于序列化
            [SecurityPermission(SecurityAction.LinkDemand,
                Flags = SecurityPermissionFlag.SerializationFormatter)]
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(c_args, m_args);
                base.GetObjectData(info, context);
            }
            public override string Message
            {
                get
                {
                    String baseStr = base.Message;
                    return (m_args == null) ? baseStr : baseStr + " (" + m_args.Message + ")";
                }
            }
            public override bool Equals(object obj)
            {
                Exception<TException> other = obj as Exception<TException>;
                if (other == null)
                    return false;
                return Object.Equals(m_args, other.m_args) && base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
        [Serializable]
        public abstract class ExceptionArgs
        {
            public virtual String Message { get { return String.Empty; } }
        }
        [Serializable]
        public class DiskFullExceptionArgs : ExceptionArgs { }
        public class Test
        {
            public static void Main()
            {
                //try
                //{
                throw new Exception<DiskFullExceptionArgs>(new DiskFullExceptionArgs(),
                        "C盘已满");
                using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope())
                {
                    
                }

                //}
                //catch(Exception e)
                //{
                //    Console.WriteLine(e.Message);
                //}
            }
        }
    }
    namespace ReflectionExceptionDemo
    {
        public class ReflectionExpception
        {
            private static void Reflection(Object o)
            {
                try
                {
                    var mi = o.GetType().GetMethod("DoSomething");
                    mi.Invoke(o, null);
                }
                catch (System.Reflection.TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            }
            public static void DoSomething()
            {
                //Console.WriteLine("DoSomething called");
                throw new InvalidOperationException("不能调用！");
            }
            public static void Main()
            {
                try
                {
                    Debug.Assert(1 == 3, "Naive");
                    ReflectionExpception.Reflection(new ReflectionExpception());
                    
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }
    }
    namespace CERDemo
    {
        class CERDemo
        {
            private static void Main()
            {
                System.Runtime.CompilerServices.RuntimeHelpers.PrepareConstrainedRegions();//接下来finally的快中代码需要准备好
                try
                {
                    Console.WriteLine("In try");
                }
                finally
                {
                    Type1.M();
                    //var mi = typeof(Type1).GetMethod("M");
                    //mi.Invoke(null, null);
                }
            }
        }
        sealed class Type1
        {
            private delegate Double Operation(Double d);
            private static Operation opt = (Double e)=>
            {
                Environment.FailFast("Manunally terminated");
                return 0.0;
            };
            static Type1()
            {
                System.Runtime.CompilerServices.RuntimeHelpers.PrepareDelegate(opt);
                Console.WriteLine("Type1's static ctor called");
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            public static void M()
            {
                
                //如果静态构造出了偏差，M就可能得不到调用
                
                Console.WriteLine(opt(5));
            }
        }
    }
}
