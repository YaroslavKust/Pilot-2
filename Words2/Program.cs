using System;

namespace Words2
{
    class Program
    {
        static void Main(string[] args)
        {
            IWriter writer = new WriterConsole();
            IReader reader = new ReaderConsole();

            while (true)
            {
                Game game = new Game(writer, reader);
                game.Start();

                Console.WriteLine("Нажмите любую клавишу для начала игры");
                Console.ReadKey();
            }
        }
    }
}
