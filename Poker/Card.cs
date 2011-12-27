// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

namespace Poker
{
    #region Types
    public enum TSuit : byte
    {
        [Description("-")]
        NoSuit,
        [Description("C")]
        Clubs,       // Unicode symbol = \x2663
        [Description("D")]
        Diamonds,    // Unicode symbol = \x2662
        [Description("H")]
        Hearts,      // Unicode symbol = \x2661
        [Description("S")]
        Spades,      // Unicode symbol = \x2660
        [Description("*")]
        WildSuit
    }

    public enum TValue : byte
    {
        [Description("-")]
        NoValue,
        [Description("a")]
        LowAce,
        [Description("2")]
        Two,
        [Description("3")]
        Three,
        [Description("4")]
        Four,
        [Description("5")]
        Five,
        [Description("6")]
        Six,
        [Description("7")]
        Seven,
        [Description("8")]
        Eight,
        [Description("9")]
        Nine,
        [Description("10")]
        Ten,
        [Description("J")]
        Jack,
        [Description("Q")]
        Queen,
        [Description("K")]
        King,
        [Description("A")]
        Ace,
        [Description("*")]
        WildValue
    }

    public interface ICard
    {
        void Demote();
        Card DemotedCopy();
        bool IsJoker();
        bool IsSuitWild();
        bool IsValueWild();
        TSuit Suit { get; }
        string ToString();
        TValue Value { get; }
    }
    #endregion // Types

    public class Card : ICard, IComparable<Card>
    {
        #region Lifecycle
        static Card()
        {
            Clubs = new List<Card>();
            Diamonds = new List<Card>();
            Hearts = new List<Card>();
            Spades = new List<Card>();
            foreach (TValue val in Values) {
                Clubs.Add(new Card(val, TSuit.Clubs));
                Diamonds.Add(new Card(val, TSuit.Diamonds));
                Hearts.Add(new Card(val, TSuit.Hearts));
                Spades.Add(new Card(val, TSuit.Spades));
            }
        }

        public Card(TValue value, TSuit suit)
        {
            this.Value = value;
            this.Suit = suit;
        }
        #endregion // Lifecycle

        #region Public fields/properties/methods
        public virtual int CompareTo(Card c)
        {
            TValue otherValue = c.Value;
            return Value.CompareTo(otherValue);
        }

        public void Demote()
        {
            if (Value == TValue.Ace) { Value = TValue.LowAce; }
        }

        public Card DemotedCopy()
        {
            Card c = new Card(Value == TValue.Ace ? TValue.LowAce : Value, Suit);
            return c;
        }

        public bool IsJoker() { return IsValueWild() && IsSuitWild(); }

        public bool IsSuitWild() { return Suit == TSuit.WildSuit; }

        public bool IsValueWild() { return Value == TValue.WildValue; }

        public TSuit Suit { get; protected set; }

        public override string ToString()
        {
            return Value.ToText() + Suit.ToText();
        }

        public TValue Value { get; protected set; }
        #endregion // Public fields/properties/methods

        #region Constant suits
        public static readonly TSuit FirstSuit = TSuit.Clubs;
        public static readonly TSuit LastSuit = TSuit.Spades;
        #endregion // Constant suits

