// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker
{
    public class EnumerableComparer<T> : IComparer<IEnumerable<T>> where T : IComparable<T>
    {
        public EnumerableComparer() { }

        int IComparer<IEnumerable<T>>.Compare(IEnumerable<T> a, IEnumerable<T> b)
        {
            var aIter = a.GetEnumerator();
            var bIter = b.GetEnumerator();
            while (true)
            {
                bool isMoreA = aIter.MoveNext();
                bool isMoreB = bIter.MoveNext();
                if (isMoreA && isMoreB)
                {
                    int cmp = aIter.Current.CompareTo(bIter.Current);
                    if (cmp != 0) { return cmp; }
                }
                else
                {
                    if (!isMoreA && !isMoreB) { return 0; }
                    if (!isMoreA) { return -1; }
                    if (!isMoreB) { return 1; }
                }
            }
        }
    }
}
