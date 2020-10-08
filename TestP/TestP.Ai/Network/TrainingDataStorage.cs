using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TestP.Ai.Data;
using TestP.Ai.QLearning;

namespace TestP.Ai.Network
{
    public class TrainingDataStorage
    {
        public static string TrainingDataDirectory => @"C:\TrainingData\";
        public static string TrainingDataDirectoryOld => @"C:\TrainingDataOld\";

        public static void StoreExperiance(List<ExperianceMemoryStateAndTargetActionValues> states)
        {
            var dataSet = new TrainingDataSet();

            foreach (var state in states)
            {                
                var trainingData = new TrainingData()
                {
                    Input = state.State.ToNeuralNetworkState(),
                    Output = state.ActionValues.ToNeuralNetwork()
                };

                dataSet.TrainingData.Add(trainingData);
            }

            var fileName = DateTime.UtcNow.ToString("MM-ddTHH:mm:ss") + Guid.NewGuid().ToString();
            File.WriteAllText( Path.Combine(TrainingDataDirectory, fileName + ".json"), JsonConvert.SerializeObject(dataSet));

            Console.WriteLine(fileName);
        }

        internal static TrainingDataSet LoadExperiance(string fileName = null)
        {            
            if(fileName == null)
            {
                TrainingDataSet trainingDataSet = new TrainingDataSet();
                var files = Directory.GetFiles(TrainingDataDirectory);

                foreach (var item in files)
                {
                    var dataSet = LoadFile(Path.GetFileNameWithoutExtension(item));
                    trainingDataSet.TrainingData.AddRange(dataSet.TrainingData);
                }

                return trainingDataSet;
            }
            else
            {
                return LoadFile(fileName);
            }
        }

        internal static TrainingDataSet LoadFile(string fileName = null)
        {
            var filePath = Path.Combine(TrainingDataDirectory, fileName + ".json");

            Console.WriteLine("Reading: " + filePath);
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<TrainingDataSet>(json);
        }
    }
}
