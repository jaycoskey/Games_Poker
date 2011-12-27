// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Poker
{
    public class TestHandGenerator
    {
        #region Types
        public delegate Hand RandomHandDelegate(int numCardsDealt, int numCardsInFinalHand);
        #endregion // Types

        #region Lifecycle
        public TestHandGenerator(int seed)
        {
            random = new Random(seed);
            Generators = new Dictionary<SimpleRank, RandomHandDelegate>()
                {
                    {   SimpleRank.HighCard,        GetRandomHighCard       },
                    {   SimpleRank.Kind2,           GetRandomKind2          },
                    // Straight4,
                    // Flush4,
                    {   SimpleRank.TwoPair,         GetRandomTwoPair        },
                    // StraightFlush3,
                    {   SimpleRank.Kind3,           GetRandomKind3          },
                    {   SimpleRank.Straight5,       GetRandomStraight5       },
                    {   SimpleRank.Flush5,          GetRandomFlush5         },
                    {   SimpleRank.FullHouse,       GetRandomFullHouse      },
                    // StraightFlush4, 
                    {   SimpleRank.Kind4,           GetRandomKind4          },
                    {   SimpleRank.StraightFlush5,  GetRandomStraightFlush5  },
                    {   SimpleRank.Kind5,           GetRandomKind5          }
                };
        }
        #endregion // Lifecycle

        #region Public data
        public readonly Dictionary<SimpleRank, RandomHandDelegate> Generators;
        #endregion // Public data

        #region Public static methods
        /// <summary>
        ///     Generate random hands, and write out the details of those under-ranked
        ///     by the rank predicate methods.  This would happen if one or more of the
        ///     rank predicate methods returned false negatives.
        /// </summary>
        public static void WriteErrantRandomHands(
            int numSamplesPerSimpleRank = 100,
            int numCardsDealt = 5,
            int numCardsInFinalHand = 5)
        {
            Ranker ranker = new Ranker(numCardsInFinalHand, false);
            TestHandGenerator handGen = new TestHandGenerator(0);
            foreach (SimpleRank sRank in Ranker.StandardRanks)
            {
                // Console.WriteLine();
                Console.Write("Press enter to see errant hands of type {0:s}", sRank.ToString());
                Console.Out.Flush();
                Console.ReadLine();
                for (int i = 0; i < 1000; i++)
                {
                    Hand h = handGen.Generators[sRank](numCardsDealt, numCardsInFinalHand);
                    FullRank fRank;
                    HandCard[] hcsRank;
                    HandCard[] hcsFill;
                    ranker.GetFullRank(h, null, out fRank, out hcsRank, out hcsFill);
                    if (fRank.SRank != sRank)
                    {
                        Console.WriteLine("Attempt to create {0:s} yielded {1:s}",
                            sRank.ToString(), fRank.SRank.ToString());
                        Console.Write("  ");
                        Hand.WriteHandData(h, null, fRank, hcsRank, hcsFill);
                    }
                }
            }
        }
        #endregion // Public static methods

        #region Public random card/suit generation methods
        public Card GetRandomCard() {
            return new Card(
                (TValue) random.Next((int) TValue.Two, (int) TValue.Ace),
                (TSuit) random.Next(1, 4)
                );
        }

        public Card GetRandomCardWithSuit(TSuit s)
        {
            return new Card((TValue)random.Next((int)TValue.Two, (int)TValue.Ace), s);
        }

        public Card GetRandomCardWithValue(TValue v)
        {
            return new Card(v, (TSuit)random.Next(1, 4));
        }

        public Card[] GetRandomCards(int n)
        {
            return Enumerable.Range(1, n).Select(k => GetRandomCard()).ToArray();
        }

        public Card[] GetRandomCardsWithSuit(TSuit s, int n)
        {
            return Enumerable.Range(1, n).Select(k => GetRandomCardWithSuit(s)).ToArray();
        }

        public Card[] GetRandomCardsWithValue(TValue v, int n)
        {
            return Enumerable.Range(1, n).Select(k => GetRandomCardWithValue(v)).ToArray();
        }

        public TSuit GetRandomSuit()
        {
            return (TSuit)random.Next((int)Card.FirstSuit, (int)Card.LastSuit);
        }
        #endregion // Public random card/suit generation methods

        #region Public random hand generation methods
        public Hand GetRandomFlush5(int numCardsDealt, int numCardsInFinalHand)
        {
            Ranker ranker = new Ranker(numCardsInFinalHand, false);
            TSuit flushSuit = GetRandomSuit();
            Hand hand;
            FullRank fRank;
            HandCard[] hcsRank;
            HandCard[] hcsFill;
            do
            {
                Card[] chosen = GetRandomCardsWithSuit(flushSuit, 5);
                Card[] fill = GetRandomCards(numCardsDealt - 5).ToArray();
                fRank = null;
                hcsRank = null;
                hcsFill = null;
                hand = new Hand(fill.Concat(chosen));
                ranker.GetFullRank(hand, null, out fRank, out hcsRank, out hcsFill);
            } while (fRank.SRank > SimpleRank.Flush5);
            return hand;
        }

        public Hand GetRandomFullHouse(int numCardsDealt, int numCardsInFinalHand)
        {
            Ranker ranker = new Ranker(numCardsInFinalHand, false);
            Hand hand;
            FullRank fRank;
            HandCard[] hcsRank;
            HandCard[] hcsFill;
            do
            {
                TValue trioValue = (TValue)random.Next(2, 14);
                TValue pairValue = (TValue)random.Next(2, 13);
                if (pairValue >= trioValue) { pairValue = pairValue.Next(); }
                Card[] trioCards = GetRandomCardsWithValue(trioValue, 3);
                Card[] pairCards = GetRandomCardsWithValue(pairValue, 2);
                Card[] fill = GetRandomCards(numCardsDealt - 5).ToArray();
                hand = new Hand(fill.Concat(pairCards).Concat(trioCards));
                ranker.GetFullRank(hand, null, out fRank, out hcsRank, out hcsFill);
            } while (fRank.SRank > SimpleRank.FullHouse);
            return hand;
        }

        public Hand GetRandomHighCard(int numCardsDealt, int numCardsInFinalHand)
        {
            Hand hand;
            FullRank fRank;
            HandCard[] hcsRank;
            HandCard[] hcsFill;
            Ranker ranker = new Ranker(numCardsInFinalHand, false);
            do
            {
                hand = new Hand(GetRandomCards(numCardsDealt));
                hcsRank = null;
                hcsFill = null;
                ranker.GetFullRank(hand, null, out fRank, out hcsRank, out hcsFill);
            } while (fRank.SRank > SimpleRank.HighCard);
            return hand;
        }

        public Hand GetRandomKind2(int numCardsDealt, int numCardsInFinalHand)
        {
            return GetRandomKindN(numCardsDealt, numCardsInFinalHand, 2);
        }

        public Hand GetRandomKind3(int numCardsDealt, int numCardsInFinalHand)
        {
            return GetRandomKindN(numCardsDealt, numCardsInFinalHand, 3);
        }

        public Hand GetRandomKind4(int numCardsDealt, int numCardsInFinalHand)
        {
            return GetRandomKindN(numCardsDealt, numCardsInFinalHand, 4);
        }

        public Hand GetRandomKind5(int numCardsDealt, int numCardsInFinalHand)
        {
            return GetRandomKindN(numCardsDealt, numCardsInFinalHand, 5);
        }

        public Hand GetRandomKindN(int numCardsDealt, int numCardsInFinalHand, int n)
        {
            Contract.Requires(n >= 2 && n <= 5, "Error: size of n-of-a-kind out of range");
            Ranker ranker = new Ranker(numCardsInFinalHand, false);
            Hand hand;
            FullRank fRank;
            HandCard[] hcsRank;
            HandCard[] hcsFill;
            SimpleRank[] availRanks = new SimpleRank[] {
                SimpleRank.Kind2, SimpleRank.Kind3, SimpleRank.Kind4, SimpleRank.Kind5
            };
            SimpleRank thisRank = availRanks[n - 2];

            do
            {
                TValue value = (TValue)random.Next(2, 14);
                Card[] tuple = GetRandomCardsWithValue(value, n);
                Card[] fill = GetRandomCards(numCardsDealt - n);
                hand = new Hand(tuple.Concat(fill));
                ranker.GetFullRank(hand, null, out fRank, out hcsRank, out hcsFill);
            } while (fRank.SRank > thisRank);
            return hand;
        }

        public Hand GetRandomStraight5(int numCardsDealt, int numCardsInFinalHand)
        {
            return GetRandomStraightOrStraightFlush5(numCardsDealt, numCardsInFinalHand, false);
        }

        public Hand GetRandomStraightFlush5(int numCardsDealt, int numCardsInFinalHand)
        {
            return GetRandomStraightOrStraightFlush5(numCardsDealt, numCardsInFinalHand, true);
        }

        public Hand GetRandomStraightOrStraightFlush5(int numCardsDealt, int numCardsInFinalHand, bool doRequireFlush)
        {
            Ranker ranker = new Ranker(numCardsInFinalHand, false);
            Hand hand;
            FullRank fRank;
            TSuit flushSuit = GetRandomSuit();
            HandCard[] hcsRank;
            HandCard[] hcsFill;
            do
            {
                TValue maxValue = (TValue)random.Next((int)TValue.Six, (int)TValue.Ace);
                TValue minValue = maxValue.Prev(4);
                Card[] chosen = Enumerable.Range((int)minValue, ((int)maxValue - (int)minValue) + 1)
                    .Select(k => doRequireFlush
                            ? (new Card((TValue)k, flushSuit))
                            : GetRandomCardWithValue((TValue)k))
                    .Reverse().ToArray();
                Card[] fill = GetRandomCards(numCardsDealt - 5).ToArray();
                hand = new Hand(fill.Concat(chosen));
                ranker.GetFullRank(hand, null, out fRank, out hcsRank, out hcsFill);
            } while (fRank.SRank > (doRequireFlush ? SimpleRank.StraightFlush5 : SimpleRank.Straight5));
            return hand;
        }

        public Hand GetRandomTwoPair(int numCardsDealt, int numCardsInFinalHand)
        {
            Ranker ranker = new Ranker(numCardsInFinalHand, false);
            Hand hand;
            FullRank fRank;
            HandCard[] hcsRank;
            HandCard[] hcsFill;
            do
            {
                TValue pair1Value = (TValue)random.Next(2, 14);
                TValue pair2Value = (TValue)random.Next(2, 13);
                if (pair2Value >= pair1Value) { pair2Value = pair2Value.Next(); }
                Card[] pair1Cards = GetRandomCardsWithValue(pair1Value, 2);
                Card[] pair2Cards = GetRandomCardsWithValue(pair2Value, 2);
                Card[] fill = GetRandomCards(numCardsDealt - 4).ToArray();
                hand = new Hand(fill.Concat(pair1Cards).Concat(pair2Cards));
                ranker.GetFullRank(hand, null, out fRank, out hcsRank, out hcsFill);
            } while (fRank.SRank > SimpleRank.TwoPair);
            return hand;
        }
        #endregion // Public random hand generation methods

        #region Private data
        private Random random;
        #endregion // Private data
    }
}