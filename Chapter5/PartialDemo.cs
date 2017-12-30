using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic
{
    partial class Base
    {
        private String m_name;
        partial void OnNameChanging(String value);
        public String Name
        {
            get { return m_name; }
            set
            {
                OnNameChanging(m_name);
                m_name = value;
            }
        }
    }
    partial class Base
    {
        partial void OnNameChanging(string value)
        {
            Console.WriteLine("Change the name");
        }
    }
    class PartialDemo
    {
        public static void Main()
        {
            Base b = new Base { Name = "Shu" };
            b.Name = "ABC";
        }
        
    }
}
