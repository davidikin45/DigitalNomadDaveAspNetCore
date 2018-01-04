using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Solution.Base.Helpers
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class ByteArraySerializer
    {
        public static byte[] SerializeToXMLBytes<T>(this T m)
        {
            DataContractSerializer serializer = new DataContractSerializer(m.GetType());
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, m);
                return ms.ToArray();
            }
        }

        public static byte[] SerializeToBytes<T>(this T m)
        {
         
            using (var ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, m);
                return ms.ToArray();
            }
        }

        public static T DeserializeFromBytes<T>(this byte[] byteArray)
        {
            using (var ms = new MemoryStream(byteArray))
            {
                var obj = new BinaryFormatter().Deserialize(ms);
                return (T)obj;
            }
        }

        public static String BytesToString(this byte[] byteArray)
        {
            return System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
        }

       
    }
}
