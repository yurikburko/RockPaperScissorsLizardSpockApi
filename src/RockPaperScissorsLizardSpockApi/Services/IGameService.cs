using RockPaperScissorsLizardSpockApi.Models;

namespace RockPaperScissorsLizardSpockApi.Services
{
    public interface IGameService
    {
        IEnumerable<Choice> GetChoices();
        Task<Choice> GetRandomChoice();
        Task<RoundResult> PlayRoundAgainstBot(int playerChoiceId);
    }
}
