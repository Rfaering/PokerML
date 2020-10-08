using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace TestP.Ai.QLearning
{
    public class QLearningModel
    {
        public Dictionary<string, double> ExpectedValue { get; set; } = new Dictionary<string, double>();
    }
}
