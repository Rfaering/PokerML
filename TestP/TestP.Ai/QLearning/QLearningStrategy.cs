using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestP.Ai.Network;
using TestP.Ai.QLearning;
using TestP.Lib.Core;
using TestP.Lib.Models.SimulationCore;
using TestP.Lib.Models.Strategies.Output;

namespace TestP.Lib.Models.Strategies
{
    public class QLearningStrategy : IReinforcementStrategy
    {
        public static Random Rng = new Random();

        public string Name { get; }

        public float ExplorationRate { get; set; }
        public float LearningRate { get; set; } = 0.50f;
        public float DiscountRate { get; set; } = 0.95f;

        public float LastActionsQValue { get; set; }

        public QLearningEpisode ReplayMemory { get; set; } = new QLearningEpisode();

        public int AllActionsTaken { get; set; }
        public int ActionTakenFromModel { get; set; }        

        public NeuralNetwork FrozenNetwork { get; set; }
        public NeuralNetwork TrainingNetwork { get; set; }

        public QLearningStrategy(string name = "Test", float explorationRate = 0.9f)
        {
            ExplorationRate = explorationRate;
            FrozenNetwork = new NeuralNetwork(name);
            TrainingNetwork = new NeuralNetwork(name);
            Name = name;
        }

        public void Save()
        {
            TrainingNetwork.Save();            
        }

        public StrategyAction DoAction(StrategyState input)
        {
            AllActionsTaken++;

            var prediction = FrozenNetwork.Predict(input);

            var exploration = Rng.NextDouble();

            if (exploration <= ExplorationRate)
            {                
                var randomStrategy = prediction.GetRandomStrategy();
                Logger.LogDecision($"Chosen random strategy With input {input} - {randomStrategy.ActionTaken}");
                return randomStrategy;
            }
            else
            {
                var bestStrategy = prediction.GetBestStrategy();
                Logger.LogDecision($"Chosen best strategy With input {input} - {bestStrategy.ActionTaken}");
                return bestStrategy;
            }
        }

        public void ProcessEpisodeMemory()
        {                        
            StrategyState previousState = null;            

            while (ReplayMemory.NonProcessedReplayMemory.Any())
            {
                var currentAction = ReplayMemory.NonProcessedReplayMemory.Pop();
                currentAction.NextState = previousState;

                var expectedActionValues = FrozenNetwork.Predict(currentAction.State);

                var oldValue = expectedActionValues.AllActionValues[currentAction.Action.ActionTaken];

                float expectedNextValue = 0;

                if(currentAction.NextState != null)
                {
                    expectedNextValue = FrozenNetwork.Predict(currentAction.NextState).GetExpectedValueFromBestStrategy();
                }

                var newValue = (expectedNextValue * DiscountRate ) * (1.0f - currentAction.NextReward.ValueRepresentation) + (currentAction.NextReward.Value * currentAction.NextReward.ValueRepresentation);

                var actualNewValue = (1 - LearningRate) * oldValue + LearningRate * newValue;

                Logger.LogLearning($"Input: {currentAction.State.ToShortString()} Action: {currentAction.Action.ActionTaken} - Old: {oldValue}, New: {newValue}, Reward: {currentAction.NextReward.Value}, ActualNew: {actualNewValue}");

                expectedActionValues.AllActionValues[currentAction.Action.ActionTaken] = actualNewValue;

                ReplayMemory.ProcessedReplayMemory.Add(new ExperianceMemoryStateAndTargetActionValues() { State = currentAction.State, ActionValues = expectedActionValues });

                previousState = currentAction.State;
            }                                   
        }

        public void StoreExperiance()
        {
            TrainingDataStorage.StoreExperiance(ReplayMemory.ProcessedReplayMemory);
        }

        public void TrainWithProcessedMemory()
        {
            TrainingNetwork.Train(ReplayMemory.ProcessedReplayMemory);
        }

        public void AddToReplayMemory(StrategyState state, StrategyAction action, StrategyReward reward)
        {
            ReplayMemory.NonProcessedReplayMemory.Push(new ExperianceMemory(state, action, reward));
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{AllActionsTaken} actions taken, {ActionTakenFromModel} taken from model");

            return sb.ToString();
        }

        public void SetRewardToLastMemory(StrategyReward reward)
        {
            if (ReplayMemory.NonProcessedReplayMemory.Any())
            {
                var peekedElement = ReplayMemory.NonProcessedReplayMemory.Peek();
                peekedElement.NextReward.Value = reward.Value;
                peekedElement.NextReward.ValueRepresentation = reward.ValueRepresentation;
            }
        }
    }
}
