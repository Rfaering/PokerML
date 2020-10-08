using System;
using System.Collections.Generic;
using System.Linq;
using TestP.Lib.Core;
using TestP.Lib.Models.Strategies.Output;

namespace TestP.Lib.Models
{
    public class Game
    {
        private int _dealerIndex = 0;
        private int _currentPlayerIndex = 0;

        public int SmallBlindSize = 20;
        public int BigBlindSize => SmallBlindSize * 2;

        public int RoundNumber = 0;

        public int Pot { get; set; }

        public bool RoundIsOver { get; set; }

        public bool Done { get; set; }
        public Player Winner { get; set; }

        public int MaxRounds = 1000;
        public int CurrentRound { get; set; }

        public int CurrentRoundMinimumInvestment;
        
        public int PlayersRemainingInRoundToTakeAction => Players.Count(x => !x.Done && !x.AllIn && !x.Folded);
        public int PlayersRemainingInRound => Players.Count(x => !x.Done && !x.Folded);

        
        public bool PauseBetweenCardDraw { get; set; }
        public bool ReadyToDrawCard { get; set; }


        public List<Player> PlayersRemainingInRoundExceptCurrentPlayer => Players.Where(x => !x.Done && !x.Folded && x == CurrentPlayer).ToList();

        public IList<Player> Players { get; }
        public Deck Deck { get; private set; }

        public IList<Card> Cards { get; set; } = new List<Card>();
        
        public Player CurrentPlayer => GetPlayerByIndex(_currentPlayerIndex);

        public int PlayersStartCount { get; }

        public float EntirePot { get; set; }

        public Game(int numberOfPlayers, bool pauseBetweenCardDraw = false)
        {
            PauseBetweenCardDraw = pauseBetweenCardDraw;

            var players = new List<Player>();

            for (int i = 0; i < numberOfPlayers; i++)
            {
                players.Add(new Player(i+1));
            }

            EntirePot = Player.StartCash * numberOfPlayers;

            Players = players;
            CurrentRound = 0;
            _dealerIndex = -1;
            PlayersStartCount = numberOfPlayers;
        }

        public void StartNewRound()
        {
            if (Done)
            {
                return;
            }

            ReadyToDrawCard = false;
            RoundIsOver = false;
            _dealerIndex++;
            CurrentRound++;

            RoundNumber += 1;
            Deck = new Deck();
            Cards = new List<Card>();

            foreach (var player in Players.Where(x=>x.Done).ToList())
            {
                Players.Remove(player);
            }

            foreach (var player in Players)
            {
                player.StartNewRound(Deck);
            }

            GetPlayerByIndex(_dealerIndex).Dealer = true;

            Pot = 0;

            var smallBlind = GetPlayerByIndex(_dealerIndex + 1);
            var bigBlind = GetPlayerByIndex(_dealerIndex + 2);

            _currentPlayerIndex = GetPlayerIndex(_dealerIndex + 3);

            Pot += smallBlind.InvestMoneyInRound(SmallBlindSize);
            Pot += bigBlind.InvestMoneyInRound(BigBlindSize);

            CurrentRoundMinimumInvestment = BigBlindSize;

            Logger.LogGame($"Round {RoundNumber} - Pot {Pot}$");

            foreach (var player in Players)
            {
                player.LogState();
            }
        }

