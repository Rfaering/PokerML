using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TestP.Lib.Models;

namespace TestP.Wpf.PlayGame.VM
{
    public class CardVM
    {
        private Card _card;

        public ImageSource CardImage { get; set; }

        public string Name => _card.GetValueStringRepresentation();

        public CardVM(Card card)
        {
            _card = card;
            CardImage = GetImage(card.ToString());
        }

        public static BitmapImage GetImage(string path)
        {
            return new BitmapImage(new Uri(@"C:\Users\rfaer\source\repos\TestP\TestP.Wpf.PlayGame\Images\Cards\"+path+".png"));
        }
    }
}
