using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadsApp
{
    class Program
    {
        public delegate int BinaryOp(int x, int y);
        private static Action action;
        private static bool isDone; // This one is not a thread safe

        static void Main(string[] args)
        {
            //GetBasicThreadInfo(); 

            //action = DelegatesExample;
            //action = AsynchronousDelegatesExample;
            action = AsyncCallbackDelegateExample;

            ExecuteAction(action);
        }

        private static void ExecuteAction(Action action)
        {
            Console.WriteLine("Main() thread invoked on {0}", Thread.CurrentThread.ManagedThreadId);

            action();

            Console.WriteLine("Finishing in main thread");
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

            Console.WriteLine("Answer is {0}", answer);
        }

        /// <summary>
        /// Using this approach delegate is invoked in a different thread. But still main thread
        /// is awaiting for delegate to finish its work. 
        /// </summary>
        private static void AsynchronousDelegatesExample()
        {
            BinaryOp operation = new BinaryOp(Add);
            IAsyncResult result = operation.BeginInvoke(10, 5, null, null);

            Console.WriteLine("Doing more work in a main thread {0}", Thread.CurrentThread.ManagedThreadId);

            int answer = operation.EndInvoke(result);

            Console.WriteLine("Answer is {0}", answer);
        }

        private static void AsyncCallbackDelegateExample()
        {
            BinaryOp operation = new BinaryOp(Add);
            IAsyncResult result = operation.BeginInvoke(10, 5, new AsyncCallback(AddComplete), null);

            // Other work is done here in main method
            while(!isDone)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Doing more work...");
            }
        }

        private static void AddComplete(IAsyncResult asyncResult)
        {
            // Async callback finishes in the same thread as delegate - so this approach won't work for
            // UI based application when UI update is required. 
            Console.WriteLine("AddCompleted executed on thread {0}", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Addition is completed");

            // Obtain results 
            AsyncResult result = (AsyncResult)asyncResult;
            BinaryOp del = (BinaryOp)result.AsyncDelegate; // reference to original delegate can be obtained in this way 

            int operationResult = del.EndInvoke(asyncResult);
            Console.WriteLine("Addition result is {0}", operationResult);

            isDone = true;
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
