// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker
{
    public class TestRankProfile
    {
        public static void WriteRankProfile(Hand hand, IEnumerable<Card> wildCards, Ranker ranker)
        {
            HandInfo hi = new HandInfo(hand, wildCards);
            foreach (SimpleRank sRank in Ranker.StandardRanks)
            {
                FullRank fRank;
                HandCard[] hcsRank;
                HandCard[] hcsFill;
                if (ranker.RankPredicateMap[sRank](hi, out fRank, out hcsRank, out hcsFill))
                {
                    Console.WriteLine("Examining hand as a hand of rank {0:s}", fRank.SRank.ToString());
                    Console.Write("  ");
                    Hand.WriteHandData(hand, wildCards, fRank, hcsRank, hcsFill);
                }
            }
        }
    }
}