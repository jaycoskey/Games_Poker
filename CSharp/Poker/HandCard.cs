// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker
{
    public class HandCard : Card, IComparable<HandCard>
    {
        #region Lifecycle
        public HandCard(Card c, int index)
            : base(c.Value, c.Suit)
        {
            this.Index = index;
            this.TamedValue = TValue.NoValue;
            this.TamedSuit = TSuit.NoSuit;
        }

        public HandCard(TValue value, TSuit suit, int index)
            : base(value, suit)
        {
            this.Index = index;
            this.TamedValue = TValue.NoValue;
            this.TamedSuit = TSuit.NoSuit;
        }
        #endregion // Lifecycle

        #region Public properties / methods
        public virtual int CompareTo(HandCard c)
        {
            TValue otherValue = c.Value;
            return Value.CompareTo(otherValue);
        }

        public int Index { get; set; }

        public TValue TamedValue { get; set; }

        public TSuit TamedSuit { get; set; }

        public string ToLongString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            if (TamedValue != TValue.NoValue || TamedSuit != TSuit.NoSuit)
            {
                sb.Append(String.Format(" (as {0:s}{1:s})",
                    TamedValue == TValue.NoValue ? "*" : TamedValue.ToText(),
                    TamedSuit == TSuit.NoSuit ? "*" : TamedSuit.ToText()));
            }
            return sb.ToString();
        }
        #endregion // Public properties / methods
    }
}