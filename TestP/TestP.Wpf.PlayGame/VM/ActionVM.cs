using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Windows.Media;
using TestP.Lib.Models;
using TestP.Lib.Models.Strategies.Output;
using TestP.Wpf.Context;

namespace TestP.Wpf.PlayGame.VM
{
    public class ActionVM : ViewModelBase
    {
        public Command Command { get; set; }
        public string Text { get; set; } = "Fold";
        public string CostToTakeAction { get; set; } = string.Empty;
        public Game Game { get; }
        public PlayerAction PlayerAction { get; }
        public IMainUpdate MainUpdate { get; }

        public SolidColorBrush Background { get; set; }

        public string Value { get; set; }

        public ActionVM(Game game, PlayerAction playerAction, IMainUpdate mainUpdate)
        {
            Text = playerAction.ToString();
            Command = new Command(TakeAction);
            Game = game;
            PlayerAction = playerAction;
            MainUpdate = mainUpdate;
        }

        private void TakeAction()
        {
            Game.CurrentPlayerTakeAction(PlayerAction);
            MainUpdate.Update();
        }

        public void SetValue(double value)
        {
            Value = string.Format("{0}", value);

            var usedValue = value;

            var max = 1.0;

            if(usedValue < -max)
            {
                usedValue = -max;
            }
            if (usedValue > max)
            {
                usedValue = max;
            }

            if (usedValue >= 0)
            {
                Background = CreateBrush(131, usedValue / max, 0.9);
            }
            if (usedValue < 0)
            {
                Background = CreateBrush(0, -usedValue / max, 0.9);
            }
        }

        public void UpdateAction()
        {
            /*var prediction = NeuralNetwork.Predict(StrategyState.Create(game));

            RaiseText = string.Format("Raise\n{0}", prediction.AllActionValues[PlayerAction.BB1Raise]);
            CheckText = string.Format("Check\n{0}", prediction.AllActionValues[PlayerAction.Check]);
            FoldText = string.Format("Fold\n{0}", prediction.AllActionValues[PlayerAction.Fold]);*/

            var priceToTakeAction = Game.PriceToTakeAction(PlayerAction);
            

            if (PlayerAction == PlayerAction.Check) 
            {
                Text = priceToTakeAction > 0 ? "Call" : "Check";
            }

            CostToTakeAction = string.Format("{0:0.##} $", priceToTakeAction);


            OnPropertyChanged(nameof(Background));
            OnPropertyChanged(nameof(Text));
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(CostToTakeAction));
        }

        public SolidColorBrush CreateBrush(double hue, double saturation, double value)
        {
            return new SolidColorBrush(ColorFromHSV(hue, saturation, value));
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            byte v = (byte)Convert.ToInt32(value);
            byte p = (byte)Convert.ToInt32(value * (1 - saturation));
            byte q = (byte)Convert.ToInt32(value * (1 - f * saturation));
            byte t = (byte)Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
    }
}
