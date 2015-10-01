//-------------------------------------------------------------------------------------------------
// Code from Handbook of exact string-matching algorithms http://www-igm.univ-mlv.fr/~lecroq/string/node15.html#SECTION00150
//-------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.SoftBytes.Text
{
    /// <summary>
    /// Class that implements Boyer-Moore and related exact string-matching algorithms.
    /// </summary>
    /// <remarks>
    /// From "Handbook of exact string-matching algorithms".
    ///   by Christian Charras and Thierry Lecroq
    ///   chapter 15
    /// http://www-igm.univ-mlv.fr/~lecroq/string/node15.html#SECTION00150
    /// </remarks>
    public sealed class BoyerMoore
    {
        private Int32[] m_badCharacterShift;
        private Int32[] m_goodSuffixShift;
        private Int32[] m_suffixes;
        private String m_pattern;

        public BoyerMoore(String pattern)
        {
            /* Preprocessing */
            m_pattern           = pattern;
            m_badCharacterShift = BuildBadCharacterShift(pattern);
            m_suffixes          = FindSuffixes(pattern);
            m_goodSuffixShift   = BuildGoodSuffixShift(pattern, m_suffixes);
        }

        /// <summary>
        /// Build the bad character shift array.
        /// </summary>
        /// <param name="pattern">Pattern for search.</param>
        /// <returns>bad character shift array.</returns>
        private static Int32[] BuildBadCharacterShift(String pattern)
        {
            Int32[] badCharacterShift = new Int32[256];

            for (Int32 c = 0; c < badCharacterShift.Length; ++c)
            {
                badCharacterShift[c] = pattern.Length;
            }

            for (Int32 i = 0; i < pattern.Length - 1; ++i)
            {
                badCharacterShift[pattern[i]] = pattern.Length - i - 1;
            }

            return badCharacterShift;
        }

        /// <summary>
        /// Find suffixes in the pattern.
        /// </summary>
        /// <param name="pattern">Pattern for search.</param>
        /// <returns>Suffix array.</returns>
        private static Int32[] FindSuffixes(String pattern)
        {
            Int32 f = 0, g;

            Int32 patternLength = pattern.Length;
            Int32[] suffixes = new Int32[pattern.Length + 1];

            suffixes[patternLength - 1] = patternLength;
            g = patternLength - 1;
            for (Int32 i = patternLength - 2; i >= 0; --i)
            {
                if (i > g && suffixes[i + patternLength - 1 - f] < i - g)
                {
                    suffixes[i] = suffixes[i + patternLength - 1 - f];
                }
                else
                {
                    if (i < g)
                    {
                        g = i;
                    }
                    
                    f = i;

                    while (g >= 0 && (pattern[g] == pattern[g + patternLength - 1 - f]))
                    {
                        --g;
                    }

                    suffixes[i] = f - g;
                }
            }

            return suffixes;
        }

        /// <summary>
        /// Build the good suffix array.
        /// </summary>
        /// <param name="pattern">Pattern for search.</param>
        /// <param name="suff">(find out what is suff).</param>
        /// <returns>Good suffix shift array.</returns>
        private static Int32[] BuildGoodSuffixShift(String pattern, Int32[] suff)
        {
            Int32 patternLength = pattern.Length;
            Int32[] goodSuffixShift = new Int32[pattern.Length + 1];

            for (Int32 i = 0; i < patternLength; ++i)
            {
                goodSuffixShift[i] = patternLength;
            }

            Int32 j = 0;
            for (Int32 i = patternLength - 1; i >= -1; --i)
            {
                if (i == -1 || suff[i] == i + 1)
                {
                    for (; j < patternLength - 1 - i; ++j)
                    {
                        if (goodSuffixShift[j] == patternLength)
                        {
                            goodSuffixShift[j] = patternLength - 1 - i;
                        }
                    }
                }
            }

            for (Int32 i = 0; i <= patternLength - 2; ++i)
            {
                goodSuffixShift[patternLength - 1 - suff[i]] = patternLength - 1 - i;
            }

            return goodSuffixShift;
        }

        /// <summary>
        /// Return all matched of the pattern in the specified text using the .NET String.indexOf API.
        /// </summary>
        /// <param name="text">text to be searched.</param>
        /// <param name="startingIndex">Index at which search begins.</param>
        /// <returns>IEnumerable which returns the indexes of pattern matches.</returns>
        public IEnumerable<Int32> BclMatch(String text, Int32 startingIndex)
        {
            Int32 patternLength = m_pattern.Length;
            Int32 index = startingIndex;
            do
            {
                index = text.IndexOf(m_pattern, index, StringComparison.InvariantCultureIgnoreCase);
                if (index < 0)
                {
                    yield break;
                }

                yield return index;
                index += patternLength;
            } while (true);
        }

        /// <summary>
        /// Return all matched of the pattern in the specified text using the .NET String.indexOf API.
        /// </summary>
        /// <param name="text">text to be searched.</param>
        /// <returns>IEnumerable which returns the indexes of pattern matches.</returns>
        public IEnumerable<Int32> BclMatch(String text)
        {
            return BclMatch(text, 0);
        }

        /// <summary>
        /// Return all matches of the pattern in specified text using the Horspool algorithm.
        /// </summary>
        /// <param name="text">text to be searched.</param>
        /// <param name="startingIndex">Index at which search begins.</param>
        /// <returns>IEnumerable which returns the indexes of pattern matches.</returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.LayoutRules", "SA1503:CurlyBracketsMustNotBeOmitted", Justification = "That's OK.")]
        public IEnumerable<Int32> HorspoolMatch(String text, Int32 startingIndex)
        {
            Int32 patternLength = m_pattern.Length;
            Int32 textLength = text.Length;

            /* Searching */
            Int32 index = startingIndex;
            while (index <= textLength - patternLength)
            {
                Int32 unmatched;
                for (unmatched = patternLength - 1;
                  unmatched >= 0 && m_pattern[unmatched] == text[unmatched + index];
                  --unmatched); // empty.

                if (unmatched < 0)
                {
                    yield return index;
                }

                index += m_badCharacterShift[text[index + patternLength - 1]];
            }
        }

        /// <summary>
        /// Return all matches of the pattern in specified text using the Horspool algorithm.
        /// </summary>
        /// <param name="text">text to be searched.</param>
        /// <returns>IEnumerable which returns the indexes of pattern matches.</returns>
        public IEnumerable<Int32> HorspoolMatch(String text)
        {
            return HorspoolMatch(text, 0);
        }

        /// <summary>
        /// Return all matches of the pattern in specified text using the Boyer-Moore algorithm.
        /// </summary>
        /// <param name="text">text to be searched.</param>
        /// <param name="startingIndex">Index at which search begins.</param>
        /// <returns>IEnumerable which returns the indexes of pattern matches.</returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.LayoutRules", "SA1503:CurlyBracketsMustNotBeOmitted", Justification = "That's OK.")]
        public IEnumerable<Int32> BoyerMooreMatch(String text, Int32 startingIndex)
        {
            Int32 patternLength = m_pattern.Length;
            Int32 textLength = text.Length;

            /* Searching */
            Int32 index = startingIndex;
            while (index <= textLength - patternLength)
            {
                Int32 unmatched;
                for (unmatched = patternLength - 1;
                  unmatched >= 0 && (m_pattern[unmatched] == text[unmatched + index]);
                  --unmatched); // empty.

                if (unmatched < 0)
                {
                    yield return index;
                    index += m_goodSuffixShift[0];
                }
                else
                {
                    index += Math.Max(m_goodSuffixShift[unmatched],
                        m_badCharacterShift[text[unmatched + index]] - patternLength + 1 + unmatched);
                }
            }
        }

        /// <summary>
        /// Return all matches of the pattern in specified text using the Boyer-Moore algorithm.
        /// </summary>
        /// <param name="text">text to be searched.</param>
        /// <returns>IEnumerable which returns the indexes of pattern matches.</returns>
        public IEnumerable<Int32> BoyerMooreMatch(String text)
        {
            return BoyerMooreMatch(text, 0);
        }

        /// <summary>
        /// Return all matches of the pattern in specified text using the Turbo Boyer-Moore algorithm.
        /// </summary>
        /// <param name="text">text to be searched.</param>
        /// <param name="startingIndex">Index at which search begins.</param>
        /// <returns>IEnumerable which returns the indexes of pattern matches.</returns>
        public IEnumerable<Int32> TurboBoyerMooreMatch(String text, Int32 startingIndex)
        {
            Int32 patternLength = m_pattern.Length;
            Int32 textLength = text.Length;

            /* Searching */
            Int32 index = startingIndex;
            Int32 overlap = 0;
            Int32 shift = patternLength;

            while (index <= textLength - patternLength)
            {
                Int32 unmatched = patternLength - 1;

                while (unmatched >= 0 && (m_pattern[unmatched] == text[unmatched + index]))
                {
                    --unmatched;
                    if (overlap != 0 && unmatched == patternLength - 1 - shift)
                    {
                        unmatched -= overlap;
                    }
                }

                if (unmatched < 0)
                {
                    yield return index;
                    shift = m_goodSuffixShift[0];
                    overlap = patternLength - shift;
                }
                else
                {
                    Int32 matched = patternLength - 1 - unmatched;
                    Int32 turboShift = overlap - matched;
                    Int32 bcShift = m_badCharacterShift[text[unmatched + index]] - patternLength + 1 + unmatched;
                    shift = Math.Max(turboShift, bcShift);
                    shift = Math.Max(shift, m_goodSuffixShift[unmatched]);

                    if (shift == m_goodSuffixShift[unmatched])
                    {
                        overlap = Math.Min(patternLength - shift, matched);
                    }
                    else
                    {
                        if (turboShift < bcShift)
                        {
                            shift = Math.Max(shift, overlap + 1);
                        }

                        overlap = 0;
                    }
                }

                index += shift;
            }
        }

        /// <summary>
        /// Return all matches of the pattern in specified text using the Turbo Boyer-Moore algorithm.
        /// </summary>
        /// <param name="text">text to be searched.</param>
        /// <returns>IEnumerable which returns the indexes of pattern matches.</returns>
        public IEnumerable<Int32> TurboBoyerMooreMatch(String text)
        {
            return TurboBoyerMooreMatch(text, 0);
        }

        /// <summary>
        /// Return all matches of the pattern in specified text using the Apostolico-GiancarloMatch algorithm.
        /// </summary>
        /// <param name="text">text to be searched.</param>
        /// <param name="startingIndex">Index at which search begins.</param>
        /// <returns>IEnumerable which returns the indexes of pattern matches.</returns>
        public IEnumerable<Int32> ApostolicoGiancarloMatch(String text, Int32 startingIndex)
        {
            Int32 patternLength = m_pattern.Length;
            Int32 textLength = text.Length;
            Int32[] skip = new Int32[patternLength];
            Int32 shift;

            /* Searching */
            Int32 index = startingIndex;
            while (index <= textLength - patternLength)
            {
                Int32 unmatched = patternLength - 1;
                while (unmatched >= 0)
                {
                    Int32 skipLength = skip[unmatched];
                    Int32 suffixLength = m_suffixes[unmatched];
                    if (skipLength > 0)
                    {
                        if (skipLength > suffixLength)
                        {
                            if (unmatched + 1 == suffixLength)
                            {
                                unmatched = (-1);
                            }
                            else
                            {
                                unmatched -= suffixLength;
                            }

                            break;
                        }

                        unmatched -= skipLength;
                        if (skipLength < suffixLength)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (m_pattern[unmatched] == text[unmatched + index])
                        {
                            --unmatched;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (unmatched < 0)
                {
                    yield return index;
                    skip[patternLength - 1] = patternLength;
                    shift = m_goodSuffixShift[0];
                }
                else
                {
                    skip[patternLength - 1] = patternLength - 1 - unmatched;
                    shift = Math.Max(m_goodSuffixShift[unmatched],
                      m_badCharacterShift[text[unmatched + index]] - patternLength + 1 + unmatched);
                }

                index += shift;

                for (Int32 copy = 0; copy < patternLength - shift; ++copy)
                {
                    skip[copy] = skip[copy + shift];
                }

                for (Int32 clear = 0; clear < shift; ++clear)
                {
                    skip[patternLength - shift + clear] = 0;
                }
            }
        }

        /// <summary>
        /// Return all matches of the pattern in specified text using the Apostolico-GiancarloMatch algorithm.
        /// </summary>
        /// <param name="text">text to be searched.</param>
        /// <returns>IEnumerable which returns the indexes of pattern matches.</returns>
        public IEnumerable<Int32> ApostolicoGiancarloMatch(String text)
        {
            return ApostolicoGiancarloMatch(text, 0);
        }
    }
}
