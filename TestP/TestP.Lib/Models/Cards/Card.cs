using System;
using System.Collections.Generic;
using System.Text;

namespace TestP.Lib.Models
{
    public class Card
    {
        public Suit Suit { get; set; }
        public Value Value { get; set; }

        public Card()
        {

        }

        public Card(string card)
        {
            var firstChar = card[0];
            var rest = card.Substring(1);

            if(firstChar == 'C')
            {
                Suit = Suit.Clubs;
            }
            else if (firstChar == 'D')
            {
                Suit = Suit.Diamonds;
            }
            else if (firstChar == 'H')
            {
                Suit = Suit.Hearts;
            }
            else if (firstChar == 'S')
            {
                Suit = Suit.Spades;
            } else
            {
                throw new ArgumentException("Cannot parse suit");
            }

            if(rest == "A")
            {
                Value = Value.Ace;
            }
            else if (rest == "2")
            {
                Value = Value.Two;
            }
            else if (rest == "3")
            {
                Value = Value.Three;
            }
            else if (rest == "4")
            {
                Value = Value.Four;
            }
            else if (rest == "5")
            {
                Value = Value.Five;
            }
            else if (rest == "6")
            {
                Value = Value.Six;
            }
            else if (rest == "7")
            {
                Value = Value.Seven;
            }
            else if (rest == "8")
            {
                Value = Value.Eight;
            }
            else if (rest == "9")
            {
                Value = Value.Nine;
            }
            else if (rest == "10")
            {
                Value = Value.Ten;
            }
            else if (rest == "J")
            {
                Value = Value.Jack;
            }
            else if (rest == "Q")
            {
                Value = Value.Queen;
            }
            else if (rest == "K")
            {
                Value = Value.King;
            }
            else
            {
                throw new ArgumentException("Cannot parse value");
            }

        }

        public override string ToString()
        {
            var s = "";

            s += GetSuitStringValue();
            s += GetValueStringRepresentation();

            return s;
        }

        private string GetSuitStringValue()
        {
            switch (Suit)
            {
                case Suit.Clubs:
                    return "C";                    
                case Suit.Diamonds:
                    return "D";                    
                case Suit.Hearts:
                    return "H";                    
                case Suit.Spades:
                    return "S";                    
                default:
                    return string.Empty;
            }
        }

        public string GetValueStringRepresentation()
        {
            switch (Value)
            {
                case Value.Ace:
                    return "A";
                case Value.Two:
                    return "2";
                case Value.Three:
                    return  "3";
                case Value.Four:
                    return  "4";
                case Value.Five:
                    return  "5";
                case Value.Six:
                    return  "6";
                case Value.Seven:
                    return  "7";
                case Value.Eight:
                    return  "8";
                case Value.Nine:
                    return  "9";
                case Value.Ten:
                    return  "10";
                case Value.Jack:
                    return  "J";
                case Value.Queen:
                    return  "Q";
                case Value.King:
                    return  "K";
                default:
                    return string.Empty;                    
            }
        }

        public override bool Equals(object obj)
        {
            if(obj is Card card)
            {
                var result = card.Suit == Suit && card.Value == Value; ;
                return result;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Suit, Value);
        }
    }
}
