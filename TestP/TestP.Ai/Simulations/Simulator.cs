using System;
using System.Collections.Generic;
using System.Linq;
using TestP.Lib.Models;
using TestP.Lib.Models.Games;
using TestP.Lib.Models.SimulationCore;
using TestP.Lib.Models.Strategies;
using TestP.Lib.Models.Strategies.Output;

namespace TestP.Ai.Simulations
{
    public class Simulator
    {
        public int NumberOfGames { get; }

        private IDictionary<int, IStrategy> PlayerStrategies = new Dictionary<int, IStrategy>();

        private IStrategy DefaultStrategy = new AlwaysRunAction(PlayerAction.Check);
        private readonly bool runToEnd;
        private readonly bool runMidSimulations;

        public SimulatorCsvOutput SimulatorCsvOuput { get; set; }

        public Simulator(int numberOfGames = 10, int? seed = null, string csvoutput = null, bool runToEnd = false, bool runMidSimulations = true)
        {
            if(csvoutput != null)
            {
                SimulatorCsvOuput = new SimulatorCsvOutput(csvoutput);
            }

            NumberOfGames = numberOfGames;
            this.runToEnd = runToEnd;
            this.runMidSimulations = runMidSimulations;
            if (seed.HasValue)
            {
                Deck.SetSeed(seed.Value);
            }
        }

        public Simulator AddPlayerStrategy(int playerNumber, IStrategy strategy)
        {
            PlayerStrategies[playerNumber] = strategy;
            return this;
        }

        public SimulationResult RunSimulation()
        {
            SimulationResult results = new SimulationResult();

            for (int i = 0; i < NumberOfGames; i++)
            {
                var game = new Game(6);

                game.StartNewRound();

                while (!game.Done)
                {
                    var playerStrategy = PlayerStrategies.ContainsKey(game.CurrentPlayer.PlayerNumber) ? PlayerStrategies[game.CurrentPlayer.PlayerNumber] : DefaultStrategy;

                    StrategyState input = StrategyState.Create(game);

                    var output = playerStrategy.DoAction(input);

                    game.CurrentPlayerTakeAction(output.ActionTaken);

                    if (game.ReadyToDrawCard)
                    {
                        if (runMidSimulations)
                        {
                            var gameWinningSimulation = new GameWinningSimulation(1000, game);
                        
                            foreach (var player in game.Players)
                            {
                                var gamePlayerStrategy = PlayerStrategies.ContainsKey(player.PlayerNumber) ? PlayerStrategies[player.PlayerNumber] : DefaultStrategy;

                                if (gamePlayerStrategy is IReinforcementStrategy gameFeedbackStrategy)
                                {
                                    var playerSimulation = gameWinningSimulation.SimulationPlayers.FirstOrDefault(x => x.Player == player);
                                    gameFeedbackStrategy.SetRewardToLastMemory(new StrategyReward() { Value = playerSimulation.ExpectedGainLoss / game.EntirePot , ValueRepresentation = 0.8f });
                                }
                            }
                        }

                        game.StartNextRoundState();
                    }

                    if (playerStrategy is IReinforcementStrategy feedbackStrategy)
                    {
                        StrategyReward reward = new StrategyReward() { Value = 0, ValueRepresentation = 0 };
                        feedbackStrategy.AddToReplayMemory(input, output, reward);
                    }

                    HandleEndOfGame(i, game);

                    game.StartNewRoundIfRoundIsOver();

                    if (!runToEnd)
                    {
                        if (!game.Players.Any(x => PlayerStrategies[x.PlayerNumber] is IReinforcementStrategy))
                        {
                            break;
                        }
                    }

                    if (game.Done)
                    {
                        results.AddGameResult(new SimulationGameResult() { Winner = game.Winner.PlayerNumber, NumberOfRounds = game.RoundNumber });
                    }
                } 

                if (i % (NumberOfGames / 10) == 0)
                {
                    Console.WriteLine($"Game {i} is done");
                }
            }

            if(SimulatorCsvOuput != null)
            {
                SimulatorCsvOuput.WriteToCsv();
            }

            return results;
        }

        private void HandleEndOfGame(int gameNumber, Game game)
        {
            if (game.RoundIsOver)
            {
                foreach (var player in game.Players)
                {
                    var gamePlayerStrategy = PlayerStrategies.ContainsKey(player.PlayerNumber) ? PlayerStrategies[player.PlayerNumber] : DefaultStrategy;

                    if (gamePlayerStrategy is IReinforcementStrategy gameFeedbackStrategy)
                    {
                        gameFeedbackStrategy.SetRewardToLastMemory( new StrategyReward() {  Value = (player.RoundWinnings - player.RoundInvestment) / game.EntirePot, ValueRepresentation = 1.0f });

                        if (SimulatorCsvOuput != null)
                        {
                            SimulatorCsvOuput.Lines.Add(new SimulatorCsvOutputLine() { Game = gameNumber + 1, Player = player.PlayerNumber, Money = player.Money, Round = game.CurrentRound, QValue = gameFeedbackStrategy.LastActionsQValue });
                        }

                        gameFeedbackStrategy.ProcessEpisodeMemory();
                    }
                }
            }
        }
    }
}