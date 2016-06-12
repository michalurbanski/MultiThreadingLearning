using System;
using System.Threading;

namespace ThreadsApp
{
    class Printer
    {
        public void PrintNumbers()
        {
            Console.WriteLine("-> {0} is executing {1}", 
                Thread.CurrentThread.Name, nameof(PrintNumbers));

            Console.Write("Your numbers: ");
            for (int i = 0; i < 10; i++)
            {
                Console.Write("{0} ", i);
                Thread.Sleep(2000); 
            }

            Console.WriteLine();
        }
    }
}
