using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace CoreFeature
{
    class ReflectionDemo
    {
        /// <summary>
        /// //理解错误：当解析一个嵌入的资源而加载失败的时候被调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">解析的事件</param>
        /// <returns></returns>
        private static Assembly ResolveEventHandler(Object sender, ResolveEventArgs e)
        {
            String dllName = new AssemblyName(e.Name) + ".dll";
            var assem = Assembly.GetExecutingAssembly();
            String resoucesName = assem.GetManifestResourceNames().FirstOrDefault(
                rn => rn.EndsWith(dllName));
            if (resoucesName == null)
                return null;//不是嵌入的资源
            using (var stream = assem.GetManifestResourceStream(resoucesName))
            {
                Byte[] assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
        public static void ShowExceptions()
        {
            LoadAssemblies();
            var allTypes =
                (from a in AppDomain.CurrentDomain.GetAssemblies()
                 from t in a.ExportedTypes
                 where typeof(Exception).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo())
                 orderby t.Name
                 select t).ToArray();
            //生成并显示继承 层次结构
            Console.WriteLine(WalkInheritanceHierarchy(new StringBuilder(), 0, typeof(Exception), allTypes));

        }
        private static StringBuilder WalkInheritanceHierarchy(StringBuilder sb, 
            Int32 indent, Type baseType, IEnumerable<Type> allTypes)
        {
            String space = new String(' ', indent * 3);
            sb.AppendLine(space + baseType.FullName);
            foreach(var t in allTypes)
            {
                if (t.GetTypeInfo().BaseType != baseType)
                    continue;//必须按照继承的顺序。当是直接继承的baseType时才打印信息，然后遍历子类
                WalkInheritanceHierarchy(sb, indent + 1, t, allTypes);
            }
            return sb;
        }
        public static void LoadAssemblies()
        {
            String[] assemblies =
            {
                "System, PublicKeyToken={0}"
            };
            String EcmaPublicKeyToken = "b77a5c561934e089";
            String MSPublickKeyToken = "b03f5f7f11d50a3a";
            //获取包含System.Object的程序集版本
            Version version = typeof(System.Object).Assembly.GetName().Version;
            //显示加载需要反射的程序集
            foreach(String a in assemblies)
            {
                String assId = String.Format(a, EcmaPublicKeyToken, MSPublickKeyToken) +
                    ", Culture=neutral, Version=" + version;
                Assembly.Load(assId);
            }
        }
        public static void Main()
        {
            //AppDomain.CurrentDomain.ResourceResolve += new System.ResolveEventHandler(ResolveEventHandler);
            //var ass = Assembly.ReflectionOnlyLoadFrom(@"D:\GitHub 开源项目&小工具\C#运行时间统计\CodeTime\CodeTime\bin\Debug\CodeTime.dll");
            //var s = Assembly.ReflectionOnlyLoad("CodeTime");
            //Console.WriteLine(s.ToString());
            //直接从字符串返回一个类型
            ShowExceptions();
        }
    }
}

