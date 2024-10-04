using System.Security.Cryptography;
using System.Text;

namespace CrossCutting.Helpers
{
    public class HmacVerifierHelper
    {
        public bool VerifyHmac(string xSignature, string dataId, string xRequestId, string secret)
        {
            var parts = xSignature.Split(',');
            string ts = null;
            string hash = null;

            foreach (var part in parts)
            {
                var keyValue = part.Split('=', 2);
                if (keyValue.Length == 2)
                {
                    var key = keyValue[0].Trim();
                    var value = keyValue[1].Trim();
                    if (key.Equals("ts", StringComparison.OrdinalIgnoreCase))
                    {
                        ts = value;
                    }
                    else if (key.Equals("v1", StringComparison.OrdinalIgnoreCase))
                    {
                        hash = value;
                    }
                }
            }

            var manifest = $"id:{dataId};request-id:{xRequestId};ts:{ts};";

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                var sha = hmac.ComputeHash(Encoding.UTF8.GetBytes(manifest));
                var shaHex = BitConverter.ToString(sha).Replace("-", "").ToLower();

                Console.WriteLine($"sha: {shaHex}");
                Console.WriteLine($"hash: {hash}");
                // Compara a hash calculada com a recebida
                return shaHex == hash;
            }
        }
    }
}
