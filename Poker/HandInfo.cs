// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker
{
    public class HandInfo
    {
        public HandInfo(Hand h, IEnumerable<Card> wilds)
        {
            if (wilds == null || wilds.Count() == 0)
            {
                wildCards = new Card[0];
            }
            else
            {
                wildCards = wilds.ToArray();
            }

            hcsUnbound = h.Cards.Where(hc => hc.IsJoker() || IsWild(hc)).ToArray();
            hcsPlain = h.Cards.Except(hcsUnbound).ToArray();

            var suitHistolist = hcsPlain
                .GroupBy(hc => hc.Suit)
                .Select(grp => new { Suit = grp.Key, Count = grp.Count() })
                .OrderByDescending(grp => grp.Count).ToList();
            suitGram = new Dictionary<TSuit, int>();
            foreach (var suitData in suitHistolist) {
                suitGram[suitData.Suit] = suitData.Count;
            }

            var valueHistolist = hcsPlain
                .GroupBy(hc => hc.Value)
                .Select(grp => new { Value = grp.Key, Count = grp.Count() })
                .OrderByDescending(grp => grp.Count).ToList();
            valueGram = new Dictionary<TValue, int>();
            foreach (var valueData in valueHistolist) {
                valueGram[valueData.Value] = valueData.Count;
            }
        }

        #region Public properties
        public Dictionary<TSuit, int> SuitGram { get { return suitGram; } }
        public Dictionary<TValue, int> ValueGram { get { return valueGram; } }
        #endregion // Public properties

        #region Public methods
        public HandCard[] CardsAll { get { return hcsPlain.Concat(hcsUnbound).ToArray(); } }
        public HandCard[] CardsPlain { get { return hcsPlain; } }
        public HandCard[] CardsUnbound { get { return hcsUnbound; } }
        public bool HasPlainCards() { return CardsPlain.Count() > 0; }
        public bool HasUnboundCards() { return CardsUnbound.Count() > 0; }
        public int PlainCount() { return CardsPlain.Count(); }
        public int UnboundCount() { return CardsUnbound.Count(); }
        public Card[] WildCards() { return wildCards.ToArray(); }
        public bool IsWild(Card c)
        {
            return wildCards.Any(wc => wc.Value == c.Value && wc.Suit == c.Suit);
        }
        #endregion // Public methods

        #region Private
        // Note: ListDictionary is an (untyped) alternative to Dictionary<T,U> for small dictionaries
        private HandCard[] hcsPlain;
        private HandCard[] hcsUnbound;
#pragma warning disable 0649
        private Card[] wildCards;
#pragma warning restore 0649
        private Dictionary<TSuit, int> suitGram;    
        private Dictionary<TValue, int> valueGram;
        #endregion // Private
    }
}