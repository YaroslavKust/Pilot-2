using System;
using System.Collections.Generic;
using System.Text;

namespace Words2
{
    class WriterConsole: IWriter
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteError(string textError)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(textError);
            Console.ResetColor();
        }
    }
}
