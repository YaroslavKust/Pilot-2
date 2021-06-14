using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Words2
{
    class Game
    {
        Player[] _players;
        Player _active, _rival;
        string _mainWord;
        string _format = @"^[A-zА-яЁё]{8,30}$";
        List<string> _wordList;
        IWriter _writer;
        IReader _reader;
        Localization _local;
        bool end;
        Timer _timer;


        public Game(IWriter writer, IReader reader)
        {
            _writer = writer;
            _reader = reader;
            _players = new Player[2];
            _wordList = new List<string>();
        }


        public void Start()
        {
            end = false;
            SelectLanguage();
            EnterGameWord();
            InitializePlayers();
            GameCycle();
        }


        void SelectLanguage()
        {
            uint key;
            string fname = "";

            _writer.WriteMessage("Select language:\n1.English\n2.Русский");
            _writer.WriteMessage("Enter number, associated with your choice:");
            string num = _reader.Read();

            if (!(UInt32.TryParse(num, out key) && key <= 2))
            {
                _writer.WriteError("Invalid data!");
                SelectLanguage();
                return;
            }

            switch (key)
            {
                case 1:
                    fname = "En.json";
                    break;
                case 2:
                    fname = "Ru.json";
                    break;
            }

            _local = JsonConvert.DeserializeObject<Localization>
                (File.ReadAllText(@"..\..\..\JSON\Localization\" + fname));
        }


        void EnterGameWord()
        {
            _writer.WriteMessage(_local.EnterGameWord);
            string inputString = _reader.Read();

            Regex regex = new Regex(_format);
            Match match = regex.Match(inputString.Trim());

            if (match.Success)
                _mainWord = inputString;
            else
            {
                if (inputString.Length < 8)
                    _writer.WriteError(_local.TooShortWord);
                else if (inputString.Length > 30)
                    _writer.WriteError(_local.TooLongWord);
                else
                    _writer.WriteError(_local.WrongSymbols);

                _writer.WriteMessage(_local.TryAgain);
                EnterGameWord();
            }
        }


        void InitializePlayers()
        {
            for (int i = 0; i < 2; i++)
            {
                _writer.WriteMessage(_local.Player + (i + 1) + ' ' + _local.EnterName);
                _players[i] = new Player(EnterPlayerName());
            }
        }


        string EnterPlayerName()
        {
            string name = _reader.Read();

            if (string.IsNullOrWhiteSpace(name))
            {
                _writer.WriteError(_local.EmptyName + '\n' + _local.TryAgain);
                return EnterPlayerName();
            }

            if (_players.Any(p =>p != null && p.Name == name))
            {
                _writer.WriteError(_local.TakenName + '\n' + _local.TryAgain);
                return EnterPlayerName();
            }

            return name;
        }


        void GameCycle()
        {
            string mainWord = _mainWord.ToLower();

            _active = _players[0];
            _rival = _players[1];

            while (true)
            {
                _writer.WriteMessage(_active.Name + ": ");

                TimerCallback tc = new TimerCallback(o => EndGame());
                _timer = new Timer(tc, null, 10000, Timeout.Infinite);

                string word = _reader.Read().ToLower();

                if (end)
                    return;
                else
                    _timer.Dispose();

                if (ExecCommand(word) == 0)
                {
                    for (int i = 0; i < word.Length; i++)
                        if (!mainWord.Contains(word[i]) || string.IsNullOrWhiteSpace(word))
                        {
                            _writer.WriteMessage(_local.WrongWord);
                            EndGame();
                            return;
                        }

                    if (_wordList.Contains(word))
                    {
                        _writer.WriteMessage(_local.WordAlreadyExists);
                        continue;
                    }
                    else
                        _wordList.Add(word);

                    Player rezPlayer = _rival;
                    _rival = _active;
                    _active = rezPlayer;
                }
            }
        }


        int ExecCommand(string command)
        {
            switch (command)
            {
                case @"/show-words":
                    foreach (string s in _wordList)
                        _writer.WriteMessage(s);
                    break;
                case @"/score":
                    for (int i = 0; i < 2; i++)
                        _writer.WriteMessage(_players[i].Name + ": " + Result.GetPlayerScore(_players[i]));
                    break;
                case @"/total-score":
                    foreach (Player p in Result.GetTotalResults())
                        _writer.WriteMessage(p.Name + ": " + p.Score);
                    break;
                default:
                    return 0;
            }
            return 1;
        }


        void EndGame()
        {
            _timer.Dispose();
            Console.WriteLine("Время вышло!");
            _writer.WriteMessage(_active.Name + ' ' + _local.Lose);
            _writer.WriteMessage(_rival.Name + ' ' + _local.Win);
            _rival.Score = Result.GetPlayerScore(_rival) + 1;
            Result.WriteResult(_rival);
            end = true;
        }
    }
}
