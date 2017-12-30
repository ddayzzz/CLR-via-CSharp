using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ThreadingDemo
{
    class ThreadPoolContext
    {
        public static void Main()
        {
            //设置上下文的数据
            CallContext.LogicalSetData("Name", "Shu");
            ThreadPool.QueueUserWorkItem(
                state => Console.WriteLine("Name={0}", CallContext.LogicalGetData("Name")));
            //阻止Main线程的执行上下文的流动
            ExecutionContext.SuppressFlow();
            ThreadPool.QueueUserWorkItem(
                state => Console.WriteLine("Name={0}", CallContext.LogicalGetData("Name")));
            //恢复上下文的流动
            ExecutionContext.RestoreFlow();
           
        }
    }
    class CancellationDemo
    {
        public static void Main()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            //登记一个被取消时候的方法
            tokenSource.Token.Register(() => Console.WriteLine("操作被取消了！msg1"));
            tokenSource.Token.Register(() => Console.WriteLine("操作被取消了！msg2"));
            ThreadPool.QueueUserWorkItem(o => Count(tokenSource.Token, 1000));
            Console.WriteLine("按任意键退出");
            Console.ReadKey();
            tokenSource.Cancel();
            Console.ReadKey();
        }
        private static void Count(CancellationToken token, Int32 countTo)
        {
            for(Int32 count=0;count < countTo; ++count)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Count is cancelled");
                    break;
                }
                Console.WriteLine(count);
                Thread.Sleep(200);
            }
            Console.WriteLine("Count is done");

        }

    }
    class InnerExceptionDemo
    {
        private static Int32 Sum(CancellationToken ct, Int32 n)
        {
            Int32 s = 0;
            for (; n > 0; --n)
            {
                ct.ThrowIfCancellationRequested();
                checked
                {
                    s += n;
                }
            }
            return s;
        }
        public static void ExpDemo()
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                //TaskScheduler.UnobservedTaskException += new EventHandler<UnobservedTaskExceptionEventArgs>((Object sender,
                //     UnobservedTaskExceptionEventArgs e) =>//不知道是干嘛的对应CLR via的621页
                //{ Console.WriteLine("发生了异常"); });
                Task<Int32> t = Task.Run(() => Sum(source.Token, 100000), source.Token);
                source.Cancel();
                t.Wait();
                Console.WriteLine("Sum is {0}", t.Result);
                //如果始终没有调用Wait方法或者使用Result的属性，不会发生错误
                //Int32 g= t.Result;
                GC.Collect();

            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.Message);
                ex.Handle(e => e is OperationCanceledException);
            }
        }
        public static void ContinueDemo()
        {
            Task<Int32> t = Task.Run(() => { Console.WriteLine(Thread.CurrentThread.Name); return Sum(CancellationToken.None, 1000); });
            t.ContinueWith(task => { Console.WriteLine(Thread.CurrentThread.Name); Console.WriteLine("Sum is " + task.Result); });

        }
        public static void Main()
        {
            ContinueDemo();
            Console.ReadKey();
        }
    }
}
