using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace BasicType.DelegateDemo
{
    class DelegateDemo
    {

    }
    namespace MultiDelegateDemo
    {
        internal sealed class Light
        {
            public String SwitchPosition()
            {
                return "灯泡已经熄灭";
            }
        }
        internal sealed class Fan
        {
            public String Speed()
            {
                throw new InvalidOperationException("风扇坏了");
            }
        }
        internal sealed class Speaker
        {
            public String Volume()
            {
                return "音量最大";
            }
        }
        internal sealed class DateRecorder
        {
            ~DateRecorder()
            {
                Console.WriteLine("Dispose");
            }

            public void Record(Object date)
            {
                DateTime dateTime = (DateTime)date;
                Console.WriteLine("{0:D}", dateTime);
            }
        }
        public class Program
        {
            private delegate String GetStatus();
            private delegate void WorkItem(Object date);
            public static void Main()
            {
                //GetStatus getStatus = null;
                //getStatus += new GetStatus(new Light().SwitchPosition);
                //getStatus += new GetStatus(new Fan().Speed);
                //getStatus += new GetStatus(new Speaker().Volume);
                //Console.WriteLine(GetComponentStatus(getStatus));
                WorkItem printDate = null;
                printDate = new WorkItem(new DateRecorder().Record);
                ThreadPool.QueueUserWorkItem(new WaitCallback(printDate), DateTime.Now);
                Thread.Sleep(1000);
                
            }
            private static String GetComponentStatus(GetStatus status)
            {
                if (status == null)
                    return null;
                StringBuilder sb = new StringBuilder();
                Delegate[] reporters = status.GetInvocationList();//获取委托的列表
                foreach (GetStatus op in reporters)//从Delegate转型为GetStatus
                {
                    try
                    {
                        sb.AppendFormat("{0}{1}{1}", op(), Environment.NewLine);
                    }
                    catch(Exception e)
                    {
                        Object component = op.Target;
                        sb.AppendFormat("Failed to get status: from {1}{2}{0}\tError:{3}{0}",
                            Environment.NewLine,
                            ((component == null) ? "" : component.GetType() + "."),
                            op.Method.Name,
                            e.Message);
                    }
                }
                return sb.ToString();
            }
        }
    }
}
