// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker
{
    class Program
    {
        static void Main(string[] args)
        {
            // List<Card> wilds = null;
            // List<Card> wilds = new List<Card>() { Card.S5, Card.S6, Card.S7, Card.S8 };
            List<Card> wilds = new List<Card>() { Card.S9 };
            
            Ranker ranker = new Ranker(5, false);

            WriteTestHandRanks(ranker, wilds);
            WriteTestHandProfiles(ranker, wilds);
            WriteErrantRandomHands(ranker, 100);
            WriteRandomHandProfiles(ranker, 7, 5, wilds, 10);

            Console.WriteLine("Press enter to exit program.");
            Console.Out.Flush();
            Console.ReadLine();
        }

        public static void WriteRandomHandProfiles(
            Ranker ranker,
            int numCardsDealt = 7,
            int numCardsInFinalHand = 5,
            IEnumerable<Card> wilds = null,
            int numSamplesPerSimpleRank = 10)
        {
            Console.WriteLine();
            Console.WriteLine("Press enter to write out full evaluation of random hands.");
            Console.Out.Flush();
            Console.ReadLine();

            TestHandGenerator handGen = new TestHandGenerator(0);
            foreach (SimpleRank sRank in Ranker.StandardRanks)
            {
                Console.WriteLine();
                Console.Write("Press enter to see full evaluation of hands of type {0:s}", sRank.ToString());
                Console.Out.Flush();
                Console.ReadLine();
                for (int i = 0; i < numSamplesPerSimpleRank; i++)
                {
                    Hand h = handGen.Generators[sRank](numCardsDealt, numCardsInFinalHand);
                    HandInfo hi = new HandInfo(h, wilds);
                    foreach (SimpleRank sRank2 in Ranker.StandardRanks)
                    {
                        FullRank fRank;
                        HandCard[] hcsRank;
                        HandCard[] hcsFill;
                        if (ranker.RankPredicateMap[sRank2](hi, out fRank, out hcsRank, out hcsFill))
                        {
                            Console.WriteLine("Examining hand as a hand of rank {0:s}", fRank.SRank.ToString());
                            Console.Write("  ");
                            Hand.WriteHandData(h, wilds, fRank, hcsRank, hcsFill);
                        }
                    }
                }
            }
        }

        public static void WriteTestHandRanks(Ranker ranker, IEnumerable<Card> wilds) {
            Console.WriteLine();
            Console.WriteLine("Press enter to write out evaluation of each test hand for each rank.");
            Console.ReadLine();
            foreach (Hand h in TestHands.Hands)
            {
                FullRank fRank;
                HandCard[] hcsRank;
                HandCard[] hcsFill;
                ranker.GetFullRank(h, wilds, out fRank, out hcsRank, out hcsFill);
                Hand.WriteHandData(h, wilds, fRank, hcsRank, hcsFill);
            }
        }

        public static void WriteTestHandProfiles(Ranker ranker, IEnumerable<Card> wilds)
        {
            Console.WriteLine();
            Console.WriteLine("Press enter to write out full evaluation of random hands.");
            Console.Out.Flush();
            Console.ReadLine();
            foreach (Hand h in TestHands.Hands)
            {
                TestRankProfile.WriteRankProfile(h, wilds, ranker);
            }
        }

        public static void WriteErrantRandomHands(Ranker ranker, int numSamplesPerSimpleRank)
        {
            Console.WriteLine();
            Console.WriteLine("Press enter to display hands that might uncover rank predicate errors.");
            Console.Out.Flush();
            Console.ReadLine();
            TestHandGenerator.WriteErrantRandomHands(numSamplesPerSimpleRank, 7, 5);
        }
    }
}