using System;
using System.Collections.Generic;
using System.Text;
using TestP.Lib.Models.SimulationCore;

namespace TestP.Ai.QLearning
{
    public class StateAction
    {
        public StateAction(StrategyState input, StrategyAction output)
        {
            State = input;
            Action = output;
        }

        public StrategyState State { get; }
        public StrategyAction Action { get; }

        public string GetUniqueString()
        {
            return State.GetUniqueString() + " " + Action.GetUniqueString();
        }
    }
}
