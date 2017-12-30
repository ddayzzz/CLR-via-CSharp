using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
namespace CoreFeature.SerializationDemo
{
    /// <summary>
    /// 用于序列化代理
    /// </summary>
    namespace CH24_8_SurrogateDemo
    {
        internal class UTCTimeSerializationSurrogate : ISerializationSurrogate
        {
            public void GetObjectData(Object obj, SerializationInfo info, StreamingContext context)
            {
                info.AddValue("Date", ((DateTime)obj).ToUniversalTime().ToString("u"));
            }

            public Object SetObjectData(Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                return DateTime.ParseExact(info.GetString("Date"), "u", null).ToLocalTime();
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
                    //构造一个代理对象
                    SurrogateSelector ss = new SurrogateSelector();
                    //为类型使用的代理:可以登记多个代理对象
                    ss.AddSurrogate(typeof(DateTime), formatter.Context, new UTCTimeSerializationSurrogate());
                    formatter.SurrogateSelector = ss;
                    //创建一个类型，代表本地时间
                    DateTime localTime = DateTime.Now;
                    formatter.Serialize(stream, localTime);
                    //显示已经序列化的UTC时间格式
                    stream.Position = 0;
                    Console.WriteLine(new StreamReader(stream).ReadToEnd());
                    //反序列化
                    stream.Position = 0;
                    DateTime localTimeAfter = (DateTime)formatter.Deserialize(stream);
                    Console.WriteLine("Before:{0}", localTime);
                    Console.WriteLine("After:{0}", localTimeAfter);
                    //循环引用的对象
                    
                }
            }
        }
    }
}
