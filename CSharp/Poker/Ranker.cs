// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Poker
{
    public class Ranker
    {
        #region Types
        public delegate bool RankPredicate(
            HandInfo hi,
            out FullRank fRank,
            out HandCard[] hcsRank,
            out HandCard[] hcsFill);
        #endregion // Types

        #region Lifecycle
        public Ranker(int numCardsInFinalHand, bool doUseExtendedPredicates)
        {
            NumCardsInFinalHand = numCardsInFinalHand;
            RankPredicateMap = new Dictionary<SimpleRank, RankPredicate>()
                {
                    {   SimpleRank.HighCard,        isHighCard      },
                    {   SimpleRank.Kind2,           isKind2         },
                    {   SimpleRank.Straight4,       isStraight4     },
                    {   SimpleRank.Flush4,          isFlush4        },
                    {   SimpleRank.TwoPair,         isTwoPair       },
                    {   SimpleRank.StraightFlush3,  isStraightFlush3},
                    {   SimpleRank.Kind3,           isKind3         },
                    {   SimpleRank.Straight5,       isStraight5     },
                    {   SimpleRank.Flush5,          isFlush5        },
                    {   SimpleRank.FullHouse,       isFullHouse     },
                    {   SimpleRank.StraightFlush4,  isStraightFlush4},
                    {   SimpleRank.Kind4,           isKind4         },
                    {   SimpleRank.StraightFlush5,  isStraightFlush5},
                    {   SimpleRank.Kind5,           isKind5         }
                };
            StandardRankPredicates = getRankPredicates(true, false);
            ExtendedRankPredicates = getRankPredicates(true, false);
            AllRankPredicates = getRankPredicates(true, true); 
            rankPredicates = doUseExtendedPredicates ? AllRankPredicates : StandardRankPredicates;
        }
        #endregion // Lifecycle

        #region Public data
        public int NumCardsInFinalHand;
        public readonly Dictionary<SimpleRank, RankPredicate> RankPredicateMap;
        public readonly RankPredicate[] StandardRankPredicates;
        public readonly RankPredicate[] ExtendedRankPredicates;
        public readonly RankPredicate[] AllRankPredicates;
        private RankPredicate[] rankPredicates;
        internal static SimpleRank[] StandardRanks = {
            SimpleRank.HighCard,
            SimpleRank.Kind2,
            SimpleRank.TwoPair,
            SimpleRank.Kind3,
            SimpleRank.Straight5,
            SimpleRank.Flush5,
            SimpleRank.FullHouse,
            SimpleRank.Kind4,
            SimpleRank.StraightFlush5,
            SimpleRank.Kind5
        };
        internal static SimpleRank[] ExtendedRanks = {
            SimpleRank.Straight4,
            SimpleRank.Flush4,
            SimpleRank.StraightFlush3,
            SimpleRank.StraightFlush4,
        };
        #endregion // Public data

        #region Public methods
        public void GetFullRank(
            Hand h,
            IEnumerable<Card> wildCards,
            out FullRank fRank,
            out HandCard[] hcsRank,
            out HandCard[] hcsFill)
        {
            Contract.Requires(h.Cards.Count() >= NumCardsInFinalHand, "Error: Not enough cards in hand for ranking");
            foreach (HandCard hc in h.Cards)
            {
                hc.TamedValue = TValue.NoValue;
                hc.TamedSuit = TSuit.NoSuit;
            }
            HandInfo hi = new HandInfo(h, wildCards);
            foreach (Ranker.RankPredicate rankPredicate in rankPredicates)
            {
                if (rankPredicate(hi, out fRank, out hcsRank, out hcsFill))
                {
                    return;
                }
            }
            // If HighCard is one of the RankPredicates, we should never arrive here.
            fRank = new FullRank();
            hcsRank = null;
            hcsFill = null;
        }
        #endregion // Public methods

        #region Private hand rank predicate methods
        private bool isFlush4(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill)
        {
            return isFlushN(hi, out fRank, out hcsRank, out hcsFill, 4);
        }
        private bool isFlush5(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill)
        {
            return isFlushN(hi, out fRank, out hcsRank, out hcsFill, 5);
        }
        private bool isFlushN(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill, int n)
        {
            Contract.Requires(n >= 4 && n <= 5, String.Format("Error: Unrecognized flush size: {0:d}", n));
            bool hasFlushN = (!hi.HasPlainCards() && hi.UnboundCount() >= n)
                || (hi.SuitGram.Values.Max() >= n - hi.UnboundCount());
            if (!hasFlushN)
            {
                setToNull(out fRank, out hcsRank, out hcsFill);
                return false;
            }

            TSuit flushSuit;
            int numChosen = 0;
            IEnumerable<HandCard> hcsChosen;
            if (!hi.HasPlainCards())
            {
                flushSuit = Card.Suits.First();     // Any suit will do 
                numChosen = 0;
                hcsChosen = new HandCard[0];
            }
            else
            {
                var sg = hi.SuitGram;
                // Note: suits can only have Length > 1 if the hand has >= 2 * numCardsInFullHand cards.
                TSuit[] candidateSuits = sg.Keys.Where(s => sg[s] >= n - hi.UnboundCount()).ToArray();
                if (candidateSuits.Length == 1)
                {
                    flushSuit = candidateSuits[0];
                }
                else
                {   // Determine which suit is best
                    var cardSeqEnum = candidateSuits
                        .Select(s => hi.CardsPlain
                            .Where(hc => hc.Suit == s)
                            .OrderByDescending(hc => hc.Value));
                    var cardSeqOrd = cardSeqEnum
                        .OrderByDescending(hcs => (IEnumerable<HandCard>) hcs,
                            new EnumerableComparer<HandCard>());
                    var cardSeq = cardSeqOrd.First();
                    flushSuit = cardSeq.First().Suit;
                }
                HandCard[] hcsAvail = hi.CardsPlain.Where(hc => hc.Suit == flushSuit).ToArray();
                numChosen = Math.Min(n, hcsAvail.Count());
                hcsChosen = hcsAvail.OrderByDescending(hc => hc.Value).Take(numChosen);
            }
            HandCard[] hcsTamed = hi.CardsUnbound.Take(n - numChosen).ToArray();
            foreach (HandCard hcs in hcsTamed)
            {
                hcs.TamedValue = TValue.Ace;
                hcs.TamedSuit = flushSuit;
            }

            List<HandCard> fillTamed;
            List<HandCard> fillPlain;
            Util.TakeFill(NumCardsInFinalHand - n,
                hi.CardsUnbound.Except(hcsTamed), out fillTamed,
                hi.CardsPlain.Except(hcsChosen), out fillPlain);
            foreach (HandCard hcs in fillTamed) { hcs.TamedValue = TValue.Ace; }
            ; SimpleRank sRank;
            switch (n)
            {
                case 4: sRank = SimpleRank.Flush4; break;
                case 5: sRank = SimpleRank.Flush5; break;
                default: throw new ApplicationException(String.Format(
                    String.Format("Error: Unrecognized flush size: {0:d}", n)));
            };
            fRank = new FullRank(sRank,
                hcsTamed.Select(hc => hc.TamedValue)
                    .Concat(hcsChosen.Select(hc => hc.Value).OrderByDescending(v => v))
                    .Concat(fillTamed.Select(hc => hc.TamedValue))
                    .Concat(fillPlain.Select(hc => hc.Value).OrderByDescending(v => v)));
            hcsRank = hcsChosen.Concat(hcsTamed).ToArray();
            hcsFill = fillTamed.Concat(fillPlain).ToArray();
            return true;
        }
        private bool isFullHouse(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill)
        {
            FullRank fRankTrio;
            HandCard[] hcsRankTrio;
            HandCard[] hcsFillTrio;
            if (!isKindN(hi, out fRankTrio, out hcsRankTrio, out hcsFillTrio, 3, 0))
            {
                setToNull(out fRank, out hcsRank, out hcsFill);
                return false;
            }
            TValue trioValue = fRankTrio.Values[0];

            FullRank fRankPair;
            HandCard[] hcsRankPair;
            HandInfo hi2 = new HandInfo(new Hand(hi.CardsAll.Except(hcsRankTrio)), hi.WildCards());
            if (!isKindN(hi2, out fRankPair, out hcsRankPair, out hcsFill, 2, NumCardsInFinalHand - 3 - 2))
            {
                setToNull(out fRank, out hcsRank, out hcsFill);
                return false;
            }
            TValue pairValue = fRankPair.Values[0];
            fRank = new FullRank(SimpleRank.FullHouse,
                Util.PairEnumerable(trioValue, pairValue).Concat(hcsFill.Select(hc => TValue.Ace)));
            hcsRank = hcsRankTrio.Concat(hcsRankPair).ToArray();
            return true;
        }
        private bool isHighCard(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill)
        {
            // Note: Any hand with 2+ cards and at least one wild card contains a pair.
            hcsRank = new HandCard[0];
            List<HandCard> fillTamed;
            List<HandCard> fillPlain;
            Util.TakeFill(NumCardsInFinalHand, hi.CardsUnbound, out fillTamed,
                hi.CardsPlain.OrderByDescending(hc => hc.Value), out fillPlain);
            foreach (HandCard hc in fillTamed) { hc.TamedValue = TValue.Ace; }
            hcsFill = fillPlain.Reverse<HandCard>().Concat(fillTamed).ToArray();
            TValue[] vals = fillTamed
                .Select(hc => hc.TamedValue)
                .Concat(fillPlain.Select(hc => hc.Value))
                .ToArray();
            fRank = new FullRank(SimpleRank.HighCard, vals);
            return true;
        }
        private bool isKind2(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill)
        {
            return isKindN(hi, out fRank, out hcsRank, out hcsFill, 2);
        }
        private bool isKind3(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill)
        {
            return isKindN(hi, out fRank, out hcsRank, out hcsFill, 3);
        }
        private bool isKind4(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill)
        {
            return isKindN(hi, out fRank, out hcsRank, out hcsFill, 4);
        }
        private bool isKind5(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill)
        {
            return isKindN(hi, out fRank, out hcsRank, out hcsFill, 5);
        }
        private bool isKindN(
            HandInfo hi,
            out FullRank fRank,
            out HandCard[] hcsRank,
            out HandCard[] hcsFill,
            int n,
            int maxFillCount = int.MaxValue)
        {
            Contract.Requires(n >= 2 && n <= 5, String.Format("Error: Unrecognized n-of-a-kind: {0:d}", n));
            bool hasKindN = (!hi.HasPlainCards() && hi.UnboundCount() >= n)
                || (hi.ValueGram.Values.Max() >= n - hi.UnboundCount());
            if (!hasKindN)
            {
                setToNull(out fRank, out hcsRank, out hcsFill);
                return false;
            }
            
            TValue kindVal;
            int numChosen;
            HandCard[] hcsChosen;
            var plainValues = hi.ValueGram.Keys;
            if (plainValues.Count() == 0)
            {
                kindVal = TValue.Ace;
                numChosen = 0;
                hcsChosen = new HandCard[0];
            }
            else
            {
                kindVal = plainValues.Where(val => hi.ValueGram[val] >= n - hi.UnboundCount()).Max();
                HandCard[] hcsAvail = hi.CardsPlain.Where(hc => hc.Value == kindVal).ToArray();
                numChosen = Math.Min(n, hcsAvail.Count());
                hcsChosen = hcsAvail.Take(numChosen).ToArray();
            }
            int numTamed = Math.Min(n - numChosen, hi.UnboundCount());
            HandCard[] hcsTamed = hi.CardsUnbound.Take(numTamed).ToArray();
            foreach (HandCard hc in hcsTamed) { hc.TamedValue = kindVal; }
            List<HandCard> fillTamed;
            List<HandCard> fillPlain;
            Util.TakeFill(Math.Min(NumCardsInFinalHand - n, maxFillCount),
                hi.CardsUnbound.Except(hcsTamed).OrderByDescending(hc => hc.Value),
                out fillTamed,
                hi.CardsPlain.Except(hcsChosen).OrderByDescending(hc => hc.Value),
                out fillPlain);
            foreach (HandCard hc in fillTamed) { hc.TamedValue = TValue.Ace; }
            SimpleRank sRank;
            switch (n)
            {
                case 5: sRank = SimpleRank.Kind5; break;
                case 4: sRank = SimpleRank.Kind4; break;
                case 3: sRank = SimpleRank.Kind3; break;
                case 2: sRank = SimpleRank.Kind2; break;
                default: throw new ArgumentException("n");
            }
            fRank = new FullRank(sRank,
                Util.SingletonEnumerable(kindVal)
                    .Concat(fillTamed.Select(hc => hc.TamedValue))
                    .Concat(fillPlain.Select(hc => hc.Value))
                );
            hcsRank = hcsChosen.Concat(hcsTamed).ToArray();
            hcsFill = fillTamed.Concat(fillPlain).ToArray();
            return true;
        }
        private bool isStraightFlush3(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill)
        {
            return isStraightOrStraightFlushN(hi, out fRank, out hcsRank, out hcsFill, true, 3);
        }
        private bool isStraightFlush4(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill)
        {
            return isStraightOrStraightFlushN(hi, out fRank, out hcsRank,  out hcsFill, true, 4);
        }
        private bool isStraightFlush5(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill)
        {
            return isStraightOrStraightFlushN(hi, out fRank, out hcsRank, out hcsFill, true, 5);
        }
        private bool isStraightFlushN(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill, int n)
        {
            return isStraightOrStraightFlushN(hi, out fRank, out hcsRank, out hcsFill, true, n);
        }
        private bool isStraight4(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill)
        {
            return isStraightOrStraightFlushN(hi, out fRank, out hcsRank, out hcsFill, false, 4);
        }
        private bool isStraight5(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill)
        {
            return isStraightOrStraightFlushN(hi, out fRank, out hcsRank, out hcsFill, false, 5);
        }
        private bool isStraightN(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill, int n)
        {
            return isStraightOrStraightFlushN(hi, out fRank, out hcsRank, out hcsFill, false, n);
        }
        private bool isStraightOrStraightFlushN(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill, bool isFlushRequired, int n)
        {
            bool isStraight = false;
            bool isStraightFlush = false;
            TValue minVal = TValue.NoValue;     // Suppress compiler warning
            TValue maxVal;
            IEnumerable<IGrouping<TValue, HandCard>> valueGroups = null;
            TSuit flushSuit = TSuit.NoSuit;
            // Note: The loop could be started at Max(hcsPlain.Value) + hi.UnboundCount()
            for (maxVal = TValue.Ace;
                maxVal >= (TValue)n; // TValue.Five
                maxVal = maxVal.Prev())
            {
                minVal = maxVal.Prev(n - 1);
                valueGroups = hi.CardsPlain
                    .Where(hc => (hc.Value >= minVal && hc.Value <= maxVal) || (minVal == TValue.LowAce && hc.Value == TValue.Ace))
                    .GroupBy(hc => hc.Value);
                isStraight = valueGroups.Count() >= n - hi.UnboundCount();
                if (isStraight && isFlushRequired)
                {
                    foreach (TSuit s in Card.Suits)
                    {
                        if (valueGroups.All(grp => grp.Any(hc => hc.Suit == s)))
                        {
                            flushSuit = s;
                            isStraightFlush = true;
                            break;
                        }
                    }
                }
                if (isStraight && (isStraightFlush || !isFlushRequired)) { break; }
            }
            if (!isStraight || (isFlushRequired && !isStraightFlush))
            {
                setToNull(out fRank, out hcsRank, out hcsFill);
                return false;
            }
            Func<HandCard, bool> pred = null;
            if (isFlushRequired)
            {
                pred = hc => hc.Suit == flushSuit;
            }
            IEnumerable<HandCard> hcsAvail = valueGroups.EnumerableChoice(pred);
            hcsRank = new HandCard[n];
            hcsFill = new HandCard[NumCardsInFinalHand - n];
            int index = 0;
            List<HandCard> hcsUnbound = new List<HandCard>(hi.CardsUnbound);    // Need random access
            for (TValue val = minVal; val <= maxVal; val = val.Next())
            {
                HandCard nextCard = hcsAvail.Where(hc => hc.Value == val).FirstOrDefault();
                if (nextCard != null)
                {
                    hcsRank[index] = nextCard;
                }
                else
                {
                    Debug.Assert(hcsUnbound.Count > 0, "Error: Not enough cards to form straight");
                    IEnumerable<HandCard> hcDesired = hcsUnbound.Where(hc => hc.Value == val).Take(1);
                    if (hcDesired.Count() > 0)
                    {
                        hcsRank[index] = hcDesired.ElementAt(0);
                        hcsUnbound.Remove(hcsRank[index]);
                    }
                    else
                    {
                        hcsRank[index] = hcsUnbound[0];
                        hcsUnbound.RemoveAt(0);
                    }
                    hcsRank[index].TamedValue = val;
                    if (isFlushRequired) { hcsRank[index].TamedSuit = flushSuit; }
                }
                index++;
            }
            fRank = new FullRank(
                    isFlushRequired
                    ? SimpleRank.StraightFlush5
                    : SimpleRank.Straight5,
                Enumerable.Range((int)minVal, ((int)maxVal - (int)minVal) + 1)
                    .OrderByDescending(x => x)
                    .Select(x => (TValue) x)
                    .ToArray()
                );
            return true;
        }
        private bool isTwoPair(HandInfo hi, out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill)
        {
            // Note: In the presence of wild cards, any hand with two pair
            //       would also have a higher rank (e.g., 3 of a kind).
            FullRank fRank1;
            HandCard[] hcsRank1;
            HandCard[] hcsFill1;
            if (!isKindN(hi, out fRank1, out hcsRank1, out hcsFill1, 2, 0))
            {
                setToNull(out fRank, out hcsRank, out hcsFill);
                return false;
            }
            TValue value1 = fRank1.Values[0];

            FullRank fRank2;
            HandCard[] hcsRank2;
            HandInfo hi2 = new HandInfo(new Hand(hi.CardsAll.Except(hcsRank1)), hi.WildCards());
            if (!isKindN(hi2, out fRank2, out hcsRank2, out hcsFill, 2, NumCardsInFinalHand - 4))
            {
                setToNull(out fRank, out hcsRank, out hcsFill);
                return false;
            }
            TValue value2 = fRank2.Values[0];
            TValue[] fillValues = hcsFill
                .Select(hc => hc.TamedValue == TValue.NoValue ? hc.Value : hc.TamedValue)
                .ToArray();
            IEnumerable<TValue> vals = Util.PairEnumerable(value1, value2).Concat(fillValues);
            fRank = new FullRank(SimpleRank.TwoPair, vals);
            hcsRank = hcsRank1.Concat(hcsRank2).ToArray();
            return true;
        }
        #endregion // Private hand rank predicate methods

        #region Private other methods
        private RankPredicate[] getRankPredicates(bool doAddStandard, bool doAddExtra)
        {
            List<SimpleRank> ranks = new List<SimpleRank>();
            if (doAddStandard) { ranks.AddRange(Ranker.StandardRanks); }
            if (doAddExtra) { ranks.AddRange(Ranker.ExtendedRanks); }
            return ranks
                .OrderByDescending(sr => sr)
                .Select(sr => this.RankPredicateMap[sr])
                .ToArray();
        }
        private static void setToNull(out FullRank fRank, out HandCard[] hcsRank, out HandCard[] hcsFill)
        {
            fRank = null;
            hcsRank = null;
            hcsFill = null;
        }
        #endregion // Private other methods
    }
}