        #region Constant cards
        #region Clubs
        public readonly static Card C1 = new Card(TValue.LowAce, TSuit.Clubs);
        public readonly static Card C2 = new Card(TValue.Two, TSuit.Clubs);
        public readonly static Card C3 = new Card(TValue.Three, TSuit.Clubs);
        public readonly static Card C4 = new Card(TValue.Four, TSuit.Clubs);
        public readonly static Card C5 = new Card(TValue.Five, TSuit.Clubs);
        public readonly static Card C6 = new Card(TValue.Six, TSuit.Clubs);
        public readonly static Card C7 = new Card(TValue.Seven, TSuit.Clubs);
        public readonly static Card C8 = new Card(TValue.Eight, TSuit.Clubs);
        public readonly static Card C9 = new Card(TValue.Nine, TSuit.Clubs);
        public readonly static Card C10 = new Card(TValue.Ten, TSuit.Clubs);
        public readonly static Card C11 = new Card(TValue.Jack, TSuit.Clubs);
        public readonly static Card C12 = new Card(TValue.Queen, TSuit.Clubs);
        public readonly static Card C13 = new Card(TValue.King, TSuit.Clubs);
        public readonly static Card C14 = new Card(TValue.Ace, TSuit.Clubs);
        #endregion // Clubs
        #region Diamonds
        public readonly static Card D1 = new Card(TValue.LowAce, TSuit.Diamonds);
        public readonly static Card D2 = new Card(TValue.Two, TSuit.Diamonds);
        public readonly static Card D3 = new Card(TValue.Three, TSuit.Diamonds);
        public readonly static Card D4 = new Card(TValue.Four, TSuit.Diamonds);
        public readonly static Card D5 = new Card(TValue.Five, TSuit.Diamonds);
        public readonly static Card D6 = new Card(TValue.Six, TSuit.Diamonds);
        public readonly static Card D7 = new Card(TValue.Seven, TSuit.Diamonds);
        public readonly static Card D8 = new Card(TValue.Eight, TSuit.Diamonds);
        public readonly static Card D9 = new Card(TValue.Nine, TSuit.Diamonds);
        public readonly static Card D10 = new Card(TValue.Ten, TSuit.Diamonds);
        public readonly static Card D11 = new Card(TValue.Jack, TSuit.Diamonds);
        public readonly static Card D12 = new Card(TValue.Queen, TSuit.Diamonds);
        public readonly static Card D13 = new Card(TValue.King, TSuit.Diamonds);
        public readonly static Card D14 = new Card(TValue.Ace, TSuit.Diamonds);
        #endregion // Diamonds
        #region Hearts
        public readonly static Card H1 = new Card(TValue.LowAce, TSuit.Hearts);
        public readonly static Card H2 = new Card(TValue.Two, TSuit.Hearts);
        public readonly static Card H3 = new Card(TValue.Three, TSuit.Hearts);
        public readonly static Card H4 = new Card(TValue.Four, TSuit.Hearts);
        public readonly static Card H5 = new Card(TValue.Five, TSuit.Hearts);
        public readonly static Card H6 = new Card(TValue.Six, TSuit.Hearts);
        public readonly static Card H7 = new Card(TValue.Seven, TSuit.Hearts);
        public readonly static Card H8 = new Card(TValue.Eight, TSuit.Hearts);
        public readonly static Card H9 = new Card(TValue.Nine, TSuit.Hearts);
        public readonly static Card H10 = new Card(TValue.Ten, TSuit.Hearts);
        public readonly static Card H11 = new Card(TValue.Jack, TSuit.Hearts);
        public readonly static Card H12 = new Card(TValue.Queen, TSuit.Hearts);
        public readonly static Card H13 = new Card(TValue.King, TSuit.Hearts);
        public readonly static Card H14 = new Card(TValue.Ace, TSuit.Hearts);
        #endregion // Hearts
        #region Spades
        public readonly static Card S1 = new Card(TValue.LowAce, TSuit.Spades);
        public readonly static Card S2 = new Card(TValue.Two, TSuit.Spades);
        public readonly static Card S3 = new Card(TValue.Three, TSuit.Spades);
        public readonly static Card S4 = new Card(TValue.Four, TSuit.Spades);
        public readonly static Card S5 = new Card(TValue.Five, TSuit.Spades);
        public readonly static Card S6 = new Card(TValue.Six, TSuit.Spades);
        public readonly static Card S7 = new Card(TValue.Seven, TSuit.Spades);
        public readonly static Card S8 = new Card(TValue.Eight, TSuit.Spades);
        public readonly static Card S9 = new Card(TValue.Nine, TSuit.Spades);
        public readonly static Card S10 = new Card(TValue.Ten, TSuit.Spades);
        public readonly static Card S11 = new Card(TValue.Jack, TSuit.Spades);
        public readonly static Card S12 = new Card(TValue.Queen, TSuit.Spades);
        public readonly static Card S13 = new Card(TValue.King, TSuit.Spades);
        public readonly static Card S14 = new Card(TValue.Ace, TSuit.Spades);
        #endregion // Spades
        #endregion // Constant cards

        #region Constant lists
        public readonly static TSuit[] Suits = new TSuit[] {
            TSuit.Clubs,
            TSuit.Diamonds,
            TSuit.Hearts,
            TSuit.Spades
        };

        public readonly static TValue[] Values = {
            TValue.Two, TValue.Three, TValue.Four, TValue.Five,
            TValue.Six, TValue.Seven, TValue.Eight, TValue.Nine, TValue.Ten,
            TValue.Jack, TValue.Queen, TValue.King, TValue.Ace
        };

        public readonly static List<Card> Clubs;
        public readonly static List<Card> Diamonds;
        public readonly static List<Card> Hearts;
        public readonly static List<Card> Spades;
        #endregion // Constant lists
    }
}