using System;
using System.Collections.Generic;
using System.Linq;
using TestP.Lib.Models.SimulationCore;
using TestP.Lib.Models.Strategies.Output;

namespace TestP.Ai.QLearning
{
    public class ActionValues
    {
        public Random rng = new Random();

        public ActionValues()
        {
            AllActionValues = new Dictionary<PlayerAction, float>();

            foreach (PlayerAction item in Enum.GetValues(typeof(PlayerAction)))
            {
                AllActionValues[item] = 0;                
            }
        }

        public Dictionary<PlayerAction, float> AllActionValues { get; set; } = new Dictionary<PlayerAction, float>();

        public float[] ToNeuralNetwork()
        {
            var values = Enum.GetValues(typeof(PlayerAction));

            var result = new float[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                result[i] = AllActionValues[(PlayerAction)(values.GetValue(i))];
            }

            return result;
        }

        public static ActionValues FromNeuralNetwork(float[] array)
        {
            var actionValues = new ActionValues();

            for (int i = 0; i < array.Length; i++)
            {
                if(array[i] != array[i])
                {
                    array[i] = 0f;
                }
            }

            var values = Enum.GetValues(typeof(PlayerAction));

            for (int i = 0; i < array.Length; i++)
            {
                actionValues.AllActionValues[(PlayerAction)(values.GetValue(i))] = array[i];
            }

            return actionValues;
        }

        internal StrategyAction GetBestStrategy()
        {
            var max = AllActionValues.OrderByDescending(x => x.Value);
            return new StrategyAction(max.First().Key);
        }

        internal float GetExpectedValueFromBestStrategy()
        {
            var max = AllActionValues.OrderByDescending(x => x.Value);
            return max.First().Value;
        }

        internal StrategyAction GetRandomStrategy()
        {
            var rngIndex = rng.Next(AllActionValues.Count);
            return new StrategyAction(AllActionValues.Select(x => x.Key).ToList()[rngIndex]);
        }
    }
}
