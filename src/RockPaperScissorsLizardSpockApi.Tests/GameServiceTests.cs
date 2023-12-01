using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RockPaperScissorsLizardSpockApi.Models;
using RockPaperScissorsLizardSpockApi.Services;

namespace RockPaperScissorsLizardSpockApi.Tests
{
    [TestClass]
    public class GameServiceTests
    {
        private static List<Choice> DefaultChoices = new List<Choice>()
            {
                new Choice { Id = 1, Name = ChoiceType.Rock },
                new Choice { Id = 2, Name = ChoiceType.Paper },
                new Choice { Id = 3, Name = ChoiceType.Scissors },
                new Choice { Id = 4, Name = ChoiceType.Lizard },
                new Choice { Id = 5, Name = ChoiceType.Spock },
            };

        private static Dictionary<int, Choice> DefaultChoicesById = DefaultChoices.ToDictionary(x => x.Id, x => x);

        private static Dictionary<ChoiceType, Choice> DefaultChoicesByName = DefaultChoices.ToDictionary(x => x.Name, x => x);

        private static IOptions<List<Choice>> DefaultChoicesOptions = Options.Create(DefaultChoices);

        [TestMethod]
        public void GetChoices_should_return_all_possible_choices_from_app_config()
        {
            var choices = new List<Choice>()
            {
                new Choice { Id = 1, Name = ChoiceType.Rock },
                new Choice { Id = 2, Name = ChoiceType.Paper },
                new Choice { Id = 3, Name = ChoiceType.Scissors },
            };
            var optionsWrapper = Options.Create(choices);

            var service = CreateService(choicesOptions: optionsWrapper);

            var result = service.GetChoices();

            result.Should().BeEquivalentTo(choices);

        }

        [DataTestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 2)]
        [DataRow(2, 3)]
        [DataRow(3, 4)]
        [DataRow(4, 5)]
        [DataRow(5, 1)]
        [DataRow(9, 5)]
        [DataRow(56, 2)]
        [DataRow(255, 1)]

        public async Task GetRandomChoice_should_return_randomly_generated_choice_depend_on_external_number_generator(int randomNumber, int expectedChoiceId)
        {
            var randomNumberGeneratorMock = new Mock<IRandomNumberGenerator>();
            randomNumberGeneratorMock.Setup(x => x.GetRandomNumber()).ReturnsAsync(randomNumber);

            var service = CreateService(randomNumberGenerator: randomNumberGeneratorMock);

            var result = await service.GetRandomChoice();

            result.Should().BeEquivalentTo(new Choice { Id = expectedChoiceId, Name = DefaultChoicesById[expectedChoiceId].Name });
        }

        [DataTestMethod]
        [DataRow(ChoiceType.Scissors, ChoiceType.Paper, PlayerRoundResult.Win, DisplayName = "Scissors cuts paper")]
        [DataRow(ChoiceType.Paper, ChoiceType.Scissors, PlayerRoundResult.Lose, DisplayName = "Scissors cuts paper")]

        [DataRow(ChoiceType.Paper, ChoiceType.Rock, PlayerRoundResult.Win, DisplayName = "Paper covers rock")]
        [DataRow(ChoiceType.Rock, ChoiceType.Paper, PlayerRoundResult.Lose, DisplayName = "Paper covers rock")]

        [DataRow(ChoiceType.Rock, ChoiceType.Lizard, PlayerRoundResult.Win, DisplayName = "Rock crushes lizard")]
        [DataRow(ChoiceType.Lizard, ChoiceType.Rock, PlayerRoundResult.Lose, DisplayName = "Rock crushes lizard")]

        [DataRow(ChoiceType.Lizard, ChoiceType.Spock, PlayerRoundResult.Win, DisplayName = "Lizard poisons Spock")]
        [DataRow(ChoiceType.Spock, ChoiceType.Lizard, PlayerRoundResult.Lose, DisplayName = "Lizard poisons Spock")]

        [DataRow(ChoiceType.Spock, ChoiceType.Scissors, PlayerRoundResult.Win, DisplayName = "Spock smashes scissors")]
        [DataRow(ChoiceType.Scissors, ChoiceType.Spock, PlayerRoundResult.Lose, DisplayName = "Spock smashes scissors")]

