using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace Flake
{
    public static class Id
    {
        public static T Next<T>()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(Properties.Settings.Default.FlakeServiceBaseUri + "/next").Result;

                if (response.IsSuccessStatusCode)
                {
                    var Id = response.Content.ReadAsAsync<string>().Result;
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter.CanConvertFrom(typeof(string)))
                    {
                        return (T)converter.ConvertFromString(Id);
                    }

                    throw new Exception("Unable to convert ID to " + typeof(T).Name);
                }
            }

            throw new Exception();
        }

        public async static Task<T> NextAsync<T>()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = Properties.Settings.Default.FlakeServiceBaseUri;

                var response = await client.GetAsync("next");

                response.EnsureSuccessStatusCode();

                var Id = await response.Content.ReadAsAsync<string>();
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter.CanConvertFrom(typeof(string)))
                {
                    return (T)converter.ConvertFromString(Id);
                }

                throw new Exception("Unable to convert ID to " + typeof(T).Name);
            }

            throw new Exception();
        }

        public async static Task<string> Next(bool format = false)
        {
            var Id = await NextAsync<string>();

            if (format)
                return await Format(Id);
            else
                return Id;
        }

        public async static Task<string> Format(string Id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = Properties.Settings.Default.FlakeServiceBaseUri;

                HttpResponseMessage response = await client.GetAsync("format?id=" + Id);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsAsync<string>();
            }
        }

        public async static Task<string> Format(long Id)
        {
            return await Format(Id.ToString());
        }
    }
}
