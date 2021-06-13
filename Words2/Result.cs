using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Words2
{
    static class Result
    {
        static string _path = @"..\..\..\JSON\GameResults.json";
        static JArray _resultList;


        static Result()
        {
            _resultList = JArray.Parse(File.ReadAllText(_path));
        }


        private static JToken GetPlayerToken(Player player)
        {
            return _resultList.FirstOrDefault(a => a.ToObject<Player>().Name == player.Name);
        }


        public static int GetPlayerScore(Player player)
        {
            JToken res = GetPlayerToken(player);

            if (res is null)
                return 0;
            else
                return res.ToObject<Player>().Score;
        }


        public static List<Player> GetTotalResults()
        {
            List<Player> players = new List<Player>();

            foreach (JToken r in _resultList)
                players.Add(r.ToObject<Player>());

            return players;
        }


        public static void WriteResult(Player player)
        {
            var p = GetPlayerToken(player);

            if (p is null)
                _resultList.Add(JToken.FromObject(player));
            else
                _resultList[_resultList.IndexOf(p)] = JToken.FromObject(player);

            File.WriteAllText(_path,_resultList.ToString());
        }
    }
}
