namespace RockPaperScissorsLizardSpockApi.Services
{
    public class RandomNumberGenerator : IRandomNumberGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RandomNumberGenerator(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<int> GetRandomNumber()
        {
            var response = await _httpClient.GetAsync(_configuration["ExternalRandomGeneratorEndpoint"]);
            var responseText = await response.Content.ReadAsStringAsync();
            return int.Parse(responseText);
        }
    }
}
