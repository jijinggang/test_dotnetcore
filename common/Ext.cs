namespace common
{
    public static class Ext
    {

        //模拟给标准库的类扩展新方法,注意this的用法
        //字符串反转
        public static string Reverse(this string s)
        {
            var chars = s.ToCharArray();
            int len = s.Length;
            for (int i = 0; i < len / 2; i++)
            {
                int j = len - i - 1;
                char c = chars[i];
                chars[i] = chars[j];
                chars[j] = c;
            }
            return new string(chars);
        }

        public static byte[] ToBytes(this string s)
        {
            return System.Text.Encoding.UTF8.GetBytes(s);
        }
    }
}