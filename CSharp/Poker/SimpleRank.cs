// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

namespace Poker
{
    public enum SimpleRank
    {
        [Description("No rank")]
        NoRank,
        [Description("High card")]
        HighCard,
        [Description("Two of a kind")]
        Kind2,
        [Description("Bobtail straight")]
        Straight4,          // a.k.a. BobtailStraight = 4 cards having consecutive values
        // FlushHouse,      // 3 cards of one suit and two cards of another
        [Description("Bobtail flush")]
        Flush4,             // a.k.a. BobtailFlush = 4 cards of the same suit
        // Russ,            // 5 cards of the same color
        [Description("Two pair")]
        TwoPair,
        // Blaze,           // All cards are J, Q, or K
        // Flash,           // One card of each suit (using Joker to for 5th suit in 4-suit deck)
        [Description("Little bobtail")]
        StraightFlush3,     // a.k.a. LittleBobtail = 3 card straight flush
        [Description("Three of a kind")]
        Kind3,              // 3 of a kind
        // Skeet,           // Hand = 2, (3|4), 5, 6, (7|8), 9
        // FiveAndDime,     // No pair; all cards are in [5..10]
        // SkipStraight,    // Having consecutive values, skipping every other one.
        // WrapAroundStraight,  // For example, {Q, K, A, 2, 3}
        [Description("Straight")]
        Straight5,
        // LittleDog,       // Small straight with 7 high and 2 low
        // BigDog,          // Small straight with A high and 9 low
        // LittleCat,       // Small straight with 8 high and 3 low
        // BigCat,          // Small straight with K high and 8 low
        // StraightFlushHouse,  // FlushHouse having consecutive values
        [Description("Flush")]
        Flush5,             // Flush
        [Description("Full house")]
        FullHouse,
        [Description("Big bobtail")]
        StraightFlush4,     // a.k.a. BigBobtail = 4 card straight flush
        [Description("Four of a kind")]
        Kind4,              // 4 of a kind
        [Description("Straight flush")]
        StraightFlush5,
        // SkeetFlush,  // Hand = 2, (3|4), 5, 6, (7|8), 9, all of the same suit
        // RoyalFlush,  // StraightFlush with A high
        [Description("Five of a kind")]
        Kind5
    }
}