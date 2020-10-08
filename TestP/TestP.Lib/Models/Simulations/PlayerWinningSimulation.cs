using System.Collections.Generic;
using System.Linq;

namespace TestP.Lib.Models.Games
{
    public class PlayerWinningSimulation
    {
        public int ExpectedGainLoss { get; set; }

        public Hand SimulationHand { get; set; }

        public Player Player { get; }

        public PlayerWinningSimulation(Player player)
        {
            Player = player;
        }
    }
}
