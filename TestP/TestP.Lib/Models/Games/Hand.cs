using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestP.Lib.Models
{
    public class Hand
    {
        public static string RoyalFlush = nameof(RoyalFlush);
        public static string StraightFlush = nameof(StraightFlush);        
        public static string FourOfAKind = nameof(FourOfAKind);
        public static string FullHouse = nameof(FullHouse);
        public static string Flush = nameof(Flush);
        public static string Straight = nameof(Straight);
        public static string ThreeOfAKind = nameof(ThreeOfAKind);
        public static string TwoPairs = nameof(TwoPairs);
        public static string Pair = nameof(Pair);
        public static string HighCard = nameof(HighCard);


        public List<Card> Cards { get; set; }
        public List<Card> BestHand { get; set; }
        public string BestHandDescription { get; set; }

        public int HandValue { get; set; }

        public Hand(List<Card> cards)
        {
            Initialize(cards);
        }

        public Hand(string hand)
        {
            var cards = hand.Split(" ").Select(x => new Card(x)).ToList();
            Initialize(cards);
        }

        private void Initialize(List<Card> cards)
        {
            Cards = cards;

            var bestStraighFlush = GetBestStraightFlush();

            if (bestStraighFlush != null)
            {
                BestHand = bestStraighFlush;

                if (bestStraighFlush.Any(x => x.Value == Value.Ace) && bestStraighFlush.Any(x => x.Value == Value.Ten))
                {
                    BestHandDescription = RoyalFlush;
                    HandValue = 1000000;
                }
                else
                {
                    BestHandDescription = StraightFlush;
                    HandValue = 900000 + DiffValue(BestHand);
                }

                return;
            }

            var fourOfAKind = GetFourOfAKind();

            if (fourOfAKind.Any())
            {
                var topFourOfAKind = fourOfAKind.First();                
                BestHand = FillRestOfCards(topFourOfAKind);
                HandValue = 800000 + DiffValue(BestHand);
                BestHandDescription = FourOfAKind;
                return;
            }

            var threeOfAKind = GetAllThreeOfAKind();
            var twoOfAKind = GetAllTwoOfAKind();

            if(threeOfAKind.Any() && twoOfAKind.Any())
            {
                var bestThreeOfAKind = threeOfAKind.First();
                var bestTwoOfAKind = twoOfAKind.First();
                var result = new List<Card>();

                result.AddRange(bestThreeOfAKind);
                result.AddRange(bestTwoOfAKind);

                BestHand = result;
                BestHandDescription = FullHouse;
                HandValue = 700000 + DiffValue(BestHand);
                return;
            }

            var bestFlush = GetBestFlush();

            if (bestFlush != null)
            {
                BestHand = bestFlush;
                BestHandDescription = Flush;
                HandValue = 600000 + DiffValue(BestHand);
                return;
            }

            var bestStraight = GetBestStraight();

            if (bestStraight != null)
            {
                BestHand = bestStraight;
                BestHandDescription = Straight;
                HandValue = 500000 + DiffValue(BestHand);
                return;
            }

            if (threeOfAKind.Any())
            {                
                BestHand = FillRestOfCards(threeOfAKind.First());
                BestHandDescription = ThreeOfAKind;
                HandValue = 400000 + DiffValue(BestHand);
                return;
            }

            if (twoOfAKind.Count >= 2)
            {                
                BestHand = FillRestOfCards(twoOfAKind.Take(2).SelectMany(x => x).ToList());
                BestHandDescription = TwoPairs;
                HandValue = 300000 + DiffValue(BestHand);
                return;
            }

            if (twoOfAKind.Any())
            {                
                BestHand = FillRestOfCards(twoOfAKind.First());
                BestHandDescription = Pair;
                HandValue = 200000 + DiffValue(BestHand);
                return;
            }

            BestHand = FillRestOfCards(new List<Card>());
            HandValue = 100000 + DiffValue(BestHand);
            BestHandDescription = HighCard;
        }


        private List<List<Card>> GetAllTwoOfAKind()
        {
            return GetAllXOfAKind(2);
        }

        private List<List<Card>> GetAllThreeOfAKind()
        {
            return GetAllXOfAKind(3);
        }

        private List<List<Card>> GetFourOfAKind()
        {
            return GetAllXOfAKind(4);
        }

        private List<List<Card>> GetAllXOfAKind(int xNumber)
        {
            var result = new List<List<Card>>();

            foreach (var cardGroup in Cards.GroupBy(x => x.Value).OrderByDescending(x=>x.Key))
            {
                if (cardGroup.Count() == xNumber)
                {
                    result.Add(new List<Card>(cardGroup));
                }
            }

            return result;
        }

        private List<Card> GetBestFlush()
        {
            foreach (var group in Cards.GroupBy(x => x.Suit))
            {
                if (group.Count() >= 5)
                {
                    return group.OrderByDescending(x => x.Value).Take(5).ToList();
                }
            }

            return null;
        }

        private List<Card> GetBestStraightFlush()
        {
            foreach (var item in Cards.GroupBy(x => x.Suit))
            {
                if (item.Count() >= 5)
                {
                    var possibleStraight = GetStraight(item.ToList());
                    if (possibleStraight != null)
                    {
                        return possibleStraight;
                    }
                }
            }

            return null;
        }

        private List<Card> FillRestOfCards(List<Card> cards)
        {
            var orderedByValue = Cards.OrderByDescending(x => x.Value).ThenBy(x => x.Suit);
            var missingCards = (5 - cards.Count);
            if (missingCards > 0)
            {
                var copyList = cards.ToList();
                copyList.AddRange(orderedByValue.Where(x => !copyList.Contains(x)).Take(missingCards));
                return copyList;
            }

            return cards;
        }

        public List<Card> GetBestStraight()
        {
            return GetStraight(Cards);
        }

        private static List<Card> GetStraight(IList<Card> cards)
        {
            var straightList = new List<Value>()
                {
                    Value.Ace,
                    Value.King,
                    Value.Queen,
                    Value.Jack,
                    Value.Ten,
                    Value.Nine,
                    Value.Eight,
                    Value.Seven,
                    Value.Six,
                    Value.Five,
                    Value.Four,
                    Value.Three,
                    Value.Two,
                    Value.Ace,
                };


            for (int i = 0; i < straightList.Count - 4; i++)
            {
                if (Contains(cards, straightList[i]) && Contains(cards, straightList[i + 1]) && Contains(cards, straightList[i + 2]) && Contains(cards, straightList[i + 3]) && Contains(cards, straightList[i + 4]))
                {
                    return new List<Card>() { GetFirst(cards, straightList[i]), GetFirst(cards, straightList[i + 1]), GetFirst(cards, straightList[i + 2]), GetFirst(cards, straightList[i + 3]), GetFirst(cards, straightList[i + 4]) };
                }
            }

            return null;
        }

        private int DiffValue(List<Card> cards)
        {
            int result = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                result += (20 * (cards.Count - i)) * (int)cards[i].Value;
            }

            return result;
        }

        public static bool Contains(ICollection<Card> cards, Value val)
        {
            return cards.Where(c => c.Value == val).Any();
        }

        public static Card GetFirst(ICollection<Card> cards, Value val)
        {
            return cards.First(c => c.Value == val);
        }

        public override string ToString()
        {
            var s = "";
            s += string.Join(" ", BestHand) + $" - {BestHandDescription} - {HandValue}";
            return s;
        }
    }
}

