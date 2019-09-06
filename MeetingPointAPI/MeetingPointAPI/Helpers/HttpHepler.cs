using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MeetingPointAPI.Helpers
{
    public static class HttpHepler
    {
        private static async Task<T> Execute<T>(string url) where T : class
        {
            try
            {
                var response = await new HttpClient().GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                    return null;

                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseBody);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
