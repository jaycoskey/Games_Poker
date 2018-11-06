// Copyright 2011, by Jay Coskey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.Reflection;

namespace Poker
{
    public static class ExtensionMethods
    {
        public static IEnumerable<T> Append<T>(this T firstItem, IEnumerable<T> otherItems) {
            yield return firstItem;
            foreach (T item in otherItems) { yield return item; }
            yield break;
        }

        public static IEnumerable<U> EnumerableChoice<T, U>(
            this IEnumerable<IGrouping<T, U>> groups,
            Func<U, bool> pred = null)
        {
            foreach (IGrouping<T, U> group in groups)
            {
                if (pred == null)
                {
                    yield return group.First();
                }
                else
                {
                    yield return group.First(pred);
                }
            }
            yield break;
        }

        public static TValue Next(this TValue val, int n = 1)
        {
            return (TValue)(((int)val) + n);
        }

        public static TValue Prev(this TValue val, int n = 1)
        {
            return val.Next(-1 * n);
        }

        public static T SecondOrDefault<T>(this IEnumerable<T> items)
        {
            return items.Skip(1).FirstOrDefault();
        }

        public static string ToText(this SimpleRank sRank)
        {
            return toText(sRank);
        }

        public static string ToText(this TSuit s) {
            return toText(s);
        }

        public static string ToText(this TValue v)
        {
            return toText(v);
        }

        /// <remarks>
        ///     Note: This method supports extension methods, but is not an extension method itself.
        /// </remarks>
        private static string toText(object obj)
        {
            Type type = obj.GetType();
            MemberInfo[] memInfo = type.GetMember(obj.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return obj.ToString();
        }
    }
}