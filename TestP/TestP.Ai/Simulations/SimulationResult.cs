using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestP.Ai.Simulations
{
    public class SimulationResult
    {
        public List<SimulationGameResult> GameResults { get; set; }

        public SimulationResult()
        {
            GameResults = new List<SimulationGameResult>();
        }

        public void AddGameResult(SimulationGameResult result)
        {
            GameResults.Add(result);
        }

        public StringBuilder PrintResult()
        {            
            var sb = new StringBuilder();

            if (GameResults.Any())
            {
                var winners = GameResults.Select(x => x.Winner).Distinct().OrderBy(x => x);

                sb.AppendLine($"{GameResults.Count() } games were played with an average length of {GameResults.Average(x => x.NumberOfRounds)}");

                foreach (var winner in winners)
                {
                    sb.AppendLine($"Player {winner} won { (GameResults.Where(x => x.Winner == winner).Count() / ((double)GameResults.Count())) * 100 } % of the games");
                }
            }

            return sb;
        }
    }
}
