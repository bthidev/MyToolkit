using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Toolkit.Extention
{
    public static class Extention
    {
        public static async Task<T> GetJsonAsync<T>(this HttpClient client, string url)
        {
            var temp = await client.GetAsync(url).ConfigureAwait(true);
            _ = await temp.Content.ReadAsStringAsync().ConfigureAwait(true);
            if (temp.IsSuccessStatusCode)
            {
                var str = await temp.Content.ReadAsStringAsync().ConfigureAwait(true);
                return JsonSerializer.Deserialize<T>(str);
            }

            return default;
        }

        public static async Task<T> PostJsonAsync<T>(this HttpClient client, string url, JsonContent content)
        {
            var temp = await client.PostAsync(url, content).ConfigureAwait(true);
            var tempSring = await temp.Content.ReadAsStringAsync().ConfigureAwait(true);
            return JsonSerializer.Deserialize<T>(tempSring);
        }
    }
}
