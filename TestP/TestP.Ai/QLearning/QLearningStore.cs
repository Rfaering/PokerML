using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestP.Ai.QLearning
{
    public class QLearningStore
    {        
        public static string DataPath = @"C:\Users\rfaer\source\repos\TestP\TestP.Ai\QLearning\Data\";

        public static QLearningModel Load(string name)
        {
            var fileName = name + ".json";

            var path = Path.Combine(DataPath, fileName);

            if (!File.Exists(path))
            {
                return new QLearningModel();
            }

            return JsonConvert.DeserializeObject<QLearningModel>(File.ReadAllText(path));
        }

        public static void Save(string name, QLearningModel model)
        {
            File.WriteAllText(Path.Combine(DataPath, name+".json"), JsonConvert.SerializeObject(model));
        }
    }
}
