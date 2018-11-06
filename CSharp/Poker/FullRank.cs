// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker
{
    public class FullRank
    {
        #region Lifecycle
        public FullRank(SimpleRank sRank = SimpleRank.NoRank)
        {
            SRank = sRank;
        }

        public FullRank(SimpleRank sRank, IEnumerable<TValue> values)
        {
            SRank = sRank;
            Values = values.ToArray();
        }
        #endregion // Lifecycle

        #region Public properties / methods
        public override string ToString()
        {
            return "(" 
                + SRank.ToString() + ", [" + String.Join(", ", Values) + "]"
                + ")";
        }

        public SimpleRank SRank;

        public TValue[] Values;
        #endregion // Public properties / methods

        #region Fixed values
        public static FullRank LowestFullRank = new FullRank(SimpleRank.Kind2,
            new TValue[] { TValue.Six, TValue.Four, TValue.Three, TValue.Two, TValue.LowAce });
        #endregion // Specific values
    }
}