using System;
using System.Collections.ObjectModel;
using System.Linq;
using TestP.Ai.Network;
using TestP.Lib.Models;
using TestP.Lib.Models.Games;
using TestP.Lib.Models.SimulationCore;
using TestP.Lib.Models.Strategies.Output;
using TestP.Wpf.Context;

namespace TestP.Wpf.PlayGame.VM
{
    public class BoardVM : ViewModelBase
    {
        private readonly Game game;
        private readonly IMainUpdate mainUpdate;
        private ObservableCollection<PlayerVM> players;

        public string Pot => string.Format("{0}$", game.Pot);

        public string InvestmentToPlay => string.Format("{0}$", game.CurrentRoundMinimumInvestment);

        public string Round => string.Format("Round {0}", game.RoundNumber);

        public ObservableCollection<CardVM> Cards { get; set; }

        public Command StartNewRound { get; set; }
        public Command NextRoundState { get; set; }
        public Command RunSimulation { get; set; }

        public NeuralNetwork NeuralNetwork { get; set; }

        public Command Check { get; set; }
        public string CheckText { get; set; } = "Check";

        public Command Raise { get; set; }
        public string RaiseText { get; set; } = "Raise";

        public Command Fold { get; set; }
        public string FoldText { get; set; } = "Fold";

        public ObservableCollection<ActionVM> Actions { get; set; }

        public BoardVM(Game game, IMainUpdate mainUpdate, NeuralNetwork neuralNetwork, ObservableCollection<PlayerVM> players)
        {
            NeuralNetwork = neuralNetwork;
            this.players = players;
            this.game = game;
            this.mainUpdate = mainUpdate;
            StartNewRound = new Command(RoundIsOverAction);
            NextRoundState = new Command(GoToNextRoundState, () => game.ReadyToDrawCard);
            RunSimulation = new Command(RunGameSimulation, () => game.ReadyToDrawCard);

            Check = new Command(CheckAction);
            Raise = new Command(RaiseAction);
            Fold = new Command(FoldAction);

            Actions = new ObservableCollection<ActionVM>();

            foreach (PlayerAction item in Enum.GetValues(typeof(PlayerAction)))
            {
                Actions.Add(new ActionVM(game, item, mainUpdate));
            }

            UpdateBoard();
        }


        public void GoToNextRoundState()
        {
            foreach (var player in players)
            {
                player.ClearSimulation();
            }

            game.StartNextRoundState();
            mainUpdate.Update();
        }

        public void RunGameSimulation()
        {
            var simulation = new GameWinningSimulation(1000, game);

            foreach (var item in players)
            {
                var simulationPlayer = simulation.SimulationPlayers.FirstOrDefault(x => x.Player == item.player);
                item.SetSimulation(simulationPlayer);
            }
            
            mainUpdate.Update();
        }

        public void RoundIsOverAction()
        {
            game.StartNewRoundIfRoundIsOver();
            mainUpdate.Update();
        }

        public void CheckAction()
        {
            game.CurrentPlayerCallsOrChecks();
            mainUpdate.Update();
        }

        public void FoldAction()
        {
            game.CurrentPlayerFolds();
            mainUpdate.Update();
        }

        public void RaiseAction()
        {
            game.CurrentPlayerRaises(1);
            mainUpdate.Update();
        }

        public void UpdateBoard()
        {
            var predictions = NeuralNetwork.Predict(StrategyState.Create(game));

            var actionsByPlayerAction = Actions.ToDictionary(x => x.PlayerAction);

            foreach (var prediction in predictions.AllActionValues)
            {
                var actionBtn = actionsByPlayerAction[prediction.Key];
                actionBtn.SetValue(prediction.Value);
            }

            /*RaiseText = string.Format("Raise\n{0}", prediction.AllActionValues[PlayerAction.BB1Raise]);
            CheckText = string.Format("Check\n{0}", prediction.AllActionValues[PlayerAction.Check]);
            FoldText = string.Format("Fold\n{0}", prediction.AllActionValues[PlayerAction.Fold]);*/

            Cards = new ObservableCollection<CardVM>(game.Cards.Select(x => new CardVM(x)));
            OnPropertyChanged(nameof(Cards));
            OnPropertyChanged(nameof(StartNewRound));
            OnPropertyChanged(nameof(Pot));
            OnPropertyChanged(nameof(InvestmentToPlay));
            OnPropertyChanged(nameof(Round));
            OnPropertyChanged(nameof(CheckAction));
            OnPropertyChanged(nameof(FoldAction));
            OnPropertyChanged(nameof(CheckAction));

            OnPropertyChanged(nameof(RaiseText));
            OnPropertyChanged(nameof(CheckText));
            OnPropertyChanged(nameof(FoldText));

            NextRoundState.CanExecuteHasChanged();
            RunSimulation.CanExecuteHasChanged();

            foreach (var action in Actions)
            {
                action.UpdateAction();
            }
        }
    }
}
