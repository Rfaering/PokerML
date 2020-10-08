using System;
using System.Collections.Generic;
using System.Text;
using TestP.Lib.Models.Strategies.Output;

namespace TestP.Lib.Models.SimulationCore
{
    public class StrategyAction
    {
        public static int StateSize = 10;

        public StrategyAction(PlayerAction actionTaken)
        {
            ActionTaken = actionTaken;
        }

        public PlayerAction ActionTaken { get; }

        public override int GetHashCode()
        {
            return ActionTaken.GetHashCode();
        }

        public string GetUniqueString()
        {
            return ActionTaken.ToString();
        }
    }
}
