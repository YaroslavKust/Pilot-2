using System;

namespace Words2
{
    class Program
    {
        static Game game;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            IWriter writer = new WriterConsole();
            IReader reader = new ReaderConsole();

            while (true)
            {
                game = new Game(writer, reader);
                game.Start();

                Console.WriteLine("Press any key to start game");
                Console.ReadKey();
            }
        }
    }
}
