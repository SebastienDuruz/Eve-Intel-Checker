namespace EveIntelChecker.Data
{
    public class EVE_API
    {
        private HttpClient Client { get; set; }
        private string BaseURL { get; set; }

        public EVE_API()
        {
            this.Client = new HttpClient();
            this.BaseURL = "https://api.coingecko.com/api/v3/";

        }
    }
}
