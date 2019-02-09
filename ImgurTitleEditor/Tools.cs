using System;
using System.Collections.Generic;
using System.Linq;

namespace ImgurTitleEditor
{
    public static class Tools
    {
        public static long LongOrDefault(string Source, long Default = long.MinValue)
        {
            long L;
            return long.TryParse(Source, out L) ? L : Default;
        }

        public static long IntOrDefault(string Source, int Default = int.MinValue)
        {
            int I;
            return int.TryParse(Source, out I) ? I : Default;
        }

        public static Dictionary<string, string> ParseFragment(Uri U)
        {
            var F = U.Fragment;
            var ret = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(F) && F[0] == '#')
            {
                foreach (var S in F.Substring(1).Split('&'))
                {
                    var K = S.Substring(0, S.Contains('=') ? S.IndexOf('=') : S.Length);
                    var V = K.Length == S.Length ? null : S.Substring(K.Length + 1);
                    if (ret.ContainsKey(K))
                    {
                        ret[K] += $", {V}";
                    }
                    else
                    {
                        ret.Add(K, V);
                    }
                }
            }
            return ret;
        }
    }
}
