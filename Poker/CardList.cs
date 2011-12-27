// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker
{
    public abstract class CardList<CardType> where CardType : ICard
    {
        #region Public properties / methods
        public virtual IEnumerable<CardType> Cards
        {
            get
            {
                var result = new List<CardType>(handCards);
                return result;
            }
        }

        public virtual IEnumerable<TSuit> Suits
        {
            get
            {
                var result = handCards.Select(hc => hc.Suit);
                return result;
            }
        }

        public virtual IEnumerable<TValue> Values
        {
            get
            {
                var result = handCards.Select(hc => hc.Value);
                return result;
            }
        }
        #endregion // Public properties / methods

        #region Public static values
        public static readonly IEnumerable<Card> PinochleDeck =
            from int n in Enumerable.Range(0, 2)
            from TSuit s in Card.Suits
            from TValue v in new TValue[] {
                TValue.Nine, TValue.Ten, TValue.Jack,
                TValue.Queen, TValue.King, TValue.Ace
            }
            select new Card(v, s);

        public static readonly IEnumerable<Card> StandardDeck =
            from TSuit s in Card.Suits
            from TValue v in Card.Values
            select new Card(v, s);
        #endregion // Static members

        #region Protected members
        protected List<CardType> handCards;
        #endregion // Protected members
    }
}