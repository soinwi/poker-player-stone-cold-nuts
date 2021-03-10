using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Simple
{
    class CardRecognizer
    {

        public static bool CheckPair(List<Card> cards)
        {
            //see if exactly 2 cards card the same rank.
            return cards.GroupBy(card => card.Rank).Count(group => group.Count() == 2) == 1;
        }

        public static bool CheckTwoPair(List<Card> cards)
        {
            //see if there are 2 lots of exactly 2 cards card the same rank.
            return cards.GroupBy(card => card.Rank).Count(group => group.Count() >= 2) == 2;
        }

        public static bool CheckTrips(List<Card> cards)
        {
            //see if exactly 3 cards card the same rank.
            return cards.GroupBy(card => card.Rank).Any(group => group.Count() == 3);
        }
        public static bool CheckStraight(List<Card> cards)
        {
            // order by decending to see order
            var cardsInOrder = cards.OrderByDescending(a => a.Value).ToList();
            // check for ace as can be high and low
            if (cardsInOrder.First().Rank == "A")
            {
                // check if straight with ace has has 2 values
                bool highStraight = cards.Where(a => a.Rank == "K" || a.Rank == "Q" || a.Rank == "J" || a.Rank == "10").Count() == 4;
                bool lowStraight = cards.Where(a => a.Rank == "2" || a.Rank == "3" || a.Rank == "4" || a.Rank == "5").Count() == 4;
                // return true if straight with ace
                if (lowStraight == true || highStraight == true)
                {
                    return true;
                }
            }
            else
            {
                // check for straight here
                return true;
            }
            // no straight if reached here.
            return false;

        }

        public static bool CheckFlush(List<Card> cards)
        {
            //see if 5 or more cards card the same rank.
            return cards.GroupBy(card => card.Suit).Count(group => group.Count() >= 5) == 1;
        }

        public static bool CheckFullHouse(List<Card> cards)
        {
            // check if trips and pair is true
            return CheckPair(cards) && CheckTrips(cards);
        }
        public static bool CheckQuads(List<Card> cards)
        {
            //see if exactly 4 cards card the same rank.
            return cards.GroupBy(card => card.Rank).Any(group => group.Count() == 4);
        }

        // need to check same 5 cards
        public static bool CheckStraightFlush(List<Card> cards)
        {
            // check if flush and straight are true.
            return CheckFlush(cards) && CheckStraight(cards);
        }
    }
}
