﻿using System.Security.Cryptography;
using System.Text;

namespace Xunkong.Core.Hoyolab
{
    public abstract class DynamicSecret
    {

        public static readonly string AppVersion_2101 = "2.10.1";
        public static readonly string AppVersion_2161 = "2.16.1";

        private static readonly string ApiSalt = "4a8knnbk5pbjqsrudp3dq484m9axoc5g"; // @Azure99 respect original author

        private static readonly string ApiSalt_v2 = "xV8v4Qu54lUKrEYFZkJhB8cuOh9Asafs";

        public static string CreateSecret()
        {
            var t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string r = GetRandomString(t);
            var bytes = MD5.HashData(Encoding.UTF8.GetBytes($"salt={ApiSalt}&t={t}&r={r}"));
            var check = Convert.ToHexString(bytes).ToLower();
            return $"{t},{r},{check}";
        }


        public static string CreateSecretV2(string url, object? postBody = null)
        {
            int t = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string r = Random.Shared.Next(100000, 200000).ToString();
            string b = postBody is null ? "" : JsonSerializer.Serialize(postBody);
            string q = "";
            string[] urls = url.Split('?');
            if (urls.Length == 2)
            {
                string[] queryParams = urls[1].Split('&').OrderBy(x => x).ToArray();
                q = string.Join("&", queryParams);
            }
            var bytes = MD5.HashData(Encoding.UTF8.GetBytes($"salt={ApiSalt_v2}&t={t}&r={r}&b={b}&q={q}"));
            var check = Convert.ToHexString(bytes).ToLower();
            string result = $"{t},{r},{check}";
            return result;
        }


        private static string GetRandomString(long timestamp)
        {
            var sb = new StringBuilder(6);
            var random = new Random((int)timestamp);
            for (int i = 0; i < 6; i++)
            {
                int v8 = random.Next(0, 32768) % 26;
                int v9 = 87;
                if (v8 < 10)
                {
                    v9 = 48;
                }
                _ = sb.Append((char)(v8 + v9));
            }
            return sb.ToString();
        }

    }
}
