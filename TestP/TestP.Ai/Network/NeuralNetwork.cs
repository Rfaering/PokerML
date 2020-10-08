using Keras;
using Keras.Layers;
using Keras.Models;
using System.Collections.Generic;
using System.IO;
using TestP.Ai.QLearning;
using TestP.Lib.Models.SimulationCore;
using Numpy;
using System.Linq;
using System;
using TestP.Ai.Network.Topologies;

namespace TestP.Ai.Network
{
    public class NeuralNetwork
    {
        private string DataPath = @"C:\Users\rfaer\source\repos\TestP\TestP.Ai\Network\Data\";

        public NeuralNetwork(string name, INeuralNetworkTopology topology = null)
        {
            Network = topology != null ? topology : new DefaultTopology();

            var jsonPath = Path.Combine(DataPath, name + ".json");
            var weightPaths = Path.Combine(DataPath, name + ".h5");            

            if (File.Exists(jsonPath))
            {
                Model = BaseModel.ModelFromJson(File.ReadAllText(jsonPath));
            }
            else
            {                                
                Model = Network.CreateBaseModel();
            }

            if (File.Exists(weightPaths))
            {
                Model.LoadWeight(weightPaths);
            }

            Name = name;
        }

        public ActionValues Predict(StrategyState state)
        {
            var array = new float[1, StrategyState.StateSize];
            var neural = state.ToNeuralNetworkState();
            
            for (int i = 0; i < neural.Length; i++)
            {
                array[0, i] = neural[i];
            }

            NDarray x = np.array(array);
            // var input = np.array(state.ToNeuralNetworkState());
            var response = Model.PredictOnBatch(x);
            return ActionValues.FromNeuralNetwork(response[0].GetData<float>());
        }

        public void TrainFromFile(string fileName = null)
        {
            if (!Compiled)
            {
                Network.CompileBaseModel(Model);
                Compiled = true;
            }

            var dataSet = TrainingDataStorage.LoadExperiance(fileName);

            float[][] stateValues = dataSet.TrainingData.Select(x => x.Input).ToArray();

            var input = np.array(CreateRectangularArray(stateValues));
            var output = np.array(CreateRectangularArray(dataSet.TrainingData.Select(x => x.Output).ToArray()));

            Model.Fit(input, output, epochs: 300, verbose: 2, shuffle: true);
        }

        public void Train(List<ExperianceMemoryStateAndTargetActionValues> states)
        {
            if (!Compiled)
            {
                Network.CompileBaseModel(Model);
                Compiled = true;
            }

            float[][] stateValues = states.Select(x => x.State.ToNeuralNetworkState()).ToArray();
            
            var input = np.array(CreateRectangularArray(stateValues));
            var output = np.array(CreateRectangularArray(states.Select(x => x.ActionValues.ToNeuralNetwork()).ToArray()));

            Model.Fit(input, output, epochs: 50, verbose: 2, shuffle: true);
        }

        public void Save()
        {
            var jsonPath = Path.Combine(DataPath, Name + ".json");
            var weightPaths = Path.Combine(DataPath, Name + ".h5");

            string json = Model.ToJson();
            File.WriteAllText(jsonPath, json);
            Model.SaveWeight(weightPaths);
        }

        public BaseModel Model { get; set; }
        public string Name { get; }
        public bool Compiled { get; set; }
        public INeuralNetworkTopology Network { get; }

        static T[,] CreateRectangularArray<T>(T[][] arrays)
        {
            int minorLength = arrays[0].Length;
            T[,] ret = new T[arrays.Length, minorLength];
            for (int i = 0; i < arrays.Length; i++)
            {
                var array = arrays[i];
                if (array.Length != minorLength)
                {
                    throw new ArgumentException("All arrays must be the same length");
                }
                for (int j = 0; j < minorLength; j++)
                {
                    ret[i, j] = array[j];
                }
            }
            return ret;
        }
    }
}
