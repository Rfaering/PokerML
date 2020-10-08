using System;
using System.Collections.Generic;
using System.Text;
using TestP.Lib.Models.SimulationCore;
using TestP.Lib.Models.Strategies.Output;

namespace TestP.Lib.Models.Strategies
{
    public interface IReinforcementStrategy : IStrategy
    {
        float LastActionsQValue { get; }
        void ProcessEpisodeMemory();
        void AddToReplayMemory(StrategyState state, StrategyAction action, StrategyReward reward);
        void SetRewardToLastMemory(StrategyReward reward);
    }
}
