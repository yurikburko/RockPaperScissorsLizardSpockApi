namespace RockPaperScissorsLizardSpockApi.Services
{
    public interface IRandomNumberGenerator
    {
        Task<int> GetRandomNumber();
    }
}