        [DataRow(ChoiceType.Scissors, ChoiceType.Lizard, PlayerRoundResult.Win, DisplayName = "Scissors decapitates lizard")]
        [DataRow(ChoiceType.Lizard, ChoiceType.Scissors, PlayerRoundResult.Lose, DisplayName = "Scissors decapitates lizard")]

        [DataRow(ChoiceType.Lizard, ChoiceType.Paper, PlayerRoundResult.Win, DisplayName = "Lizard eats paper")]
        [DataRow(ChoiceType.Paper, ChoiceType.Lizard, PlayerRoundResult.Lose, DisplayName = "Lizard eats paper")]

        [DataRow(ChoiceType.Paper, ChoiceType.Spock, PlayerRoundResult.Win, DisplayName = "Paper disproves Spock")]
        [DataRow(ChoiceType.Spock, ChoiceType.Paper, PlayerRoundResult.Lose, DisplayName = "Paper disproves Spock")]

        [DataRow(ChoiceType.Spock, ChoiceType.Rock, PlayerRoundResult.Win, DisplayName = "Spock vaporizes rock")]
        [DataRow(ChoiceType.Rock, ChoiceType.Spock, PlayerRoundResult.Lose, DisplayName = "Spock vaporizes rock")]

        [DataRow(ChoiceType.Rock, ChoiceType.Scissors, PlayerRoundResult.Win, DisplayName = "Rock crushes scissors")]
        [DataRow(ChoiceType.Scissors, ChoiceType.Rock, PlayerRoundResult.Lose, DisplayName = "Rock crushes scissors")]
        public async Task PlayRoundAgainstBot_Scissors_cuts_paper(
            ChoiceType playerChoice, ChoiceType botChoice, PlayerRoundResult playerRoundResult)
        {
            var randomNumberGeneratorMock = new Mock<IRandomNumberGenerator>();
            randomNumberGeneratorMock.Setup(x => x.GetRandomNumber()).ReturnsAsync(DefaultChoicesByName[botChoice].Id - 1);

            var service = CreateService(randomNumberGenerator: randomNumberGeneratorMock);

            var result = await service.PlayRoundAgainstBot(DefaultChoicesByName[playerChoice].Id);

            result.Should().BeEquivalentTo(new RoundResult 
            {
                Player = DefaultChoicesByName[playerChoice].Id,
                Bot = DefaultChoicesByName[botChoice].Id,
                Results = playerRoundResult
            });
        }

        [DataTestMethod]
        [DataRow(ChoiceType.Rock, ChoiceType.Rock)]
        [DataRow(ChoiceType.Paper, ChoiceType.Paper)]
        [DataRow(ChoiceType.Scissors, ChoiceType.Scissors)]
        [DataRow(ChoiceType.Lizard, ChoiceType.Lizard)]
        [DataRow(ChoiceType.Spock, ChoiceType.Spock)]
        public async Task PlayRoundAgainstBot_should_return_tie_if_choices_are_the_same(ChoiceType playerChoice, ChoiceType botChoice)
        {
            var randomNumberGeneratorMock = new Mock<IRandomNumberGenerator>();
            randomNumberGeneratorMock.Setup(x => x.GetRandomNumber()).ReturnsAsync(DefaultChoicesByName[botChoice].Id - 1);

            var service = CreateService(randomNumberGenerator: randomNumberGeneratorMock);

            var result = await service.PlayRoundAgainstBot(DefaultChoicesByName[playerChoice].Id);

            result.Should().BeEquivalentTo(new RoundResult
            {
                Player = DefaultChoicesByName[playerChoice].Id,
                Bot = DefaultChoicesByName[botChoice].Id,
                Results = PlayerRoundResult.Tie
            });
        }

        public async Task PlayRoundAgainstBot_should_throw_Exception_if_playerChoiceId_is_invalid()
        {
            var service = CreateService();

            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => service.PlayRoundAgainstBot(999));
        }


        static IGameService CreateService(IOptions<List<Choice>> choicesOptions = null,
                                        Mock<IRandomNumberGenerator> randomNumberGenerator = null)
        => new GameService(
                choicesOptions ?? DefaultChoicesOptions,
                randomNumberGenerator?.Object ?? Mock.Of<IRandomNumberGenerator>()
            );
    }
}