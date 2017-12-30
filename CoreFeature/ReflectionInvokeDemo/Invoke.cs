using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder;
namespace CoreFeature.ReflectionInvokeDemo
{
    class Invoke
    {
        /// <summary>
        /// Object直接绑定并调用
        /// </summary>
        /// <param name="t"></param>
        private static void BindToMemberThenInvokeTheMember(Type t)
        {
            Type ctorArg = Type.GetType("System.Int32&");//BNF定义，创建类型的引用
            ConstructorInfo ctor = t.GetTypeInfo().DeclaredConstructors.First(
                c => c.GetParameters()[0].ParameterType == ctorArg);//满足构造函数的条件
            Object[] args = new Object[] { 12 };
            Console.WriteLine("x 在构造函数调用之前：{0}", args[0]);
            Object obj = ctor.Invoke(args);
            Console.WriteLine("x 在构造函数调用之后：{0}", args[0]);
            //读写字段
            FieldInfo fi = obj.GetType().GetTypeInfo().GetDeclaredField("m_someField");
            fi.SetValue(obj, 33);//obj需要指定，这是实例的字段
            Console.WriteLine("SomeField:{0}", fi.GetValue(obj));
            //读写属性
            PropertyInfo pi = obj.GetType().GetTypeInfo().GetDeclaredProperty("SomeProp");
            try
            {
                pi.SetValue(obj, 0, null);//这个参数不合法
            }
            catch(TargetInvocationException e)
            {
                if (e.InnerException.GetType() != typeof(ArgumentOutOfRangeException))
                    throw;
                Console.WriteLine("属性设置出错：{0}\nTraceBack:\n{1}\n", e.InnerException.Message, e.InnerException.StackTrace);//捕获内部错误。因为反射覆盖了错误的类型
            }
            pi.SetValue(obj, 2, null);
            Console.WriteLine("SomeProp:{0}", pi.GetValue(obj, null));
            //添加委托
            EventInfo ei = obj.GetType().GetTypeInfo().GetDeclaredEvent("SomeEvent");
            EventHandler eh = new EventHandler(EventCallBack);
            ei.AddEventHandler(obj, eh);
            //删除时Remove...

        }

        private static void EventCallBack(Object sender, EventArgs e)
        {

        }

        private static void BindToMemebrCreateDelegateToMemberThenInvokeTheMember(Type t)
        {
            //构造实例
            Object[] args = new Object[] { 12 };
            Console.WriteLine("参数在构造函数调用之前：{0}", args[0]);
            Object obj = Activator.CreateInstance(t, args);
            Console.WriteLine("类型：" + obj.GetType().ToString());
            Console.WriteLine("参数在构造函数调用之后：{0}", args[0]);
            //不能创建对字段的委托
            TypeInfo objInfo = obj.GetType().GetTypeInfo();
            MethodInfo mi = objInfo.GetDeclaredMethod("ToString");
            var toString = mi.CreateDelegate(typeof(Func<String>), obj);
            Console.WriteLine("ToString() : " + toString.DynamicInvoke());
            //读写属性
            var pi = objInfo.GetDeclaredProperty("SomeProp");
            var setDele = pi.SetMethod.CreateDelegate(typeof(Action<Int32>), obj);//访问属性，使用Get
            try
            {
                setDele.DynamicInvoke(1);
            }
            catch(ArgumentOutOfRangeException e)
            {
                Console.WriteLine("设置属性的错误：" + e.Message);//直接抛出属性设置其抛出的错误
            }
            setDele.DynamicInvoke(50);
            Console.WriteLine("ToString() : " + toString.DynamicInvoke());
            //读写事件差不多 ，引用AddMethod即可
        }
        private static void UseDynamicObject(Type t)
        {
            //构造实例
            Object[] args = new Object[] { 12 };
            Console.WriteLine("参数在构造函数调用之前：{0}", args[0]);
            dynamic obj = Activator.CreateInstance(t, args);
            Console.WriteLine("类型：" + obj.GetType().ToString());
            Console.WriteLine("参数在构造函数调用之后：{0}", args[0]);
            //读写字段
            try
            {
                obj.m_someField = 5;//尝试访问私有字段
                Int32 v = (Int32)obj.m_someField;
                Console.WriteLine("SomeField:" + v);

            }
            catch(RuntimeBinderException e)
            {
                Console.WriteLine("不能访问字段：" + e.Message);
            }
            //方法调用
            Console.WriteLine("ToString() : " + obj.ToString());
            //访问其他的字段都差不多
        }
        public static void Main()
        {
            Type t = typeof(SomeType);
            Console.WriteLine("Object直接绑定并调用:");
            BindToMemberThenInvokeTheMember(t);
            Console.WriteLine("创建委托引用对象和成员:");
            BindToMemebrCreateDelegateToMemberThenInvokeTheMember(t);
            Console.WriteLine("使用动态类型：");
            UseDynamicObject(t);
        }
    }
}
