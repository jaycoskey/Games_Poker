// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker
{
    public partial class Hand : CardList<HandCard>
    {
        #region Lifecycle
        public Hand()
        {
            handCards = new List<HandCard>();
        }
        public Hand(Card c1, Card c2, Card c3, Card c4, Card c5)
            : this()
        {
            addCards(new Card[] { c1, c2, c3, c4, c5 }, 0);
            
        }
        public Hand(Card c1, Card c2, Card c3, Card c4, Card c5, Card c6, Card c7)
            : this()
        {
            handCards = new List<HandCard>();
            addCards(new Card[] { c1, c2, c3, c4, c5, c6, c7 }, 0);
        }
        public Hand(IEnumerable<Card> cs)
            : this()
        {
            addCards(cs, 0);
        }
        #endregion // Lifecycle

        #region Public data/properties/methods

        public override string ToString()
        {
            return "(" + String.Join(", ", handCards) + ")";
        }

        public static void WriteHandData(
            Hand hand,
            IEnumerable<Card> wilds,
            FullRank fRank,
            HandCard[] hcsRank,
            HandCard[] hcsFill)
        {
            Console.WriteLine("Hand = {0:s}", hand.ToString());
            if (wilds != null && wilds.Count() > 0)
            {
                string wildsStr = String.Join(", ", wilds.Select(wc => wc.ToString()));
                int numWildsInHand = wilds
                    .Count(wc => hand.Cards.Count(hc => hc.Value == wc.Value && hc.Suit == wc.Suit) > 0);
                Console.WriteLine("\tWilds cards= {0:s}{1:s}",
                    wildsStr,
                    numWildsInHand == 0
                        ? String.Empty
                        : String.Format(" ({0:d} wild card{1:s} in hand)",
                            numWildsInHand, numWildsInHand > 1 ? "s" : String.Empty));
            }
            Console.WriteLine("\tFull rank  = {0:s}", fRank.ToString());

            string hcsRankStr = String.Join(", ",
                hcsRank.Select(hc => hc == null ? "NULL" : hc.ToLongString()));
            Console.WriteLine("\tCards forming rank = {0:s}",
                hcsRankStr.Length == 0 ? "NONE" : hcsRankStr);

            string hcsFillStr = String.Join(", ",
                hcsFill.Select(hc => hc == null ? "NULL" : hc.ToLongString()));
            Console.WriteLine("\tCards forming fill = {0:s}",
                hcsFillStr.Length == 0 ? "NONE" : hcsFillStr);
        }
        #endregion Public data/properties/methods

        #region Private methods
        private void addCards(IEnumerable<Card> cs, int startingIndex)
        {
            int index = startingIndex;
            foreach (Card c in cs)
            {
                handCards.Add(new HandCard(c, index++));
            }
        }
        #endregion // Private methods
    }
}