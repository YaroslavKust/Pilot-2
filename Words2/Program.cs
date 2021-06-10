using System;

namespace Words2
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Game game = new Game();
                game.Start();

                Console.WriteLine("Нажмите любую клавишу для начала игры");
                Console.ReadKey();
            }
        }
    }
}
