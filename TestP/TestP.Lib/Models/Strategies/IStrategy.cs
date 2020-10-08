using System;
using System.Collections.Generic;
using System.Text;

namespace TestP.Lib.Models.SimulationCore
{
    public interface IStrategy
    {
        StrategyAction DoAction(StrategyState input);
    }
}
