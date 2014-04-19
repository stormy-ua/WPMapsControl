using System;

namespace MapsControl.Infrastructure
{
    public static class FormattingExtensions
    {
        public static string ReplaceTileTemplates(this string input, int levelOfDetail, int x, int y)
        {
            string result = input
                .Replace("{zoom}", levelOfDetail.ToString())
                .Replace("{x}", x.ToString())
                .Replace("{y}", y.ToString());

            return result;
        }
    }
}
