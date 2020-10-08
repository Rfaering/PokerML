using Keras;
using Keras.Layers;
using Keras.Models;
using TestP.Lib.Models.SimulationCore;

namespace TestP.Ai.Network.Topologies
{
    public class TestTopology : INeuralNetworkTopology
    {
        public BaseModel CreateBaseModel()
        {
            var model = new Sequential();
            model.Add(new Dense(32, activation: "relu", input_shape: new Shape(StrategyState.StateSize)));            
            model.Add(new Dense(16, activation: "relu"));
            model.Add(new Dense(StrategyAction.StateSize, activation: "linear"));

            return model;
        }

        public void CompileBaseModel(BaseModel model)
        {
            model.Compile(optimizer: "adam", loss: "mse");
        }
    }
}
