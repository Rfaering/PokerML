using System;
using TestP.Ai.Network;
using TestP.Ai.Network.Topologies;
using TestP.Ai.Simulations;
using TestP.Lib.Models.Strategies;
using TestP.Lib.Models.Strategies.Output;

namespace TestP
{
    class Program
    {
        static void Main(string[] args)
        {
            // TrainNetwork();
            RunTest();
            // RunLargeTraining();
            // RunSimulation();
        }

        private static void RunLargeTraining()
        {
            for (int i = 0; i < 1000; i++)
            {
                RunSimulation();
                // TrainNetwork();
            }
        }

        private static void TrainNetwork()
        {
            var neuralNetwork = new NeuralNetwork("Test", new DefaultTopology());
            neuralNetwork.TrainFromFile();
            neuralNetwork.Save();
        }

        private static void RunTest()
        {
            // Logger.DecisionWriter = s => Console.WriteLine(s);
            // Logger.LearningWriter = s => Console.WriteLine(s);
            // Logger.GameWriter = s => Console.WriteLine(s);

            var qLearning = new QLearningStrategy("Test", 0.0f);

            var result = new Simulator(1000, runToEnd: true, runMidSimulations: false)
                .AddPlayerStrategy(1, qLearning)
                .AddPlayerStrategy(2, new AlwaysRunAction(PlayerAction.Check))
                .AddPlayerStrategy(3, new AlwaysRunAction(PlayerAction.Check))
                .AddPlayerStrategy(4, new AlwaysRunAction(PlayerAction.Check))
                .AddPlayerStrategy(5, new AlwaysRunAction(PlayerAction.Check))
                .AddPlayerStrategy(6, new AlwaysRunAction(PlayerAction.Check))
                .RunSimulation();

            Console.WriteLine(result.PrintResult());            
        }

        private static void RunSimulation()
        {
            // Logger.DecisionWriter = s => Console.WriteLine(s);
            // Logger.LearningWriter = s => Console.WriteLine(s);
            // Logger.GameWriter = s => Console.WriteLine(s);

            var qLearning = new QLearningStrategy("Test", 0.5f);

            var result = new Simulator(100)
                .AddPlayerStrategy(1, qLearning)
                .AddPlayerStrategy(2, new AlwaysRunAction(PlayerAction.Check))
                .AddPlayerStrategy(3, new AlwaysRunAction(PlayerAction.Check))
                .AddPlayerStrategy(4, new AlwaysRunAction(PlayerAction.Check))
                .AddPlayerStrategy(5, new AlwaysRunAction(PlayerAction.Check))
                .AddPlayerStrategy(6, new AlwaysRunAction(PlayerAction.Check))
                .RunSimulation();

            Console.WriteLine(result.PrintResult());

            qLearning.StoreExperiance();            
        }
    }
}

