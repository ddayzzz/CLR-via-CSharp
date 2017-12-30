using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace ThreadingDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("主线程：开进行异步操作");
            Thread thread = new Thread(ComputeBoundOp);
            thread.Start(5);

        }
        private static void ComputeBoundOp(Object state)
        {
            Console.WriteLine("ComputeBpundOp函数：状态={0}", state);
            Thread.Sleep(1000);
        }
    }
}
