namespace WorkoutApp.Services
{
    public static class NetworkService
    {
        //Local LAN IP on HTTP port 5163
        public static string BaseUrl => "https://zenithapi-eonb.onrender.com/";
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };

        public static HttpClient CreateClient() => _client;
    }
}