// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker
{
    class Deck : CardList<Card>
    {
        /// <remarks>Unused</remarks>
        public List<Card> DealCards(int numCards)
        {
            List<Card> result = handCards.Take(numCards).ToList();
            handCards.RemoveRange(0, numCards);
            return result;
        }

        /// <remarks>Unused</remarks>
        public List<Card>[] DealHands(int handSize, int numHands)
        {
            List<Card>[] hands = new List<Card>[numHands];
            for (int i = 0; i < handSize; i++)
            {
                hands[i] = new List<Card>();
            }
            for (int i = 0; i < handSize; i++)
            {
                foreach (List<Card> hand in hands)
                {
                    hand.Add(handCards[0]);
                    handCards.RemoveAt(0);
                }
            }
            return hands;
        }
    }
}
