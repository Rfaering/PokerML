using System;
using System.Collections.Generic;
using System.Text;
using TestP.Lib.Models.SimulationCore;

namespace TestP.Ai.QLearning
{
    public class QLearningEpisode
    {
        public Stack<ExperianceMemory> NonProcessedReplayMemory { get; set; } = new Stack<ExperianceMemory>();
        public List<ExperianceMemoryStateAndTargetActionValues> ProcessedReplayMemory { get; set; } = new List<ExperianceMemoryStateAndTargetActionValues>();
    }
}
