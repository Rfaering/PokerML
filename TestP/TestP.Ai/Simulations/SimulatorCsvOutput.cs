using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestP.Ai.Simulations
{
    public class SimulatorCsvOutput
    {
        private string _csvName;

        public SimulatorCsvOutput(string csvName)
        {
            _csvName = csvName;
        }

        public ICollection<SimulatorCsvOutputLine> Lines { get; set; } = new List<SimulatorCsvOutputLine>();

        public void WriteToCsv()
        {
            var sb = new StringBuilder();
            sb.AppendLine(SimulatorCsvOutputLine.Header());

            foreach (var line in Lines)
            {
                sb.AppendLine(line.Line());
            }

            File.WriteAllText(Path.Combine(@"C:\Users\rfaer\source\repos\TestP\TestP.Ai\Simulations\Simulations\", _csvName + ".csv"), sb.ToString());
        }
    }
}
