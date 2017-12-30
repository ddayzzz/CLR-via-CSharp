using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
//这个文件时23.5.1 发现类型成员部分
namespace CoreFeature
{
    class MemberFindDemo
    {
        private static void Show(Int32 indent, String format, params Object[] args)
        {
            Console.WriteLine(new String(' ', 3 * indent) + format, args);
        }
        public static void Main()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach(Assembly a in assemblies)
            {
                Show(0, "程序集：{0}", a);
                foreach(Type t in a.ExportedTypes)
                {
                    
                    Show(1, "公开的类型：{0}", t);
                    foreach(MemberInfo mi in t.GetTypeInfo().DeclaredMembers)
                    {
                        String typeName = String.Empty;
                        if (mi is Type) typeName = "嵌套的类型";
                        else if (mi is FieldInfo)
                            typeName = "字段";
                        else if (mi is MethodInfo)
                            typeName = "方法";
                        else if (mi is ConstructorInfo)
                            typeName = "构造函数";
                        else if (mi is PropertyInfo)
                            typeName = "属性";
                        else if (mi is EventInfo)
                            typeName = "事件定义";
                        Show(2, "{0}:{1}", typeName, mi);
                    }
                }
            }
        }
    }
}
