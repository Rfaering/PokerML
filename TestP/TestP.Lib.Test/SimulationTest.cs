using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TestP.Ai.QLearning;
using TestP.Ai.Simulations;
using TestP.Lib.Core;
using TestP.Lib.Models;
using TestP.Lib.Models.Strategies;
using Xunit;
using Xunit.Abstractions;

namespace TestP.Lib.Test
{
    public class SimulationTest
    {

        public SimulationTest(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
            var converter = new Converter(outputHelper);
            Console.SetOut(converter);
        }

        public ITestOutputHelper OutputHelper { get; }

        private class Converter : TextWriter
        {
            ITestOutputHelper _output;
            public Converter(ITestOutputHelper output)
            {
                _output = output;
            }
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
            public override void WriteLine(string message)
            {
                _output.WriteLine(message);
            }
            public override void WriteLine(string format, params object[] args)
            {
                _output.WriteLine(format, args);
            }
        }


        [Fact]
        public void SimpleRun()
        {
            var results = new Simulator(100, 42)
                .AddPlayerStrategy(1, new AlwaysRunAction(Models.Strategies.Output.PlayerAction.Check))
                .AddPlayerStrategy(2, new AlwaysRunAction(Models.Strategies.Output.PlayerAction.Fold))
                .RunSimulation();

            OutputHelper.WriteLine(results.PrintResult().ToString());
        }

        [Fact]
        public void Learning()
        {
            for (int i = 0; i < 500; i++)
            {
                var qLearning = new QLearningStrategy("Test", 0.25f);
                // qLearning.Writer = s => OutputHelper.WriteLine(s);

                var results = new Simulator(100, 24)
                    .AddPlayerStrategy(1, new AlwaysRunAction(Models.Strategies.Output.PlayerAction.Check))
                    .AddPlayerStrategy(2, qLearning)
                    .RunSimulation();

                OutputHelper.WriteLine(qLearning.ToString());
                OutputHelper.WriteLine(results.PrintResult().ToString());

                qLearning.Save();
            }
        }

        [Fact]
        public void Test()
        {
            // Logger.GameWriter = m => OutputHelper.WriteLine(m);
            // Logger.DecisionWriter = m => OutputHelper.WriteLine(m);

            var qLearning1 = new QLearningStrategy("Test2", 0.5f);
            var qLearning2 = new QLearningStrategy("Test", 0.5f);

            var results = new Simulator(20)
                .AddPlayerStrategy(1, qLearning1)
                .AddPlayerStrategy(2, qLearning2)
                .RunSimulation();

            OutputHelper.WriteLine(qLearning2.ToString());
            OutputHelper.WriteLine(results.PrintResult().ToString());

            qLearning1.Save();

            qLearning2.TrainWithProcessedMemory();
            qLearning2.Save();            
        }        
    }
}
