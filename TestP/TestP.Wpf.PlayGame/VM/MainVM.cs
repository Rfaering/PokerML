using System.Collections.ObjectModel;
using System.Linq;
using TestP.Ai.Network;
using TestP.Ai.Simulations;
using TestP.Lib.Models;
using TestP.Wpf.Context;

namespace TestP.Wpf.PlayGame.VM
{
    public class MainVM : ViewModelBase, IMainUpdate
    {
        public BoardVM Board { get; set; }

        public ObservableCollection<PlayerVM> Players { get; set; }

        public MainVM()
        {            
            var neuralNetwork = new NeuralNetwork("Test");

            var game = new Game(6);
            
            Players = new ObservableCollection<PlayerVM>(game.Players.Select(x => new PlayerVM(x, game)));

            Board = new BoardVM(game, this, neuralNetwork, Players);

            game.StartNewRound();

            Board.UpdateBoard();

            foreach (var player in Players)
            {
                player.UpdatePlayer();
            }           
        }

        public void Update()
        {
            Board.UpdateBoard();
            
            foreach (var player in Players)
            {
                player.UpdatePlayer();
            }
        }
    }
}
