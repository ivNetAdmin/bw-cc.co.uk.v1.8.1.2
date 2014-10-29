
using System;

namespace ivNet.Club.Helpers
{
    public static class CustomStringHelper
    {
        public static string BuildKey(string[] items)
        {
            return String.Join(string.Empty, items)
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty).ToLowerInvariant();            
        }
    }
}