using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadsApp
{
    class Program
    {
        public delegate int BinaryOp(int x, int y);

        static void Main(string[] args)
        {
            //GetBasicThreadInfo(); 

            Console.WriteLine("Main() thread invoked on {0}", Thread.CurrentThread.ManagedThreadId);

            BinaryOp operation = new BinaryOp(Add);
            int answer = operation(10, 5);

            Console.WriteLine("Doing more work in main thread");
            Console.ReadLine(); 
        }

        private static void GetBasicThreadInfo()
        {
            Thread thread = Thread.CurrentThread;
            AppDomain domain = Thread.GetDomain();
            Context context = Thread.CurrentContext;
        }

        private static int Add(int x, int y)
        {
            // Print out id of executing thread
            Console.WriteLine("Add() method invoked on thread {0}", Thread.CurrentThread.ManagedThreadId);

            Thread.Sleep(5000);

            return x + y; 
        }
    }
}
