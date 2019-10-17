using NPinyin;

namespace yuyu.Infrastructure
{
    public class PinYinHelper
    {
        public static bool IsEqual(string hanZi, string pinYin)
        {
            bool res = ContainsFirstLetter(hanZi, pinYin) || ContainsFullPinyin(hanZi, pinYin);
            return res;
        }

        public static string getPinyin(string hanzi)
        {
            if (string.IsNullOrEmpty(hanzi))
                return "";

            return Pinyin.GetPinyin(hanzi).Trim().Replace(" ", "");
        }

        public static bool ContainsFullPinyin(string hanZi, string pinYin)
        {
            string strs = Pinyin.GetPinyin(hanZi).Trim().Replace(" ", "").ToUpper();
            return strs.Contains(pinYin.Trim().ToUpper());
        }

        public static bool ContainsFirstLetter(string hanZi, string pinYin)
        {
            string strs = Pinyin.GetInitials(hanZi).Trim().Replace(" ", "");
            return strs.Contains(pinYin.Trim().ToUpper());
        }
    }
}
