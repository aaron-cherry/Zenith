namespace WorkoutApp.Services
{
    public static class NetworkService
    {
        //Local LAN IP on HTTP port 5163
        public static string BaseUrl => "http://10.0.0.229:5163/";

        public static HttpClient CreateClient()
        {
            return new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }
    }
}