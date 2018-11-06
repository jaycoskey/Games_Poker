// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker
{
    class Util
    {
        public static IEnumerable<T> PairEnumerable<T>(T item1, T item2)
        {
            yield return item1;
            yield return item2;
            yield break;
        }

        public static IEnumerable<T> SingletonEnumerable<T>(T item)
        {
            yield return item;
            yield break;
        }

        /// <summary>
        ///     Fill two containers with items from two sources, respectively.  Stop when count items have been found.
        ///     Input sources are assumed to be in the order in which items are meant to be taken.
        /// </summary>
        public static void TakeFill<T>(int count, IEnumerable<T> src1, out List<T> dst1, IEnumerable<T> src2, out List<T> dst2)
        {
            int numFromSrc1 = Math.Min(count, src1.Count());
            int numFromSrc2 = Math.Min(count - numFromSrc1, src2.Count());
            if (numFromSrc1 + numFromSrc2 != count)
            {
                throw new ApplicationException("Error: could not find enough cards to complete a hand");
            }
            dst1 = new List<T>(src1.Take(numFromSrc1));
            dst2 = new List<T>(src2.Take(numFromSrc2));
        }
    }
}
