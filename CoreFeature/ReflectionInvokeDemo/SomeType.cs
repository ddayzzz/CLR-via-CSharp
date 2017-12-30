using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFeature.ReflectionInvokeDemo
{
    internal sealed class SomeType
    {
        private Int32 m_someField;
        public SomeType(ref Int32 x)
        {
            x *= 2;
        }
        public override string ToString()
        {
            return m_someField.ToString();
        }
        public Int32 SomeProp
        {
            get
            {
                return m_someField;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");
                m_someField = value;
            }
        }
        public event EventHandler SomeEvent;
        private void NoCompilerWarnings() { SomeEvent.ToString(); }
    }
}
