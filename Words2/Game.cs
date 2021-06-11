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

        IWriter _writer;
        IReader _reader;


        public Game(IWriter writer, IReader reader)
        {
            _writer = writer;
            _reader = reader;
            _players = new Player[2];
            _wordList = new List<string>();
            _res = new Result();
        }


        public void Start()
        {
            EnterGameWord();
            InitializePlayers();
            GameCycle();
        }


        void EnterGameWord()
        {
            _writer.Write("Введите игровое слово длиной от 8 до 30 символов: ");
            string inputString = _reader.Read();

            Regex regex = new Regex(_format);
            Match match = regex.Match(inputString.Trim());

            if (match.Success)
                _mainWord = inputString;
            else
            {
                if (inputString.Length < 8)
                    _writer.WriteError("Слово слишком короткое!");
                else if (inputString.Length > 30)
                    _writer.WriteError("Слово слишком длинное!");
                else
                    _writer.WriteError("Слово содержит неверные символы!");

                _writer.Write("Попробуйте ещё раз");
                EnterGameWord();
            }
        }


        string EnterPlayerName()
        {
            string name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                _writer.WriteError("Введено пустое имя!\nПопробуйте ещё раз: ");
                return EnterPlayerName();
            }

            if (_players.Any(p =>p != null && p.Name == name))
            {
                _writer.WriteError("Имя уже занято!\nПопробуйте ещё раз: ");
                return EnterPlayerName();
            }

            return name;
        }


        void InitializePlayers()
        {
            for (int i = 0; i < 2; i++)
            {
                _writer.Write($"Игрок {i + 1}, введите ваше имя: ");
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
                _writer.Write(player.Name + ": ");
                string word = _reader.Read();
                word = word.ToLower();

                if (ExecCommand(word) == 0)
                {
                    for (int i = 0; i < word.Length; i++)
                        if (!mainWord.Contains(word[i]) || string.IsNullOrWhiteSpace(word))
                        {
                            _writer.Write("Неверное слово!");
                            _writer.Write(player.Name + " проиграл");
                            _writer.Write(rival.Name + " победил");
                            rival.Score = _res.GetPlayerScore(rival) + 1;
                            _res.WriteResult(rival);
                            return;
                        }

                    if (_wordList.Contains(word))
                    {
                        _writer.Write("Слово уже было");
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
                        _writer.Write(s);
                    break;
                case @"/score":
                    for (int i = 0; i < 2; i++)
                        _writer.Write(_players[i].Name + ": " + _res.GetPlayerScore(_players[i]));
                    break;
                case @"/total-score":
                    foreach (Player p in _res.GetTotalResults())
                        _writer.Write(p.Name + ": " + p.Score);
                    break;
                default:
                    return 0;
            }
            return 1;
        }
    }
}
