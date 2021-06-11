using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Words2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            IWriter writer = new WriterConsole();
            IReader reader = new ReaderConsole();

            while (true)
            {
                Game game = new Game(writer, reader);
                game.Start();

                Console.WriteLine("Press any key to start game");
                Console.ReadKey();
            }
        }
    }
}
