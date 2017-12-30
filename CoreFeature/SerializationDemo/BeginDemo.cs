using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace CoreFeature.SerializationDemo
{
    class BeginDemo
    {
        /// <summary>
        /// 序列化对象到内存流
        /// </summary>
        /// <param name="objectGraph">对象图</param>
        /// <returns></returns>
        private static MemoryStream SerializeToMemory(Object objectGraph)
        {
            //构造一个流来容纳序列化对象
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            //告诉格式化器将对象序列化到流中
            formatter.Serialize(stream, objectGraph);
            return stream;
        }
        /// <summary>
        /// 从流中反序列化为对象
        /// </summary>
        /// <param name="stream">流的实例</param>
        /// <returns></returns>
        private static Object DeserializeFromMemory(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }
        private static void Begin2()
        {
            var objGraph1 = new List<Int32> { 14,15,16 };
            var objGraph2 = new List<String> { "Wang", "Xing", "Niu" };
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, objGraph1);
            formatter.Serialize(stream, objGraph2);
            //重新定位
            stream.Seek(0, SeekOrigin.Begin);
            var dobj1 = (List<String>)formatter.Deserialize(stream);
            var dobj2 = (List<Int32>)formatter.Deserialize(stream);
            dobj1.ForEach(s => Console.WriteLine(s));
            dobj2.ForEach(s => Console.WriteLine(s));
        }
        public static void Main()
        {
           // Begin2();//注意序列化和反序列化的顺序
            var objGraph = new List<String> { "Shu", "Jilong", "Jinyu" };
            
            Stream stream = SerializeToMemory(objGraph);
            //重写设置流的位置
            stream.Seek(0, SeekOrigin.Begin);
            objGraph = null;
            objGraph = (List<String>)DeserializeFromMemory(stream);
            foreach(var ming in objGraph)
            {
                Console.WriteLine("{0}", ming);
            }
        }
    }
}
