using System;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Windows.Forms;

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
            //action = AsyncCallbackDelegateExample;
            //action = ThreadDelegatesThreadStartExample;
            action = ThreadDelegatesParameterizeThreadStartExample;

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

        /// <summary>
        /// Last two parameters of BeginInvoke method are used. Callback method is automatically 
        /// called when asynchronous operation is finished. 
        /// </summary>
        private static void AsyncCallbackDelegateExample()
        {
            BinaryOp operation = new BinaryOp(Add);

            string customMessage = "Thanks delegate for adding these numbers";
            // Last parameter allows to pass any data from calling thread to spawn thread
            IAsyncResult result = operation.BeginInvoke(10, 5, new AsyncCallback(AddComplete), customMessage);

            // Other work is done here in main method
            while(!isDone)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Doing more work...");
            }
        }

        /// <summary>
        /// Manual creation of threads using delegate which does not allow to use any parameters
        /// </summary>
        private static void ThreadDelegatesThreadStartExample()
        {
            // Name the current thread
            Thread primaryThread = Thread.CurrentThread;
            primaryThread.Name = "primary thread";

            Console.WriteLine("-> {0} is executing main() method", Thread.CurrentThread.Name);

            // Make worker class 
            Printer p = new Printer();

            // Now make another foreground thread
            Thread anotherThread = new Thread(new ThreadStart(p.PrintNumbers));
            anotherThread.Name = "secondary thread";
            Console.WriteLine("Is background thread {0}", anotherThread.IsBackground);

            anotherThread.Start();

            MessageBox.Show("I'm busy", "Main thread window");
        }

        /// <summary>
        /// Manual creation of threads using delegate which allows to use additional parameters
        /// </summary>
        private static void ThreadDelegatesParameterizeThreadStartExample()
        {
            Thread primaryThread = Thread.CurrentThread;
            primaryThread.Name = "primary thread";

            Console.WriteLine("-> {0} is executing main() method", Thread.CurrentThread.Name);

            // Make worker class 
            Printer p = new Printer();

            // Now make another foreground thread with parameters
            string additionalParameter = "Value passed from main method";
            Thread anotherThread = new Thread(new ParameterizedThreadStart(p.PrintNumbers));
            anotherThread.Start(additionalParameter); // This is a way to pass additional parameter (this requires parameterized method to access object) 

            Thread.Sleep(1000);

            // Thread is executing and main method can continue
            Console.WriteLine("Doing more work in main method ...");
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

            // Get custom data passed to delegate
            string message = (string)asyncResult.AsyncState; // you have know the type here of passed object data

            Console.WriteLine(message);

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
