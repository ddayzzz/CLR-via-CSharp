using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

namespace CoreFeature.SerializationDemo
{
    /// <summary>
    /// 这个类用来展示单例的序列化
    /// </summary>
    namespace CH24_7_SingletonSerializationDemo
    {
        [Serializable]
        public sealed class Singleton:ISerializable
        {
            //类型的唯一实例
            private static readonly Singleton s_theOneObj = new Singleton();
            //实例字段
            public String Name = "Shu";
            public DateTime Date = DateTime.Now;
            //私有构造器
            private Singleton() { }
            public static Singleton GetSingleton() { return s_theOneObj; }
            //使用显示接口实现
            [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.SetType(typeof(SingetonSerializationHelper));
            }
            [Serializable]
            private sealed class SingetonSerializationHelper : IObjectReference//https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iobjectreference?view=netframework-4.7.1
            {
                public Object GetRealObject(StreamingContext context)
                {
                    return Singleton.GetSingleton();
                }
            }
            //不需要特殊的构造器，不会在反序列化时使用。因为使用的是代理的类型。而且这两个代理的类型还都是一个，所以实际上
            //只会反序列化一个
        }
        public class Test
        {
            public static void Main()
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                Singleton[] a1 = new Singleton[]
                {
                    Singleton.GetSingleton(),
                    Singleton.GetSingleton()
                };
                using (stream)
                {
                    formatter.Serialize(stream, a1);
                    stream.Position = 0;
                    Singleton[] a2 = (Singleton[])formatter.Deserialize(stream);
                    Console.WriteLine("反序列化后：a1[0]==a2[0]:{0}", a1[0] == a2[0]);
                    Console.WriteLine("反序列化后：a1[1]==a2[1]:{0}", a1[1] == a2[1]);
                }
            }
        }
    }
}
