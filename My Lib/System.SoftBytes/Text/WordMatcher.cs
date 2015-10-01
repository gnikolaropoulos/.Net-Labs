//-------------------------------------------------------------------------------------------------
// Code from Nikos Baxevanis http://nikobaxevanis.com/
//-------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace System.SoftBytes.Text
{
	public static class WordMatcher
	{
        private static readonly List<String> s_ignoredItems = new List<String>(new[] { "(TV)", "V", "VS", "@" });

		public static Boolean Matches(String source, String target)
		{
            if (String.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentNullException("source");
            }

            if (String.IsNullOrWhiteSpace(target))
            {
                throw new ArgumentNullException("target");
            }

			Boolean found = false;

			foreach (String pattern in source.Split(' '))
			{
                if (s_ignoredItems.Contains(pattern.ToUpperInvariant()))
                {
                    continue;
                }

				BoyerMoore matcher = new BoyerMoore(pattern);

				foreach (String actual in target.Split(' '))
				{
					if (matcher.TurboBoyerMooreMatch(actual).Count() > 0)
					{
						found = true;
					}
					else
					{
						Int32 dist = LevenshteinDistance.Compute(pattern, actual);
						if (dist == 1 && (pattern.Length == actual.Length))
						{
							found = true;
						}
						else if (dist <= pattern.Length - actual.Length)
						{
							found = true;
						}
						else
						{
							found = false;
						}
					}
				}
			}

			return found;
		}
	}
}
