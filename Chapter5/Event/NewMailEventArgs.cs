using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Event
{
    public class  NewMailEventArgs:EventArgs
    {
        private readonly String m_from;
        private readonly String m_to;
        private readonly String m_subject;

        public NewMailEventArgs(String from, String to, String subject)
        {
            m_from = from;
            m_to = to;
            m_subject = subject;
        }

        public String From
        {
            get => m_from;
        }

        public String To
        {
            get => m_to;
        }

        public String Subject
        {
            get => m_subject;
        }

        
    }
}