        public void CheckNextRountState()
        {
            var playersMoney = (Players.Sum(x => x.Money) + Pot);
            var startMoney = Player.StartCash* PlayersStartCount;
            
            if (playersMoney != startMoney)
            {
                throw new Exception($"Missing cash from system");
            }

            if(Players.Count(x => !x.Folded && !x.Done) == 1)
            {
                var notFolded = Players.First(x => !x.Folded);
                notFolded.GiveMoney(Pot);
                Pot = 0;
                Logger.LogGame($"Player {notFolded.Name} won {Pot}$");

                CheckWinner();
                return;
            }

            if(Cards.Count != 5)
            {
                ReadyToDrawCard = true;
                return;
            }
            
            if(Cards.Count == 5)
            {
                foreach (var player in Players.Where(x => !x.Folded).ToList())
                {
                    player.Hand = new Hand(Cards.Union(player.Cards).ToList());
                    Logger.LogGame($"Player {player.Name} - Best Hand: {string.Join(" ", player.Hand.BestHand)} - {player.Hand.BestHandDescription}");
                }

                var remainingPlayers = Players.Where(x => !x.Folded && !x.Done);
                var notDonePlayers = Players.Where(x => !x.Done).ToList();

                var winners = Players.Where(x => !x.Folded).GroupBy(x => x.Hand.HandValue).OrderByDescending(x => x.Key);
                var numberOfWinners = winners.Count();

                var remainingPot = Pot;

                foreach (var winningGroup in winners)
                {
                    var fairSplit = remainingPot / winningGroup.Count();
                    
                    var allIns = winningGroup.Where(x => x.AllIn).OrderByDescending(x => x.RoundInvestment).ToList();

                    if (allIns.Any())
                    {
                        foreach (var allInPlayer in allIns)
                        {
                            var maxWinning = Math.Min(notDonePlayers.Sum(player => Math.Min(player.RoundInvestment, allInPlayer.RoundInvestment)), fairSplit);

                            allInPlayer.GiveMoney(maxWinning);
                            remainingPot -= maxWinning;
                            Logger.LogGame($"Player {allInPlayer.Name} were all in with {allInPlayer.RoundInvestment} and won {maxWinning}$");
                        }
                    }

                    var notAllIns = winningGroup.Where(x => !x.AllIn).ToList();

                    if (notAllIns.Any())
                    {
                        var fairSplitAfterAllIns = remainingPot / notAllIns.Count();

                        foreach (var players in notAllIns)
                        {
                            players.GiveMoney(fairSplitAfterAllIns);
                            remainingPot -= fairSplitAfterAllIns;
                            Logger.LogGame($"Player {players.Name} won pot split of {fairSplitAfterAllIns}$");
                        }
                    }

                    if(remainingPot > 0 && remainingPot < 10)
                    {
                        winningGroup.First().GiveMoney(remainingPot);
                        Logger.LogGame($"Missing money were given to {winningGroup.First().Name} of {remainingPot}$");
                        remainingPot -= remainingPot;
                    }

                    if(remainingPot == 0)
                    {
                        break;
                    }
                }

                Pot = 0;

                CheckWinner();
            }
        }

        public void StartNextRoundState()
        {
            ReadyToDrawCard = false;

            if (Cards.Count == 0)
            {
                Cards.Add(Deck.Draw());
                Cards.Add(Deck.Draw());
                Cards.Add(Deck.Draw());

                foreach (var player in Players)
                {
                    player.HasTakenActionInRoundStage = false;
                }

                _currentPlayerIndex = GetPlayerIndex(_dealerIndex + 2);
                PassTurnToNextPlayer();

                Logger.LogGame($"Flop {string.Join(" ", Cards)} - Pot {Pot}$");
                return;
            }

            if (Cards.Count == 3)
            {
                Cards.Add(Deck.Draw());

                foreach (var player in Players)
                {
                    player.HasTakenActionInRoundStage = false;
                }

                _currentPlayerIndex = GetPlayerIndex(_dealerIndex + 2);
                PassTurnToNextPlayer();

                Logger.LogGame($"Turn {string.Join(" ", Cards)} - Pot {Pot}$");
                return;
            }

            if (Cards.Count == 4)
            {
                Cards.Add(Deck.Draw());

                foreach (var player in Players)
                {
                    player.HasTakenActionInRoundStage = false;
                }

                _currentPlayerIndex = GetPlayerIndex(_dealerIndex + 2);
                PassTurnToNextPlayer();

                Logger.LogGame($"River {string.Join(" ", Cards)} - Pot {Pot}$");
            }
        }

