using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
namespace Toolkit.Extention
{
    public static class Extention
    {
        static public async Task<T> GetJsonAsync<T>(this HttpClient client, string url)
        {
            var temp = await client.GetAsync(url);
            if (temp.IsSuccessStatusCode)
            {
                var str = await temp.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(str);
            }
            else 
                return default(T);
        }
        static public async Task<T> PostJsonAsync<T>(this HttpClient client, string url, StringContent content)
        {
            var temp = await client.PostAsync(url, content);
            var tempSring = await temp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(tempSring);
        }


    }
}
