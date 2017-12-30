using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//直接定义在程序集中
/// <summary>
/// 这个类的实例可跨越程序域边界按引用封送
/// </summary>
public sealed class MarshalByRefType : MarshalByRefObject
{
    public MarshalByRefType()
    {
        Console.WriteLine("{0} 构造函数在 程序域{1}中运行", this.GetType().ToString(),
            Thread.GetDomain().FriendlyName);
    }
    public void SomeMethod()
    {
        Console.WriteLine("{0}的SomeMethod在程序域{1}中运行", this.GetType().ToString(),
            Thread.GetDomain().FriendlyName);
    }
    public MarshalByValType MethodWithReturn()
    {
        Console.WriteLine("{0}的MethodWithReturn在程序域{1}中运行", this.GetType().ToString(),
            Thread.GetDomain().FriendlyName);
        return new MarshalByValType();
    }
    public NonMarshalableType MethodArgAndReturn(String callDomainName)
    {
        Console.WriteLine("调用{1}，来自{0}", callDomainName, Thread.GetDomain().FriendlyName);
        return new NonMarshalableType();
    }
    /// <summary>
    /// 无法通过对象直接访问
    /// </summary>
    public static void StaticMethod()
    {
        Console.WriteLine("在静态方法：StaticMethod，来自{0}", Thread.GetDomain().FriendlyName);
    }
}
/// <summary>
/// 该类型的实例可以跨越AppDomain边界的按值封送
/// </summary>
[Serializable]
public sealed class MarshalByValType : Object
{
    private DateTime m_createTime = DateTime.Now;//DateTime可以序列化
    public MarshalByValType()
    {
        Console.WriteLine("{0} 构造函数在 程序域{1}中运行", this.GetType().ToString(),
            Thread.GetDomain().FriendlyName);
    }
    public override string ToString()
    {
        return m_createTime.ToLongDateString();
    }
}
/// <summary>
/// 这个类型不能跨AppDomain边界封送
/// </summary>
//[Serializable] 未标记可序列化
public sealed class NonMarshalableType : Object
{
    public NonMarshalableType()
    {
        Console.WriteLine("{0} 构造函数在 程序域{1}中运行", this.GetType().ToString(),
            Thread.GetDomain().FriendlyName);
    }
}
public class AppDomainDemo
{

    public static void MarshallingDemo1()
    {
        AppDomain adCallingThreadDomain = Thread.GetDomain();
        //AppDomain.Unload(adCallingThreadDomain); 卸载当前的程序域，直接抛出CannotUnloadAppDomainException
        String name = adCallingThreadDomain.FriendlyName;
        Console.WriteLine("当前程序域的友好名称：{0}", name);
        String exeAssembly = Assembly.GetEntryAssembly().FullName;
        Console.WriteLine("包好Main的程序集名称：{0}", exeAssembly);
        //创建一个新的AppDomain
        AppDomain ad2 = AppDomain.CreateDomain("AD #2", null, null);
        MarshalByRefType mbrt = null;//封送
        mbrt = (MarshalByRefType)ad2.CreateInstanceAndUnwrap(exeAssembly, "MarshalByRefType");
        Console.WriteLine("对象的类型：{0}", mbrt.GetType());
        Console.WriteLine("是否是代理？{0}", RemotingServices.IsTransparentProxy(mbrt));
        AppDomain.Unload(ad2);
        //mbrt.SomeMethod();//尝试访问已卸载的程序域异常
    }
    //按值封送
    public static void MarshallingDemo2()
    {
        AppDomain adCallingThreadDomain = Thread.GetDomain();
        String name = adCallingThreadDomain.FriendlyName;
        Console.WriteLine("当前程序域的友好名称：{0}", name);
        String exeAssembly = Assembly.GetEntryAssembly().FullName;
        Console.WriteLine("包好Main的程序集名称：{0}", exeAssembly);
        //创建一个新的AppDomain
        AppDomain ad2 = AppDomain.CreateDomain("AD #3", null, null);
        MarshalByRefType mbrt = null;//封送
        mbrt = (MarshalByRefType)ad2.CreateInstanceAndUnwrap(exeAssembly, "MarshalByRefType");
        MarshalByValType mbvt = mbrt.MethodWithReturn();
        Console.WriteLine("对象的类型：{0}", mbvt.GetType());
        Console.WriteLine("是否是代理？{0}", RemotingServices.IsTransparentProxy(mbvt));
        Console.WriteLine("对象创建于：{0}", mbvt.ToString());
        AppDomain.Unload(ad2);
        Console.WriteLine("卸载程序域，按值封送没有影响：{0}", mbvt.ToString());
    }
    public static void Main()
    {
        MarshallingDemo1();
    }
}
/// <summary>
/// 用于展示检查作用域的变化
/// </summary>
public sealed class AppDomainMonitorDelta : IDisposable
{
    private AppDomain m_appDomain;//保存程序域
    private TimeSpan m_thisADCpu;
    private Int64 m_thisADMemoryInUse;
    private Int64 m_thisADMemoryAllocated;
    static AppDomainMonitorDelta()
    {
        AppDomain.MonitoringIsEnabled = true;
    }
    public AppDomainMonitorDelta(AppDomain AD)
    {
        m_appDomain = AD ?? AppDomain.CurrentDomain;
        m_thisADCpu = m_appDomain.MonitoringTotalProcessorTime;//CPU占用率
        m_thisADMemoryAllocated = m_appDomain.MonitoringTotalAllocatedMemorySize;//上一次垃圾回收之后的分配的字节数
        m_thisADMemoryInUse = m_appDomain.MonitoringSurvivedMemorySize;//上一次垃圾回收之后正在使用字节数
    }
    public void Dispose()
    {
        GC.Collect();
        Console.WriteLine("程序域友好名称：{0}，CPU使用时间={1}ms",
            m_appDomain.FriendlyName,
            (m_appDomain.MonitoringTotalProcessorTime - m_thisADCpu).TotalMilliseconds);
        Console.WriteLine("分配了{0:N0} 字节，其中{1:N0} 字节保留下来",
            (m_appDomain.MonitoringTotalAllocatedMemorySize - m_thisADMemoryAllocated),
            (m_appDomain.MonitoringSurvivedMemorySize - m_thisADMemoryInUse));

    }
}

namespace CoreFeature
{
    public class AppDomainMonitorDemo
    {
        public static void Main()
        {
            using (var apm = new AppDomainMonitorDelta(null))
            {
                List<Int32> lists = new List<int>();
                for (Int32 x = 0; x < 10000000; ++x)
                    lists.Add(x);

            }
        }
    }
}

//AppDomainManager 的Payload注入 https://yq.aliyun.com/articles/217291