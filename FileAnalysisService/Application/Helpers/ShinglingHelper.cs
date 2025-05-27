using System;
using System.Collections.Generic;
using System.Linq;

namespace FileAnalysisService.Application.Helpers
{
    public static class ShinglingHelper
    {
        public static HashSet<string> GetShingles(string text, int k = 5)
        {
            var shingles = new HashSet<string>();
            for (int i = 0; i + k <= text.Length; i++)
            {
                shingles.Add(text.Substring(i, k));
            }
            return shingles;
        }

        public static double JaccardSimilarity(string a, string b, int k = 5)
        {
            var setA = GetShingles(a, k);
            var setB = GetShingles(b, k);
            var intersection = setA.Intersect(setB).Count();
            var union = setA.Union(setB).Count();
            return union == 0 ? 0 : (double)intersection / union;
        }
    }
}