
using Keras;
using Keras.Layers;
using Keras.Models;
using Numpy;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TestP.Ai.QLearning;
using TestP.Lib.Models.SimulationCore;

namespace TestP.Ai
{
    public class AiTest
    {
        public void TrainWithQLearning(QLearningEpisode episode)
        {
            episode.NonProcessedReplayMemory.Select(x => x.State.ToNeuralNetworkState());
        }

        public void Train()
        {
            //Load train data
            NDarray x = np.array(new float[,] { { 1, 1, 1, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 1 }, { 1, 1, 1, 1 } });
            NDarray y = np.array(new float[] { 0, 0, 0, 1 });

            //Build sequential model
            var model = new Sequential();
            model.Add(new Dense(24, activation: "relu", input_shape: new Shape(StrategyState.StateSize)));
            model.Add(new Dense(24, activation: "relu"));
            model.Add(new Dense(StrategyAction.StateSize, activation: "sigmoid"));

            //Compile and train
            model.Compile(optimizer: "sgd", loss: "binary_crossentropy", metrics: new string[] { "accuracy" });

            var loaded_model = Sequential.ModelFromJson(File.ReadAllText("model.json"));
            loaded_model.LoadWeight("model.h5");
            


            model.Fit(x, y, epochs: 1000, verbose: 1);
            

            //Save model and weights
            string json = model.ToJson();
            File.WriteAllText("model.json", json);
            model.SaveWeight("model.h5");

            //Load model and weight

        }
    }
}
