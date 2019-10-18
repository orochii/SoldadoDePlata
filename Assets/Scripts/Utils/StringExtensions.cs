using System;
using System.Globalization;
using System.Text;

public static class StringEx {
    public static string UnicodeSafeSubstring(this string str, int startIndex, int length) {
        // Argument catching
        if (str == null) throw new ArgumentNullException("str");
        if (startIndex < 0 || startIndex > str.Length) throw new ArgumentOutOfRangeException("startIndex");
        if (length < 0) throw new ArgumentOutOfRangeException("length");
        if (startIndex + length > str.Length) throw new ArgumentOutOfRangeException("length");
        if (length == 0) return string.Empty;
        // 
        StringBuilder sb = new StringBuilder(length);
        int end = startIndex + length;
        TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(str, startIndex);
        // Iterate through string
        while (enumerator.MoveNext()) {
            string grapheme = enumerator.GetTextElement();
            startIndex += grapheme.Length;
            if (startIndex > length) break;
            // Skip initial Low Surrogates/Combining Marks
            if (sb.Length == 0) {
                if (char.IsLowSurrogate(grapheme[0])) continue;
                UnicodeCategory cat = char.GetUnicodeCategory(grapheme, 0);
                if (cat == UnicodeCategory.NonSpacingMark || cat == UnicodeCategory.SpacingCombiningMark || cat == UnicodeCategory.EnclosingMark) {
                    continue;
                }
            }
            // Add grapheme
            sb.Append(grapheme);
            if (startIndex == length) break;
        }
        // Return result.
        return sb.ToString();
    }
}