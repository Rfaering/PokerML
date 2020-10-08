using System;
using System.Collections.Generic;
using System.Linq;

namespace TestP.Lib.Models.Games
{
    public class GameWinningSimulation
    {        
        public List<PlayerWinningSimulation> SimulationPlayers { get; set; } = new List<PlayerWinningSimulation>();

        public GameWinningSimulation(int simulations, Game game)
        {
            foreach (var player in game.Players)
            {
                SimulationPlayers.Add(new PlayerWinningSimulation(player));
            }

            for (int i = 0; i < simulations; i++)
            {
                var cards = game.Cards.ToList();

                var randomCards = game.Deck.TakeRandomCards(5 - cards.Count);

                foreach (var item in randomCards)
                {
                    cards.Add(item);
                }

                foreach (var simulation in SimulationPlayers.Where(x => !x.Player.Folded).ToList())
                {
                    simulation.SimulationHand = new Hand(cards.Union(simulation.Player.Cards).ToList());
                    simulation.ExpectedGainLoss -= simulation.Player.RoundInvestment;
                }

                var remainingPlayers = SimulationPlayers.Where(x => !x.Player.Folded && !x.Player.Done);
                var notDonePlayers = SimulationPlayers.Where(x => !x.Player.Done).ToList();

                var winners = SimulationPlayers.Where(x => !x.Player.Folded).GroupBy(x => x.SimulationHand.HandValue).OrderByDescending(x => x.Key);
                var numberOfWinners = winners.Count();

                var remainingPot = game.Pot;

                foreach (var winningGroup in winners)
                {
                    var fairSplit = remainingPot / winningGroup.Count();

                    var allIns = winningGroup.Where(x => x.Player.AllIn).OrderByDescending(x => x.Player.RoundInvestment).ToList();

                    if (allIns.Any())
                    {
                        foreach (var allInPlayer in allIns)
                        {
                            var maxWinning = Math.Min(notDonePlayers.Sum(player => Math.Min(player.Player.RoundInvestment, allInPlayer.Player.RoundInvestment)), fairSplit);

                            allInPlayer.ExpectedGainLoss += maxWinning;
                            remainingPot -= maxWinning;
                        }
                    }

                    var notAllIns = winningGroup.Where(x => !x.Player.AllIn).ToList();

                    if (notAllIns.Any())
                    {
                        var fairSplitAfterAllIns = remainingPot / notAllIns.Count();

                        foreach (var players in notAllIns)
                        {
                            players.ExpectedGainLoss += fairSplitAfterAllIns;
                            remainingPot -= fairSplitAfterAllIns;                            
                        }
                    }

                    if (remainingPot > 0 && remainingPot < 10)
                    {
                        winningGroup.First().ExpectedGainLoss += remainingPot;
                        remainingPot -= remainingPot;
                    }

                    if (remainingPot == 0)
                    {
                        break;
                    }
                }
            }

            foreach (var item in SimulationPlayers)
            {
                item.ExpectedGainLoss = item.ExpectedGainLoss / simulations;
            }
        }
    }
}
