using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestP.Lib.Models.SimulationCore
{
    public class StrategyState
    {
        public float PotSize { get; set; }
        public float Money { get; set; }
        public float MoneyToCall { get; set; }
        public float PlayersRemaining { get; set; }

        public int ActivePlayersBeforeYou { get; set; }
        public int ActivePlayersAfterYou { get; set; }
        
        public List<Card> OwnCards { get; set; }
        public List<Card> BoardCards { get; set; }


        public static int StateSize = 20;

        public static StrategyState Create(Game game)
        {
            var active = game.GetOtherActivePlayers();

            var playersRemaining = game.PlayersRemainingInRoundExceptCurrentPlayer;

            return new StrategyState()
            {
                ActivePlayersBeforeYou = active.beforeCurrentPlayer,
                ActivePlayersAfterYou = active.afterCurrentPlayer,
                PlayersRemaining = playersRemaining.Any() ? playersRemaining.Sum(x => x.Money) / game.EntirePot : 0,
                MoneyToCall = game.PriceToTakeAction(Strategies.Output.PlayerAction.Check) / game.EntirePot,
                PotSize = game.Pot / game.EntirePot,
                Money = game.CurrentPlayer.Money / game.EntirePot,
                OwnCards = game.CurrentPlayer.Cards.ToList(),
                BoardCards = game.Cards.ToList()
            };
        }
        
        public float[] ToNeuralNetworkState()
        {
            List<float> floats = new List<float>();

            floats.Add(MoneyToCall);
            floats.Add(Money);
            floats.Add(PotSize);
            floats.Add(PlayersRemaining);
            floats.Add(ActivePlayersBeforeYou);
            floats.Add(ActivePlayersAfterYou);

            for (int i = 0; i < 2; i++)
            {
                if (OwnCards.Count > i)
                {
                    floats.Add((int)OwnCards[i].Suit);
                    floats.Add((int)OwnCards[i].Value);
                }
                else
                {
                    floats.Add(0);
                    floats.Add(0);
                }
            }

            for (int i = 0; i < 5; i++)
            {
                if (BoardCards.Count > i)
                {
                    floats.Add((int)BoardCards[i].Suit);
                    floats.Add((int)BoardCards[i].Value);
                }
                else
                {
                    floats.Add(0);
                    floats.Add(0);
                }
            }

            return floats.ToArray();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19;

                // hash = hash * 31 + PotSizeInBB.GetHashCode();
                // hash = hash * 31 + MoneyInBB.GetHashCode();

                foreach (var ownCard in OwnCards)
                {
                    hash = hash * 31 + ownCard.GetHashCode();
                }

                foreach (var boardCard in BoardCards)
                {
                    hash = hash * 31 + boardCard.GetHashCode();
                }

                return hash;
            }
        }

        public string GetUniqueString()
        {
            var cards = string.Join(',', OwnCards.Select(x => x.ToString()).ToArray());
            var boardCards = string.Join(',', BoardCards.Select(x => x.ToString()).ToArray());

            return $"{cards} {boardCards}";
        }

        public override string ToString()
        {
            var cards = string.Join(',', OwnCards.Select(x => x.ToString()).ToArray());
            var boardCards = string.Join(',', BoardCards.Select(x => x.ToString()).ToArray());

            return $"OwnCards: {cards} BoardCards: {boardCards}";
        }

        public string ToShortString()
        {
            var cards = string.Join(',', OwnCards.Select(x => x.ToString()).ToArray());
            var boardCards = string.Join(',', BoardCards.Select(x => x.ToString()).ToArray());

            return $"{cards} {boardCards}";
        }
    }
}
