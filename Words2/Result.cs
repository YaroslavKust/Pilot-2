using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Words2
{
    class Result
    {
        string _path = @"..\..\..\JSON\GameResults.json";
        JArray _resultList;


        public Result()
        {
            _resultList = JArray.Parse(File.ReadAllText(_path));
        }


        private JToken GetPlayerToken(Player player)
        {
            return _resultList.FirstOrDefault(a => a.ToObject<Player>().Name == player.Name);
        }


        public int GetPlayerScore(Player player)
        {
            JToken res = GetPlayerToken(player);

            if (res is null)
                return 0;
            else
                return res.ToObject<Player>().Score;
        }


        public List<Player> GetTotalResults()
        {
            List<Player> players = new List<Player>();

            foreach (JToken r in _resultList)
                players.Add(r.ToObject<Player>());

            return players;
        }


        public void WriteResult(Player player)
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
