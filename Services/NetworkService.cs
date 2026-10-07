namespace WorkoutApp.Services
{
    public static class NetworkService
    {
        //Local LAN IP on HTTP port 5163
        public static string BaseUrl => "http://10.0.0.229:5163/";

        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };

        public static HttpClient CreateClient() => _client;
    }
}