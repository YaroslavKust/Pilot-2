using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Words2
{
    class Game
    {
        Player[] _players;
        string _mainWord;
        string _format = @"^[A-zА-яЁё]{8,30}$";
        List<string> _wordList;
        Result _res;


        public Game()
        {
            _players = new Player[2];
            _wordList = new List<string>();
            _res = new Result();
        }


        public void Start()
        {
            EnterGameWord();
            InicializePlayers();
            GameCycle();
        }


        void EnterGameWord()
        {
            Console.Write("Введите игровое слово длиной от 8 до 30 символов: ");
            string inputString = Console.ReadLine();

            Regex regex = new Regex(_format);
            Match match = regex.Match(inputString.Trim());

            if (match.Success)
                _mainWord = inputString;
            else
            {
                if (inputString.Length < 8)
                    Console.WriteLine("Слово слишком короткое!");
                else if (inputString.Length > 30)
                    Console.WriteLine("Слово слишком длинное!");
                else
                    Console.WriteLine("Слово содержит неверные символы!");

                Console.WriteLine("Попробуйте ещё раз");
                EnterGameWord();
            }
        }


        string EnterPlayerName()
        {
            string name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.Write("Введено пустое имя!\nПопробуйте ещё раз: ");
                return EnterPlayerName();
            }

            if (_players.Any(p =>p != null && p.Name == name))
            {
                Console.Write("Имя уже занято!\nПопробуйте ещё раз: ");
                return EnterPlayerName();
            }

            return name;
        }


        void InicializePlayers()
        {
            for (int i = 0; i < 2; i++)
            {
                Console.Write($"Игрок {i + 1}, введите ваше имя: ");
                _players[i] = new Player(EnterPlayerName());
            }
        }


        void GameCycle()
        {
            string mainWord = _mainWord.ToLower();

            Player player = _players[0];
            Player rival = _players[1];

            while (true)
            {
                Console.Write(player.Name + ": ");
                string word = Console.ReadLine();
                word = word.ToLower();

                if (ExecCommand(word) == 0)
                {
                    for (int i = 0; i < word.Length; i++)
                        if (!mainWord.Contains(word[i]) || string.IsNullOrWhiteSpace(word))
                        {
                            Console.WriteLine("Неверное слово!");
                            Console.WriteLine(player.Name + " проиграл");
                            rival.Score = _res.GetPlayerScore(rival) + 1;
                            _res.WriteResult(rival);
                            return;
                        }

                    if (_wordList.Contains(word))
                    {
                        Console.WriteLine("Слово уже было");
                        continue;
                    }
                    else
                        _wordList.Add(word);

                    Player rezPlayer = rival;
                    rival = player;
                    player = rezPlayer;
                }
            }
        }


        int ExecCommand(string command)
        {
            switch (command)
            {
                case @"/show-words":
                    foreach (string s in _wordList)
                        Console.WriteLine(s);
                    break;
                case @"/score":
                    for (int i = 0; i < 2; i++)
                        Console.WriteLine(_players[i].Name + ": " + _res.GetPlayerScore(_players[i]));
                    break;
                case @"/total-score":
                    foreach (Player p in _res.GetTotalResults())
                        Console.WriteLine(p.Name + ": " + p.Score);
                    break;
                default:
                    return 0;
            }
            return 1;
        }
    }
}
