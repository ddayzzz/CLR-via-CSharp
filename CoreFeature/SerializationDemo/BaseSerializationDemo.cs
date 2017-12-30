using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace CoreFeature.SerializationDemo.BaseSerializationDemo
{
    [Serializable]
    internal class Base
    {
        protected String m_name = "Jeff";
        public Base() { }
    }
    [Serializable]
    internal class Derived:Base, ISerializable
    {
        private DateTime m_date = DateTime.Now;
        public Derived() { }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter =true)]
        protected Derived(SerializationInfo info, StreamingContext context)
        {
            Type baseType = this.GetType().BaseType;
            //为我们的类和基类提供可序列化的成员集合
            MemberInfo[] mi = FormatterServices.GetSerializableMembers(baseType, context);
            //从info对象反序列化基类字段
            for(Int32 i=0;i<mi.Length;++i)
            {
                //获取字段
                FieldInfo fi = (FieldInfo)mi[i];
                fi.SetValue(this, info.GetValue(baseType.FullName + "+" + fi.Name, fi.FieldType));
            }
            //反序列化为这个类型的序列化的值
            m_date = info.GetDateTime("Date");
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //为这个序列化希望的值
            info.AddValue("Date", m_date);
            //获取我们的类和基类的可序列化的成员
            Type baseType = this.GetType().BaseType;
            MemberInfo[] mi = FormatterServices.GetSerializableMembers(baseType, context);
            for (Int32 i = 0; i < mi.Length; ++i)
                info.AddValue(baseType.FullName + "+" + mi[i].Name, ((FieldInfo)mi[i]).GetValue(this));//将基类的序列化成员写入
        }
        public override String ToString()
        {
            return String.Format("Name={0}, Date={1}", m_name, m_date);
        }
    }
    public class Test
    {
        public static void Main()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            Base b1 = new Base();
            Base b2 = new Derived();
            formatter.Serialize(stream, b1);
            formatter.Serialize(stream, b2);
            stream.Position = 0;
            Console.WriteLine("反序列化后：{0}", (Base)formatter.Deserialize(stream));
            Console.WriteLine("反序列化后：{0}", (Base)formatter.Deserialize(stream));
        }
    }
}
