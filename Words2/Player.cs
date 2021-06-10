using System;
using System.Collections.Generic;
using System.Text;

namespace Words2
{
    class Player
    {
        public string Name { get; set; }

        public int Score {get; set;}

        public Player(string name)
        {
            Name = name;
        }
    }
}
