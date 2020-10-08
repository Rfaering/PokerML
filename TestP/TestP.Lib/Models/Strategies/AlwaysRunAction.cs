using TestP.Lib.Models.SimulationCore;
using TestP.Lib.Models.Strategies.Output;

namespace TestP.Lib.Models.Strategies
{
    public class AlwaysRunAction : IStrategy
    {
        public AlwaysRunAction(PlayerAction strategyAction)
        {
            PlayerAction = strategyAction;
        }

        public PlayerAction PlayerAction { get; }

        public StrategyAction DoAction(StrategyState input)
        {
            return new StrategyAction(PlayerAction);
        }
    }
}
