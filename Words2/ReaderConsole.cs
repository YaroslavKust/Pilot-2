using System;
using System.Collections.Generic;
using System.Text;

namespace Words2
{
    class ReaderConsole: IReader
    {
        public string Read()
        {
            return Console.ReadLine();
        }
    }
}
