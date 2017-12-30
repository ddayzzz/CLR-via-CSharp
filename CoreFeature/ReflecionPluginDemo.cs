using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
namespace CoreFeature
{
    namespace ReflecionPluginDemo
    {
        public interface IAddIn
        {
            String DoSomething(Object obj);
        }
        public class Test
        {
            public static void Main()
            {
                //可执行文件的目录
                String addInDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                //在目录中寻找插件
                var addInAssemblies = Directory.EnumerateFiles(addInDir, "*.dll");
                var addInTypes =
                    from file in addInAssemblies
                    let assembly = Assembly.LoadFile(file)
                    from t in assembly.ExportedTypes//公开的导出类型
                                                    //只要是个实现了接口
                where t.IsClass && typeof(ReflecionPluginDemo.IAddIn).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo())
                    select t;//投影为这个类型
                foreach (Type type in addInTypes)
                {
                    ReflecionPluginDemo.IAddIn iadd = (ReflecionPluginDemo.IAddIn)Activator.CreateInstance(type);
                    Console.WriteLine(iadd.DoSomething(null));
                }
            }
        }
    }
    
}
