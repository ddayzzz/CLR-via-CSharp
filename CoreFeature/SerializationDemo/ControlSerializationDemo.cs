using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security;

namespace CoreFeature.SerializationDemo
{
    [Serializable]
    public sealed class Circle
    {
        private Double m_radius;//半径
        [NonSerialized]
        private Double m_area;

        public Circle(Double radius)
        {
            this.m_radius = radius;
            m_area = Math.PI * m_radius * m_radius;
        }
        public override String ToString()
        {
            return String.Format("半径：{0}，面积：{1}", m_radius, m_area);
        }
        //area不可序列化，所以反序列化会丢失信息
        //解决方法
        [OnDeserialized]
        private void SetArea(StreamingContext context)
        {
            m_area = Math.PI * m_radius * m_radius;
        }
    }
    class ControlSerializationDemo
    {
        public static void Main()
        {
            Circle c1 = new Circle(10.0);
            Console.WriteLine(c1);
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, c1);
            stream.Seek(0, SeekOrigin.Begin);
            Circle c2 = formatter.Deserialize(stream) as Circle;
            Console.WriteLine(c2);
        }
    }
    
}