        private void CheckWinner()
        {
            foreach (var player in Players.Where(x => x.Money == 0))
            {
                player.Done = true;
            }

            if (Players.Count(x => !x.Done) == 1 || CurrentRound == MaxRounds)
            {
                Done = true;
                Winner = Players.Where(x=>!x.Done).OrderByDescending(x => x.Money).First();
            } else
            {
                RoundIsOver = true;
            }
        }

        public float CurrentPlayerTakeAction(PlayerAction playerAction)
        {
            if (RoundIsOver || ReadyToDrawCard)
            {
                return 0;
            }

            var currentPlayer = CurrentPlayer;

            switch (playerAction)
            {
                case PlayerAction.Fold:
                    return CurrentPlayerFolds();
                case PlayerAction.Check:
                    return CurrentPlayerCallsOrChecks();
                case PlayerAction.BB1Raise:
                    return CurrentPlayerRaises(1);
                case PlayerAction.BB2Raise:
                    return CurrentPlayerRaises(2);
                case PlayerAction.BB3Raise:
                    return CurrentPlayerRaises(3);
                case PlayerAction.BB4Raise:
                    return CurrentPlayerRaises(4);
                case PlayerAction.BB10Percent:
                    return CurrentPlayerRaises(currentPlayer.Money / (BigBlindSize * 10));
                case PlayerAction.BB25Percent:
                    return CurrentPlayerRaises(currentPlayer.Money / (BigBlindSize * 4));
                case PlayerAction.BB50Percent:
                    return CurrentPlayerRaises(currentPlayer.Money / (BigBlindSize * 2));
                case PlayerAction.BB100Percent:
                    return CurrentPlayerRaises(currentPlayer.Money / (BigBlindSize));
                default:
                    return 0;
            }
        }

        public float PriceToTakeAction(PlayerAction playerAction)
        {
            if (RoundIsOver || ReadyToDrawCard)
            {
                return 0;
            }

            var currentPlayer = CurrentPlayer;

            var missingAmountToInvest = CurrentRoundMinimumInvestment - currentPlayer.RoundInvestment;

            switch (playerAction)
            {
                case PlayerAction.Fold:
                    return 0;
                case PlayerAction.Check:
                    return Math.Min(currentPlayer.Money, missingAmountToInvest);
                case PlayerAction.BB1Raise:
                    return Math.Min(currentPlayer.Money, missingAmountToInvest + BigBlindSize);
                case PlayerAction.BB2Raise:
                    return Math.Min(currentPlayer.Money, missingAmountToInvest + BigBlindSize*2);
                case PlayerAction.BB3Raise:
                    return Math.Min(currentPlayer.Money, missingAmountToInvest + BigBlindSize*3);
                case PlayerAction.BB4Raise:
                    return Math.Min(currentPlayer.Money, missingAmountToInvest + BigBlindSize*4);
                case PlayerAction.BB10Percent:
                    return Math.Min(currentPlayer.Money, missingAmountToInvest + currentPlayer.Money / 10);
                case PlayerAction.BB25Percent:
                    return Math.Min(currentPlayer.Money, missingAmountToInvest + currentPlayer.Money / 4);
                case PlayerAction.BB50Percent:
                    return Math.Min(currentPlayer.Money, missingAmountToInvest + currentPlayer.Money / 2);
                case PlayerAction.BB100Percent:
                    return Math.Min(currentPlayer.Money, missingAmountToInvest + currentPlayer.Money);
                default:
                    return 0;
            }
        }

        public float CurrentPlayerFolds()
        {
            if (RoundIsOver || ReadyToDrawCard)
            {
                return 0;
            }

            var currentPlayer = CurrentPlayer;

            currentPlayer.HasTakenActionInRoundStage = true;
            currentPlayer.Folded = true;

            Logger.LogGame($"{currentPlayer.Name} - Folds");

            PassTurnToNextPlayer();

            return GetCurrentActionsValue(currentPlayer);
        }

