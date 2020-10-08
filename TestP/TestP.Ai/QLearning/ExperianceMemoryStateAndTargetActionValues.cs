using System;
using System.Collections.Generic;
using System.Text;
using TestP.Lib.Models.SimulationCore;

namespace TestP.Ai.QLearning
{
    public class ExperianceMemoryStateAndTargetActionValues
    {
        public StrategyState State { get; set; }
        public ActionValues ActionValues { get; set; }
    }
}
