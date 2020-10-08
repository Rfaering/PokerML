using System;
using System.Collections.Generic;
using System.Linq;

namespace TestP.Lib.Models
{
    public class Deck
    {
        private static Random rng = new Random();        

        public static void SetSeed(int seed)
        {
            rng = new Random(seed);
        }

        public Deck()
        {
            var cards = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Value value in Enum.GetValues(typeof(Value)))
                {
                    cards.Add(new Card { Suit = suit, Value = value });
                }
            }

            Cards = cards;

            ShuffleCards();
        }

        public Deck(IEnumerable<Card> cards)
        {            
            Cards = new List<Card>(cards);
            ShuffleCards();
        }

        private void ShuffleCards()
        {
            int n = Cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = Cards[k];
                Cards[k] = Cards[n];
                Cards[n] = value;
            }
        }

        public IList<Card> Cards { get; private set; }

        private int currentIndex = 0;
        
        public ICollection<Card> TakeRandomCards(int numberOfCards)
        {
            var randomIndexHashSet = new HashSet<int>();

            while(randomIndexHashSet.Count < numberOfCards)
            {
                var index = rng.Next(currentIndex, Cards.Count);
                randomIndexHashSet.Add(index);
            }

            var result = new List<Card>();

            var randomIndex = randomIndexHashSet.ToList();

            for (int i = 0; i < randomIndexHashSet.Count; i++)
            {                
                result.Add(Cards[randomIndex[i]]);
            }

            return result;
        }

        public Card Draw()
        {
            var card = Cards[currentIndex];
            currentIndex++;
            return card;
        }        
    }
}
