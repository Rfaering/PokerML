using System;
using System.Collections.Generic;
using System.Text;
using TestP.Lib.Core;

namespace TestP.Lib.Models
{
    public class Player
    {
        public const int StartCash = 1500;

        public Player(int playerNumber)
        {
            PlayerNumber = playerNumber;
            Money = StartCash;
            Cards = new List<Card>();
        }
        
        public int Money { get; set; }
        public IList<Card> Cards { get; set; }
        public int PlayerNumber { get; }

        public string Name => $"Player {PlayerNumber}";

        public bool Dealer { get; set; }
        public bool Folded { get; set; }
        public bool HasTakenActionInRoundStage { get; set; }
        public int RoundInvestment { get; set; }
        public int RoundWinnings { get; set; }

        public bool RoundIsDone { get; set; }

        public Hand Hand { get; set; }

        public bool AllIn => Money == 0 && !Done;

        public bool Done { get; internal set; }

        public void StartNewRound(Deck deck)
        {
            Hand = null;
            Dealer = false;
            HasTakenActionInRoundStage = false;
            Folded = false;
            RoundWinnings = 0;
            RoundInvestment = 0;
            Cards.Clear();
            
            if (!Done)
            {
                Cards.Add(deck.Draw());
                Cards.Add(deck.Draw());
            }
        }

        internal void LogState()
        {
            Logger.LogGame($"{Name} - {Money}$ - {string.Join(" ", Cards)}");
        }

        internal void GiveMoney(int money)
        {
            Money += money;            
            RoundWinnings += money;
        }

        internal int InvestMoneyInRound(int money)
        {
            if(money > Money)
            {
                var allIn = Money;
                Money = 0;
                RoundInvestment += allIn;
                return allIn;
            }

            Money -= money;
            RoundInvestment += money;
            return money;
        }
    }
}
