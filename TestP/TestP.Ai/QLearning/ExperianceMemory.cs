using System;
using System.Reflection.PortableExecutable;
using TestP.Lib.Models.SimulationCore;
using TestP.Lib.Models.Strategies.Output;

namespace TestP.Ai.QLearning
{
    public class ExperianceMemory
    {
        public ExperianceMemory(StrategyState state, StrategyAction action, StrategyReward reward)
        {
            State = state;
            Action = action;
            NextReward = reward;            
        }

        public StrategyState State { get; }
        public StrategyAction Action { get; }
        public StrategyReward NextReward { get; }
        public StrategyState NextState { get; set; }
    }
}
