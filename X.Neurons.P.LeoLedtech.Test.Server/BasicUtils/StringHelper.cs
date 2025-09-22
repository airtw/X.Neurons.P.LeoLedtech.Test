using System.Text.RegularExpressions;

namespace X.Neurons.P.LeoLedtech.Test.Server.BasicUtils
{
    public class StringHelper
    {
        /// <summary>
        /// 將輸入字串拆解成 prefix (非數字) 與 number (數字)
        /// </summary>
        public static (string Prefix, string NumberStr, int? Number) SplitPrefixAndNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return ("", "", null);

            // Regex：^([^0-9]*)([0-9]+)?$
            // Group 1 = 開頭非數字 (可為空)
            // Group 2 = 後續數字 (可為空)
            var match = Regex.Match(input, @"^([^0-9]*)([0-9]+)?$");

            if (!match.Success)
                return (input, "", null);

            string prefix = match.Groups[1].Value;       // 字母或其他非數字
            string numberStr = match.Groups[2].Value;    // 數字字串 (可空)
            int? number = int.TryParse(numberStr, out var n) ? n : (int?)null;

            return (prefix, numberStr, number);
        }
    }
}
