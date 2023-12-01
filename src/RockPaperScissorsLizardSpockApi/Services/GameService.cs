using Microsoft.Extensions.Options;
using RockPaperScissorsLizardSpockApi.Models;

namespace RockPaperScissorsLizardSpockApi.Services
{
    public class GameService : IGameService
    {
        private readonly Dictionary<int, Choice> _choices;
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public GameService(IOptions<List<Choice>> choicesOptions, IRandomNumberGenerator randomNumberGenerator)
        {
            _choices = choicesOptions.Value.ToDictionary(x => x.Id, x => x);
            _randomNumberGenerator = randomNumberGenerator;
        }

        public IEnumerable<Choice> GetChoices()
        {
            return _choices.Values;
        }

        private Choice GetChoiceById(int choiceId)
        {
            return _choices.TryGetValue(choiceId, out var value)
                ? value
                : throw new ArgumentOutOfRangeException($"Invalid ChoiceId: {choiceId}");
        }

        public async Task<Choice> GetRandomChoice()
        {
            var randomNumber = await _randomNumberGenerator.GetRandomNumber();
            return _choices[(randomNumber % _choices.Count()) + 1];
        }

        // Game rules:
        //  Scissors cuts paper.
        //  Paper covers rock.
        //  Rock crushes lizard.
        //  Lizard poisons Spock.
        //  Spock smashes scissors.
        //  Scissors decapitates lizard.
        //  Lizard eats paper.
        //  Paper disproves Spock.
        //  Spock vaporizes rock.
        //  Rock crushes scissors.
        public async Task<RoundResult> PlayRoundAgainstBot(int playerChoiceId)
        {
            var playerChoice = GetChoiceById(playerChoiceId);
            var botRandomChoice = await GetRandomChoice();
            var botChoice = botRandomChoice.Name;

            PlayerRoundResult roundResult;

            if (playerChoice == botRandomChoice)
            {
                roundResult = PlayerRoundResult.Tie;
            }
            else
            {
                roundResult = playerChoice.Name switch
                {
                    ChoiceType.Rock => botChoice == ChoiceType.Scissors || botChoice == ChoiceType.Lizard ? PlayerRoundResult.Win : PlayerRoundResult.Lose,
                    ChoiceType.Paper => botChoice == ChoiceType.Rock || botChoice == ChoiceType.Spock ? PlayerRoundResult.Win : PlayerRoundResult.Lose,
                    ChoiceType.Scissors => botChoice == ChoiceType.Paper || botChoice == ChoiceType.Lizard ? PlayerRoundResult.Win : PlayerRoundResult.Lose,
                    ChoiceType.Lizard => botChoice == ChoiceType.Paper || botChoice == ChoiceType.Spock ? PlayerRoundResult.Win : PlayerRoundResult.Lose,
                    ChoiceType.Spock => botChoice == ChoiceType.Rock || botChoice == ChoiceType.Scissors ? PlayerRoundResult.Win : PlayerRoundResult.Lose,
                    _ => throw new NotImplementedException(),
                };
            }

            return new RoundResult()
            {
                Results = roundResult,
                Player = playerChoiceId,
                Bot = botRandomChoice.Id
            };
        }
    }
}
