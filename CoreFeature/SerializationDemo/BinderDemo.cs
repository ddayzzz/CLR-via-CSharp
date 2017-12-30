using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Reflection;
namespace CoreFeature.SerializationDemo
{
    namespace BinderDemo
    {
        [Serializable]
        internal sealed class Ver1
        {
            private String m_version = "Version1";
            public String Version { get { return m_version; } }
            public override String ToString()
            {
                return m_version;
            }
        }
        [Serializable]
        internal sealed class Ver2
        {
            private String m_version = "Version2";
            public String Version { get { return m_version; } }
            public override String ToString()
            {
                return m_version;
            }
        }
        internal sealed class Ver1ToVer2SerializationBinder: SerializationBinder
        {
            public override Type BindToType(String assemblyName, String typeName)
            {
                //将任何版本1的对象从版本1.0.0.0反序列化为Ver2对象
                AssemblyName assemblyVer1 = Assembly.GetExecutingAssembly().GetName();
                assemblyVer1.Version = new Version(1, 0, 0, 0);
                if (assemblyName == assemblyVer1.ToString() && typeName.Equals("CoreFeature.SerializationDemo.BinderDemo.Ver1"))
                    return typeof(Ver2);//保持字段相等，但是值会被重新设置
                return Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));

            }
        }
        public class Test
        {
            public static void Main()
            {
                using (var stream = new MemoryStream())
                {
                    //构造所需的格式化器
                    IFormatter formatter = new SoapFormatter();
                    formatter.Binder = new Ver1ToVer2SerializationBinder();
                    Ver1 v1 = new Ver1();
                    formatter.Serialize(stream, v1);
                    stream.Position = 0;
                    Ver2 v2 = formatter.Deserialize(stream) as Ver2;
                    Console.WriteLine(v1);
                    Console.WriteLine(v2);
                }
            }
        }
    }
}
