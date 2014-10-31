
using System;
using System.Text.RegularExpressions;

namespace ivNet.Club.Helpers
{
    public static class CustomStringHelper
    {
        public static string BuildKey(string[] items)
        {
            var key = String.Join(string.Empty, items).ToLowerInvariant();
            return Regex.Replace(key, "[^0-9a-z]", string.Empty);                      
        }
    }
}