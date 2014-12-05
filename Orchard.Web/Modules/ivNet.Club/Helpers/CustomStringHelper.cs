
using System;
using System.Text.RegularExpressions;
using ivNet.Club.Entities;

namespace ivNet.Club.Helpers
{
    public static class CustomStringHelper
    {
        public static string BuildKey(string[] items)
        {
            var key = String.Join(string.Empty, items).ToLowerInvariant();
            return Regex.Replace(key, "[^0-9a-z]", string.Empty);                      
        }

        public static string GenerateInitialPassword(Member member)
        {
            return string.Format("{0}{1}1",
              member.Firstname.ToLowerInvariant(),
              member.Firstname.ToUpperInvariant())
              .Replace(" ", string.Empty);
        }

    }
}