using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RockPaperScissorsLizardSpockApi.Services;

namespace RockPaperScissorsLizardSpockApi.Tests
{
    [TestClass]
    public class RandomNumberGeneratorTests
    {
        private ServiceCollection _serviceCollection { get; set; } = new ServiceCollection();

        private IRandomNumberGenerator _service => _serviceCollection.BuildServiceProvider().GetService<IRandomNumberGenerator>()!;

        [TestInitialize]
        public virtual void TestInitialize()
        {
            _serviceCollection.AddSingleton(TestsConfiguration.Configuration);

            _serviceCollection.AddSingleton(Mock.Of<ILogger<RandomNumberGenerator>>());

            _serviceCollection
                .AddHttpClient<IRandomNumberGenerator, RandomNumberGenerator>()
                .AddStandardResilienceHandler(options =>
                {
                    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
                    options.Retry.MaxRetryAttempts = 7;
                    options.Retry.Delay = TimeSpan.FromMilliseconds(100);
                });
        }

        [TestMethod]
        public async Task GetRandomNumber_should_return_random_number_from_external_generator()
        {
            var result = await _service.GetRandomNumber();
            result.Should().BeInRange(0, 255);
        }

        [TestMethod]
        public async Task GetRandomNumber_Resilience_on_parallel_requests_integration_test()
        {
            var requestsTasks = Enumerable.Range(1, 30).Select(i =>
            {
                return _service.GetRandomNumber();
            });

            var results = await Task.WhenAll(requestsTasks);

            results.Should().OnlyContain(n => n >= 0 && n <=255);
            results.Should().HaveCount(30);

            Assert.IsNotNull(results);
        }
    }
}
