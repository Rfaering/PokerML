using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using TestP.Ai.Network;
using TestP.Lib.Models;
using TestP.Lib.Models.Games;
using TestP.Wpf.Context;

namespace TestP.Wpf.PlayGame.VM
{
    public class PlayerVM : ViewModelBase
    {
        private PlayerWinningSimulation simulation;
        private Game game;
        public Player player;

        public string Name => player.Name;

        public string Money => string.Format("{0}$",player.Money);

        public bool Done => player.Done;

        public string Hand { get; set; }
        public string Winnings => player.RoundWinnings > 0 ? string.Format("+ {0}$", player.RoundWinnings) : string.Empty; 
        public string Dealer => player.Dealer ? "Dealer" : string.Empty;

        public string ExpectedReturn => simulation != null ? string.Format("{0}$", simulation.ExpectedGainLoss) : string.Empty;

        public SolidColorBrush Background { get; set; }        

        public string MoneyInPot => string.Format("- {0}$", player.RoundInvestment);

        public bool Active => game.CurrentPlayer == player && !player.Done && !player.RoundIsDone && !player.Folded;

        public ObservableCollection<CardVM> Cards { get; set; }
        
        public PlayerVM(Player player, Game game)
        {
            this.game = game;
            this.player = player;
            
            UpdatePlayer();
        }

        public void SetSimulation(PlayerWinningSimulation playerSimulation)
        {
            simulation = playerSimulation;
        }

        public void ClearSimulation()
        {
            simulation = null;
        }

        public void UpdatePlayer()
        {
            Cards = new ObservableCollection<CardVM>(player.Cards.Select(x => new CardVM(x)));

            if (game.Cards.Any())
            {
                var cardsList = game.Cards.ToList();
                cardsList.AddRange(player.Cards);

                Hand = new Hand(cardsList).BestHandDescription;
            } else
            {
                Hand = string.Empty;
            }


            if (player.Folded)
            {
                Background = Color("#595959"); 
            }
            else if (player.Done)
            {
                Background = Color("#8a7676");
            }
            else
            {
                Background = Color("#bdbdbd");
            }

            if (Active)
            {
                Background = Color("#45b055");
            }
            
            OnPropertyChanged(nameof(Background));

            OnPropertyChanged(nameof(Winnings));
            OnPropertyChanged(nameof(Dealer));

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Cards));
            OnPropertyChanged(nameof(Money));
            OnPropertyChanged(nameof(Hand));
            OnPropertyChanged(nameof(MoneyInPot));
            OnPropertyChanged(nameof(Active));

            OnPropertyChanged(nameof(ExpectedReturn));
        }

        public SolidColorBrush Color(string html)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(html));
        }
    }
}

