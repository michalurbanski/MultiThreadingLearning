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
        private static Action action; 

        static void Main(string[] args)
        {
            //GetBasicThreadInfo(); 

            action = DelegatesExample;

            ExecuteAction(action);
        }

        private static void ExecuteAction(Action action)
        {
            Console.WriteLine("Main() thread invoked on {0}", Thread.CurrentThread.ManagedThreadId);

            action();

            Console.WriteLine("Doing more work in main thread");
            Console.ReadLine();
        }

        private static void GetBasicThreadInfo()
        {
            Thread thread = Thread.CurrentThread;
            AppDomain domain = Thread.GetDomain();
            Context context = Thread.CurrentContext;
        }

        /// <summary>
        /// Using this delegates approach main thread is blocked. Until Add action is finished main 
        /// thread is awaiting. 
        /// </summary>
        private static void DelegatesExample()
        {
            BinaryOp operation = new BinaryOp(Add);
            int answer = operation(10, 5);
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
