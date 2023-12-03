using RockPaperScissorsLizardSpockApi.Models;

namespace RockPaperScissorsLizardSpockApi.Services
{
    public class RandomNumberGenerator : IRandomNumberGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RandomNumberGenerator> _logger;

        public RandomNumberGenerator(HttpClient httpClient, IConfiguration configuration, ILogger<RandomNumberGenerator> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<int> GetRandomNumber()
        {
            var response = await _httpClient.GetAsync(_configuration["ExternalRandomGeneratorEndpoint"]);

            response.EnsureSuccessStatusCode();

            var randomNumberModel = await response.Content.ReadFromJsonAsync<RandomNumberModel>();
            if (randomNumberModel == null)
            {
                throw new Exception("Error while getting random number via external random generator.");
            }

            _logger.LogInformation($"Received random number from external generator: {randomNumberModel.Random}");
                
            return randomNumberModel.Random;
        }
    }
}
