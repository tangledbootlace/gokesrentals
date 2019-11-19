using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jose;
using Newtonsoft.Json;

namespace Folly.Auth
{

    public static class JWT
    {
        private static readonly byte[] KEY = Encoding.UTF8.GetBytes(Auth.ENCRYPTION_KEY);
        private static readonly NewtonsoftMapper mapper = new NewtonsoftMapper();

        static JWT()
        {
            Jose.JWT.DefaultSettings.JsonMapper = new NewtonsoftMapper();
        }

        public static string Encode(object obj)
        {
            return Jose.JWT.Encode(obj, KEY, JweAlgorithm.A256KW, JweEncryption.A256CBC_HS512, JweCompression.DEF);
        }

        public static T Decode<T>(string token)
        {
            if (token == null)
                return default(T);

            try
            {
                return Jose.JWT.Decode<T>(token, KEY);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public class Container<T>
        {
            public Container()
            {

            }

            public Container(T data) :
                this(data, TimeSpan.FromDays(30))
            {
            }

            public Container(T data, TimeSpan expiresIn)
            {
                Expires = DateTime.UtcNow.Add(expiresIn);
                Data = data;
            }

            public DateTime Expires { get; set; }
            public T Data { get; set; }

            public bool IsExpired
            {
                get
                {
                    return Expires < DateTime.UtcNow;
                }
            }
        }

        public class NewtonsoftMapper : IJsonMapper
        {
            public string Serialize(object obj)
            {
                return JsonConvert.SerializeObject(obj);
            }

            public T Parse<T>(string json)
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}
