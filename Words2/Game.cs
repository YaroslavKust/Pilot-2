using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
        Localization _local;


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
            EditLocalization();
            EnterGameWord();
            InitializePlayers();
            GameCycle();
        }


        void EditLocalization()
        {
            _writer.WriteMessage("Select language:\n1.English\n2.Русский");

            uint key = SelectLanguage();
            string fname = "";

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

        uint SelectLanguage()
        {
            uint key;

            _writer.WriteMessage("Enter number, associated with your choice:");
            string num = _reader.Read();

            if (UInt32.TryParse(num, out key) && key <= 2)
                return key;
            else
            {
                _writer.WriteError("Invalid data!");
                return SelectLanguage();
            }
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


        void InitializePlayers()
        {
            for (int i = 0; i < 2; i++)
            {
                _writer.WriteMessage(_local.Player + (i + 1) + ' ' + _local.EnterName);
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
                _writer.WriteMessage(player.Name + ": ");
                string word = _reader.Read();
                word = word.ToLower();

                if (ExecCommand(word) == 0)
                {
                    for (int i = 0; i < word.Length; i++)
                        if (!mainWord.Contains(word[i]) || string.IsNullOrWhiteSpace(word))
                        {
                            _writer.WriteMessage(_local.WrongWord);
                            _writer.WriteMessage(player.Name + ' ' + _local.Lose);
                            _writer.WriteMessage(rival.Name + ' ' + _local.Win);
                            rival.Score = _res.GetPlayerScore(rival) + 1;
                            _res.WriteResult(rival);
                            return;
                        }

                    if (_wordList.Contains(word))
                    {
                        _writer.WriteMessage(_local.WordAlreadyExists);
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
                        _writer.WriteMessage(s);
                    break;
                case @"/score":
                    for (int i = 0; i < 2; i++)
                        _writer.WriteMessage(_players[i].Name + ": " + _res.GetPlayerScore(_players[i]));
                    break;
                case @"/total-score":
                    foreach (Player p in _res.GetTotalResults())
                        _writer.WriteMessage(p.Name + ": " + p.Score);
                    break;
                default:
                    return 0;
            }
            return 1;
        }
    }
}
