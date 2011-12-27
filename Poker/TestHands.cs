// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker
{
    public class TestHands
    {
        public static Hand h5_Kind5         = new Hand(Card.D5, Card.D5, Card.H5, Card.S5,  Card.S5);
        public static Hand h5_StraightFlush = new Hand(Card.S4, Card.S5, Card.S6, Card.S7,  Card.S8);
        public static Hand h5_Kind4         = new Hand(Card.C4, Card.D4, Card.H4, Card.S4,  Card.S10);
        public static Hand h5_FullHouse     = new Hand(Card.C3, Card.D3, Card.H3, Card.S7,  Card.H7);
        public static Hand h5_Flush         = new Hand(Card.S2, Card.S4, Card.S6, Card.S8,  Card.S10);
        public static Hand h5_Straight      = new Hand(Card.C4, Card.D5, Card.H7, Card.S6,  Card.S8);
        public static Hand h5_Kind3         = new Hand(Card.C3, Card.D3, Card.H3, Card.S5,  Card.S9);
        public static Hand h5_TwoPair       = new Hand(Card.C7, Card.D7, Card.H9, Card.S9,  Card.S13);
        public static Hand h5_Kind2         = new Hand(Card.C9, Card.D9, Card.H5, Card.S14, Card.S3);
        public static Hand h5_HighCard      = new Hand(Card.C6, Card.D2, Card.H8, Card.S4,  Card.S14);

        public static Hand h5_NearStraightFlush = new Hand(Card.S9, Card.C4, Card.C5, Card.C7, Card.C6);
        public static Hand h7_FullHouse     = new Hand(Card.C3, Card.D3, Card.H3, Card.S5, Card.S9, Card.H4, Card.D4);

        public static Hand h10_Flush_A      = new Hand(new Card[] { Card.C2, Card.C3, Card.C4, Card.C8, Card.C7,
                                                             Card.D2, Card.D3, Card.D5, Card.D8, Card.D7 });
        public static Hand h10_Flush_B      = new Hand(new Card[] { Card.C2, Card.C3, Card.C6, Card.C8, Card.C7,
                                                             Card.D2, Card.D3, Card.D5, Card.D8, Card.D7 });

        public static Hand[] Hands = new Hand[] {
            h5_Kind5,
            h5_StraightFlush,
            h5_Kind4,
            h5_FullHouse,
            h5_Flush,
            h5_Straight,
            h5_Kind3,
            h5_TwoPair,
            h5_Kind2,
            h5_HighCard,

            h5_NearStraightFlush,
            h7_FullHouse,
            h10_Flush_A,
            h10_Flush_B
        };
    }
}