        public float CurrentPlayerCallsOrChecks()
        {
            if (RoundIsOver || ReadyToDrawCard)
            {
                return 0;
            }

            var currentPlayer = CurrentPlayer;

            currentPlayer.HasTakenActionInRoundStage = true;
            var moneyInvested = currentPlayer.InvestMoneyInRound(CurrentRoundMinimumInvestment - currentPlayer.RoundInvestment);
            Pot += moneyInvested;

            if(moneyInvested == 0)
            {
                Logger.LogGame($"{currentPlayer.Name} - Checks");
            }
            else 
            {
                Logger.LogGame($"{currentPlayer.Name} - Calls {moneyInvested}");
            }


            PassTurnToNextPlayer();

            return GetCurrentActionsValue(currentPlayer);
        }

        public float CurrentPlayerRaises(int bigBlindMultiplier)
        {
            if (RoundIsOver || ReadyToDrawCard)
            {
                return 0;
            }

            var currentPlayer = CurrentPlayer;

            CurrentRoundMinimumInvestment += BigBlindSize * bigBlindMultiplier;
            var moneyInvested = currentPlayer.InvestMoneyInRound(CurrentRoundMinimumInvestment - currentPlayer.RoundInvestment);
            Pot += moneyInvested;

            currentPlayer.HasTakenActionInRoundStage = true;

            Logger.LogGame($"{currentPlayer.Name} - Raises {moneyInvested}");

            PassTurnToNextPlayer();
            
            return GetCurrentActionsValue(currentPlayer);
        }



        private int GetCurrentActionsValue(Player currentPlayer)
        {
            var value = 0;

            if (RoundIsOver || currentPlayer.Folded || currentPlayer.Done)
            {
                value = currentPlayer.RoundWinnings - currentPlayer.RoundInvestment;
            }

            return value;
        }

        public void StartNewRoundIfRoundIsOver()
        {
            if (RoundIsOver)
            {
                StartNewRound();
            }
        }

        public (int beforeCurrentPlayer, int afterCurrentPlayer) GetOtherActivePlayers()
        {
            var currentPlayerIndex = _currentPlayerIndex;

            var playersBeforeYou = 0;
            var playersAfterYou = 0;

            bool hasPassedButton = false;

            for (int i = 1; i < Players.Count; i++)
            {
                var otherPlayerIndex = GetPlayerIndex(currentPlayerIndex + i);
                var otherPlayer = Players[otherPlayerIndex];

                if ( otherPlayer.Folded || otherPlayer.Done)
                {
                    continue;
                }

                if (otherPlayerIndex == _dealerIndex)
                {
                    hasPassedButton = true;
                }

                if (hasPassedButton)
                {
                    playersAfterYou++;
                } else
                {
                    playersBeforeYou++;
                }
                
            }

            return (playersBeforeYou, playersAfterYou);
        }

        private Player GetPlayerByIndex(int index)
        {
            return Players[GetPlayerIndex(index)];
        }

        private void PassTurnToNextPlayer()
        {
            var playersRemainingInRound = Players.Where(x => !x.Done && !x.Folded);

            if(playersRemainingInRound.Count() == 1)
            {
                CheckNextRountState();
            }

            var playersRemainingInRoundToTakeAction = playersRemainingInRound.Where(x => !x.AllIn && (x.RoundInvestment != CurrentRoundMinimumInvestment || !x.HasTakenActionInRoundStage)).ToHashSet();
            
            for (int i = 1; i <= Players.Count; i++)
            {
                var potentionalNextPlayerIndex = GetPlayerIndex(_currentPlayerIndex + i);
                var potentionalNextPlayer = GetPlayerByIndex(potentionalNextPlayerIndex);

                if (playersRemainingInRoundToTakeAction.Contains(potentionalNextPlayer))
                {
                    _currentPlayerIndex = potentionalNextPlayerIndex;
                    return;
                }
            }

            CheckNextRountState();                            
        }

        private int GetPlayerIndex(int index)
        {
            return index % Players.Count;
        }
    }
}
