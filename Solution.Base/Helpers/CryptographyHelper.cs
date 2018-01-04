using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Helpers
{
    public static class CryptographyHelper
    {
        public static string GetHash(Byte[] bytes)
        {
            var checksum = new byte[0];
            checksum = MD5.Create().ComputeHash(bytes);
            return Convert.ToBase64String(checksum, 0, checksum.Length);
        }

        public static string GetHash(Stream stream)
        {
            var checksum = new byte[0];
            checksum = MD5.Create().ComputeHash(stream);
            return Convert.ToBase64String(checksum, 0, checksum.Length);
        }
    }
}
