using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Event
{
    class MailManager
    {
        public event EventHandler<NewMailEventArgs> NewMail;
        protected virtual void OnNewMail(NewMailEventArgs e)
        {
            e.Raise(this, ref NewMail);
        }
        public void SimulateNewMail(String from, String to, String subject)
        {
            NewMailEventArgs e = new NewMailEventArgs(from, to, subject);
            OnNewMail(e);
        }
    }
}
