using System;
using System.Collections.Generic;
using System.Text;

namespace TestP.Ai.Simulations
{
    public class SimulatorCsvOutputLine
    {
        public int Game { get; set; }
        public int Player { get; set; }
        public int Round { get; set; }
        public double Money { get; set; }
        public double QValue { get; set; }

        public static string Header()
        {
            return "Game;Player;Round;Money;QValue";
        }

        internal string Line()
        {
            return $"{Game};{Player};{Round};{Money};{QValue}";
        }
    }
}
